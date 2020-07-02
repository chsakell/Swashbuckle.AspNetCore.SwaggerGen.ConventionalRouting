using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public class RouteTemplateResolver : IRouteTemplateResolver
    {
        public string ResolveRouteTemplate(ActionDescriptor actionDescriptor)
        {
            var routes = ConventionalRoutingSwaggerGen.ROUTES;
            var template = string.Empty;

            if (routes != null)
            {
                foreach (var router in routes)
                {
                    var route = router as Route;
                    if (route != null)
                    {
                        var routeController = GetRouteController(route, out var isControllerParameter);
                        var routeAction = GetRouteAction(route, out bool isActionParameter);

                        var actionDescController = GetActionDescriptorController(actionDescriptor);
                        var actionDescAction = GetActionDescriptorAction(actionDescriptor);

                        var routeMatchConfig = new MatchConfig(routeController, routeAction)
                        {
                            IsControllerParameter = isControllerParameter,
                            IsActionParameter = isActionParameter
                        };

                        var actionMatchConfig = new MatchConfig(actionDescController, actionDescAction);

                        if (MatchConfig.Match(routeMatchConfig, actionMatchConfig))
                        {
                            var paramIndex = 0;
                            foreach (var segment in route.ParsedTemplate.Segments)
                            {
                                var firstPart = segment.Parts.First();

                                var hasConstraint = route.Constraints != null &&
                                                           route.Constraints.Any(c => c.Key.Equals(firstPart.Name));

                                IRouteConstraint routeConstraint = null;
                                bool passConstraint = false;
                                if (hasConstraint)
                                {
                                    routeConstraint = route.Constraints[firstPart.Name];
                                }

                                if (firstPart.IsLiteral)
                                {
                                    template += $"{firstPart.Text}/";
                                }
                                else if (firstPart.Name.Equals("controller"))
                                {
                                    if (hasConstraint && !IsCustomConstraint(routeConstraint))
                                    {
                                        passConstraint =
                                            PassConstraint(actionMatchConfig.Controller, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    template += $"{WithNoSuffix(actionMatchConfig.Controller, "Controller")}/";
                                }
                                else if (firstPart.Name.Equals("action"))
                                {
                                    if (hasConstraint)
                                    {
                                        passConstraint =
                                            PassConstraint(actionMatchConfig.Action, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    template += $"{actionMatchConfig.Action}/";
                                }
                                else if (firstPart.IsParameter)
                                {
                                    var hasActionDescParameter =
                                        HasActionDescriptorParameter(actionDescriptor, firstPart.Name);

                                    if (hasActionDescParameter && hasConstraint)
                                    {
                                        var parameterInfo = actionDescriptor.Parameters.First(param =>
                                            param.Name.Equals(firstPart.Name,
                                                StringComparison.InvariantCultureIgnoreCase));

                                        passConstraint =
                                            PassConstraint(parameterInfo, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    if (hasActionDescParameter)
                                    {
                                        template += $"{{{firstPart.Name}{(firstPart.IsOptional ? "?" : string.Empty)}}}/";
                                    }
                                    else if (!firstPart.IsOptional)
                                    {
                                        if (hasConstraint && IsCustomConstraint(routeConstraint) ||
                                            route.ParsedTemplate.Parameters.IndexOf(firstPart) != route.ParsedTemplate.Parameters.Count -1)
                                        {
                                            template += $"{{{firstPart.Name}}}/";
                                        }
                                        else
                                        {
                                            template = null;
                                            break;
                                        }
                                    }
                                    else if (actionDescriptor.Parameters.Count > 0)
                                    {
                                        var httpMethod = actionDescriptor.ActionConstraints
                                            .FirstOrDefault(c => c.GetType() == typeof(HttpMethodActionConstraint));

                                        if (httpMethod != null &&
                                            ((HttpMethodActionConstraint) httpMethod).HttpMethods.Count() == 1 &&
                                            ((HttpMethodActionConstraint) httpMethod).HttpMethods.First().Equals("GET"))
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    paramIndex++;
                                }

                            }
                        }

                        // exit on first route match
                        if (!string.IsNullOrEmpty(template))
                        {
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(template))
            {
                template = template.TrimEnd('/');
            }

            return template;
        }

        private bool PassConstraint(string term, IRouteConstraint constraint)
        {
            var isValid = false;

            if (string.IsNullOrEmpty(term) || constraint == null)
            {
                return false; // something is wrong here..?
            }

            if (constraint is RegexInlineRouteConstraint regexInlineRouteConstraint)
            {
                isValid = regexInlineRouteConstraint.Constraint.Match(term).Success;
            }

            return isValid;
        }

        private bool PassConstraint(ParameterDescriptor parameter, IRouteConstraint constraint)
        {
            var isValid = false;

            if (constraint is OptionalRouteConstraint optionalRouteConstraint)
            {
                isValid = IsValidRouteConstraint(parameter, optionalRouteConstraint.InnerConstraint);
            }
            else
            {
                isValid = IsValidRouteConstraint(parameter, constraint);
            }

            return isValid;
        }

        private bool IsValidRouteConstraint(ParameterDescriptor parameter, IRouteConstraint constraint)
        {
            var isValid = false;

            if (constraint is IntRouteConstraint intRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(int);
            }
            if (constraint is BoolRouteConstraint boolRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(bool);
            }
            else if (constraint is DateTimeRouteConstraint dateTimeRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(DateTime);
            }
            else if (constraint is DecimalRouteConstraint decimalRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(decimal);
            }
            else if (constraint is DoubleRouteConstraint doubleRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(double);
            }
            else if (constraint is FloatRouteConstraint floatRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(float);
            }
            else if (constraint is GuidRouteConstraint guidRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(Guid);
            }
            else if (constraint is LongRouteConstraint longRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(long);
            }
            else if (constraint is AlphaRouteConstraint alphaRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(string);
            }
            

            return isValid;
        }

        private bool IsCustomConstraint(IRouteConstraint constraint)
        {
            bool isCustomConstraint = !(constraint is OptionalRouteConstraint ||
                                        constraint is IntRouteConstraint ||
                                        constraint is BoolRouteConstraint ||
                                        constraint is DateTimeRouteConstraint ||
                                        constraint is DecimalRouteConstraint ||
                                        constraint is DoubleRouteConstraint ||
                                        constraint is FloatRouteConstraint ||
                                        constraint is GuidRouteConstraint ||
                                        constraint is LongRouteConstraint ||
                                        constraint is AlphaRouteConstraint);

            return isCustomConstraint;
        }

        private string GetRouteController(Route route, out bool isParameter)
        {
            var controller = string.Empty;

            isParameter = route.ParsedTemplate.Parameters.Any(p => p.Name.Equals("controller"));

            if (route.Defaults.TryGetValue("controller", out var controllerObj))
            {
                return WithSuffix(controllerObj.ToString(), "Controller");
            }

            return controller;
        }

        private string GetRouteAction(Route route, out bool isParameter)
        {
            var action = string.Empty;

            isParameter = route.ParsedTemplate.Parameters.Any(p => p.Name.Equals("action"));

            if (route.Defaults.TryGetValue("action", out var controllerObj))
            {
                return controllerObj.ToString();
            }

            return action;
        }

        private string GetActionDescriptorController(ActionDescriptor actionDescriptor)
        {
            var controller = string.Empty;

            if (actionDescriptor.RouteValues.TryGetValue("controller", out var controllerObj))
            {
                return WithSuffix(controllerObj, "Controller");
            }

            return controller;
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

        private bool HasActionDescriptorParameter(ActionDescriptor actionDescriptor, string parameter)
        {
            return actionDescriptor.Parameters.Any(param =>
                param.Name.Equals(parameter, StringComparison.InvariantCultureIgnoreCase));
        }

        private string WithSuffix(string term, string suffix)
        {
            if (!term.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                return $"{term}{suffix}";
            }

            return term;
        }

        private string WithNoSuffix(string term, string suffix)
        {
            if (term.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
            {
                term = term.Replace(suffix, string.Empty, StringComparison.InvariantCultureIgnoreCase);
                return $"{term}";
            }

            return term;
        }
    }

    public interface IRouteTemplateResolver
    {
        string ResolveRouteTemplate(ActionDescriptor actionDescriptor);
    }
}
