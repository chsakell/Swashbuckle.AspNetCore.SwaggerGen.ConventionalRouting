#if (NETCOREAPP3_0 || NETCOREAPP3_1)

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public interface IConventionalRoutingActionDescriptorCollectionProvider :
        IActionDescriptorCollectionProvider
    {

    }

    public class ConventionalRoutingActionDescriptorCollectionProvider : ActionDescriptorCollectionProvider, IConventionalRoutingActionDescriptorCollectionProvider
    {
        private readonly IRouteTemplateResolver _routeTemplateResolver;
        private readonly IActionDescriptorProvider[] _actionDescriptorProviders;
        private readonly IActionDescriptorChangeProvider[] _actionDescriptorChangeProviders;

        // The lock is used to protect WRITES to the following (do not need to protect reads once initialized).
        private readonly object _lock;
        private ActionDescriptorCollection _collection;
        private IChangeToken _changeToken;
        private CancellationTokenSource _cancellationTokenSource;
        private int _version = 0;
        private SwaggerRoutingOptions _swaggerRoutingOptions;

        public ConventionalRoutingActionDescriptorCollectionProvider(
            IEnumerable<IActionDescriptorProvider> actionDescriptorProviders,
            IEnumerable<IActionDescriptorChangeProvider> actionDescriptorChangeProviders,
            IRouteTemplateResolver routeTemplateResolver,
            IOptions<SwaggerRoutingOptions> swaggerRoutingOptions)
        {
            _routeTemplateResolver = routeTemplateResolver;
            _actionDescriptorProviders = actionDescriptorProviders
                .OrderBy(p => p.Order)
                .ToArray();

            _actionDescriptorChangeProviders = actionDescriptorChangeProviders.ToArray();

            _lock = new object();

            // IMPORTANT: this needs to be the last thing we do in the constructor. Change notifications can happen immediately!
            ChangeToken.OnChange(
                GetCompositeChangeToken,
                UpdateCollection);

            if (swaggerRoutingOptions != null)
            {
                _swaggerRoutingOptions = swaggerRoutingOptions.Value;
            }
        }

        /// <summary>
        /// Returns a cached collection of <see cref="ActionDescriptor" />.
        /// </summary>
        public override ActionDescriptorCollection ActionDescriptors
        {
            get
            {
                Initialize();
                Debug.Assert(_collection != null);
                Debug.Assert(_changeToken != null);

                return _collection;
            }
        }

        /// <summary>
        /// Gets an <see cref="IChangeToken"/> that will be signaled after the <see cref="ActionDescriptors"/>
        /// collection has changed.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public override IChangeToken GetChangeToken()
        {
            Initialize();
            Debug.Assert(_collection != null);
            Debug.Assert(_changeToken != null);

            return _changeToken;
        }

        private IChangeToken GetCompositeChangeToken()
        {
            if (_actionDescriptorChangeProviders.Length == 1)
            {
                return _actionDescriptorChangeProviders[0].GetChangeToken();
            }

            var changeTokens = new IChangeToken[_actionDescriptorChangeProviders.Length];
            for (var i = 0; i < _actionDescriptorChangeProviders.Length; i++)
            {
                changeTokens[i] = _actionDescriptorChangeProviders[i].GetChangeToken();
            }

            return new CompositeChangeToken(changeTokens);
        }

        private void Initialize()
        {
            // Using double-checked locking on initialization because we fire change token callbacks
            // when the collection changes. We don't want to do that repeatedly for redundant changes.
            //
            // The main call path of this code on the first call is async initialization from Endpoint Routing
            // which is done in a non-blocking way so in practice no caller will ever block here.
            if (_collection == null)
            {
                lock (_lock)
                {
                    if (_collection == null)
                    {
                        UpdateCollection();
                    }
                }
            }
        }

        private void UpdateCollection()
        {
            // Using the lock to initialize writes means that we serialize changes. This eliminates
            // the potential for changes to be processed out of order - the risk is that newer data
            // could be overwritten by older data.
            lock (_lock)
            {
                var context = new ActionDescriptorProviderContext();

                for (var i = 0; i < _actionDescriptorProviders.Length; i++)
                {
                    _actionDescriptorProviders[i].OnProvidersExecuting(context);
                }

                for (var i = _actionDescriptorProviders.Length - 1; i >= 0; i--)
                {
                    _actionDescriptorProviders[i].OnProvidersExecuted(context);
                }

                // The sequence for an update is important because we don't want anyone to obtain
                // the new change token but the old action descriptor collection.
                // 1. Obtain the old cancellation token source (don't trigger it yet)
                // 2. Set the new action descriptor collection
                // 3. Set the new change token
                // 4. Trigger the old cancellation token source
                //
                // Consumers who poll will observe a new action descriptor collection at step 2 - they will see
                // the new collection and ignore the change token.
                //
                // Consumers who listen to the change token will re-query at step 4 - they will see the new collection
                // and new change token.
                //
                // Anyone who acquires the collection and change token between steps 2 and 3 will be notified of
                // a no-op change at step 4.

                // Step 1.
                var oldCancellationTokenSource = _cancellationTokenSource;

                foreach (var actionDescriptor in context.Results)
                {
                    if (actionDescriptor.AttributeRouteInfo == null || actionDescriptor.ActionConstraints == null)
                    {
                        if (actionDescriptor.ActionConstraints == null)
                        {
                            var httpMethod = GetHttpMethod(actionDescriptor);
                            // check from config for default HTTP method
                            actionDescriptor.ActionConstraints = new List<IActionConstraintMetadata>
                            {
                                new HttpMethodActionConstraint(new[] {httpMethod})
                            };
                        }
                        var routeTemplate = _routeTemplateResolver.ResolveRouteTemplate(actionDescriptor, _swaggerRoutingOptions);

                        if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                        {
                            var apiExplorerAttribute = controllerActionDescriptor
                                ?.MethodInfo
                                ?.GetCustomAttributes(typeof(SwaggerConfigAttribute), inherit: true)
                                .FirstOrDefault();
                            if (apiExplorerAttribute is SwaggerConfigAttribute apiExplorerSettings)
                            {
                                if (apiExplorerSettings.IgnoreApi)
                                {
                                    routeTemplate = string.Empty;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(routeTemplate))
                        {
                            actionDescriptor.AttributeRouteInfo = new AttributeRouteInfo
                            {
                                Template = routeTemplate
                            };
                        }
                    }
                }

                // Step 2.
                _collection = new ActionDescriptorCollection(
                    new ReadOnlyCollection<ActionDescriptor>(context.Results),
                    _version++);

                // Step 3.
                _cancellationTokenSource = new CancellationTokenSource();
                _changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);

                // Step 4 - might be null if it's the first time.
                oldCancellationTokenSource?.Cancel();
            }
        }

        private string GetHttpMethod(ActionDescriptor descriptor)
        {
            var method = "GET";

            var action = GetActionDescriptorAction(descriptor).ToLower();

            if (action.StartsWith("post"))
            {
                method = "POST";
            }
            else if (action.StartsWith("update") || action.StartsWith("put"))
            {
                method = "PUT";
            }
            else if (action.StartsWith("patch"))
            {
                method = "PATCH";
            }
            else if (action.StartsWith("delete") || action.StartsWith("remove"))
            {
                method = "DELETE";
            }

            return method;
        }

        private string GetActionDescriptorAction(ActionDescriptor actionDescriptor)
        {
            var controller = string.Empty;

            if (actionDescriptor.RouteValues.TryGetValue("action", out var controllerObj))
            {
                return controllerObj;
            }

            return controller;
        }
    }
}

#endif