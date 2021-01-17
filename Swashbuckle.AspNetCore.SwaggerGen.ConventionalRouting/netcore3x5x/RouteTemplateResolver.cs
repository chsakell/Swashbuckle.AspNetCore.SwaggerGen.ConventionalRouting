#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Routing.Patterns;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public class RouteTemplateResolver : IRouteTemplateResolver
    {
        public string ResolveRouteTemplate(ActionDescriptor actionDescriptor, SwaggerRoutingOptions options)
        {
            var routes = ConventionalRoutingSwaggerGen.ROUTES;
            var template = string.Empty;

            if (routes != null)
            {
                foreach (var route in routes)
                {
                    if (route != null)
                    {
                        if (options?.IgnoreTemplateFunc != null)
                        {
                            var ignoreRoute = options.IgnoreTemplateFunc(route.RawText);
                            if (ignoreRoute)
                            {
                                continue;
                            }
                        }

                        var routeArea = GetRouteArea(route, out bool isAreaParameter);
                        var routeController = GetRouteController(route, out var isControllerParameter);
                        var routeAction = GetRouteAction(route, out bool isActionParameter);

                        var actionDescArea = GetActionDescriptorArea(actionDescriptor);
                        var actionDescController = GetActionDescriptorController(actionDescriptor);
                        var actionDescAction = GetActionDescriptorAction(actionDescriptor);

                        var routeMatchConfig = new MatchConfig(routeArea, routeController, routeAction)
                        {
                            IsAreaParameter = isAreaParameter,
                            IsControllerParameter = isControllerParameter,
                            IsActionParameter = isActionParameter
                        };

                        var actionMatchConfig = new MatchConfig(actionDescArea, actionDescController, actionDescAction);

                        if (MatchConfig.Match(routeMatchConfig, actionMatchConfig))
                        {
                            var paramIndex = 0;
                            var actionParametersUsed = new List<string>();
                            foreach (var segment in route.PathSegments)
                            {
                                var firstPart = segment.Parts.First();
                                var firstPartName = GetRoutePatternPartPropertyValue(firstPart, out bool firstPartIsOptional);

                                var hasConstraint = !firstPart.IsLiteral && route.ParameterPolicies != null &&
                                                          route.ParameterPolicies.Any(c => c.Key.Equals(firstPartName));

                                RoutePatternParameterPolicyReference policyReference = null;
                                bool passConstraint = false;
                                if (hasConstraint)
                                {
                                    policyReference = route.ParameterPolicies
                                        .FirstOrDefault(policy => policy.Key.Equals(firstPartName))
                                        .Value.FirstOrDefault();
                                }

                                if (firstPart.IsLiteral)
                                {
                                    template += $"{firstPartName}/";
                                }
                                else if (firstPartName.Equals("area"))
                                {
                                    if (string.IsNullOrEmpty(actionDescArea))
                                    {
                                        template = null;
                                        break;
                                    }

                                    if (hasConstraint)
                                    {
                                        passConstraint =
                                            PassPolicyReference(actionMatchConfig.Area, policyReference);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    template += $"{actionMatchConfig.Area}/";
                                }
                                else if (firstPartName.Equals("controller"))
                                {
                                    if (hasConstraint && !IsCustomPolicyReference(policyReference))
                                    {
                                        passConstraint =
                                            PassPolicyReference(actionMatchConfig.Controller, policyReference);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    template += $"{WithNoSuffix(actionMatchConfig.Controller, "Controller")}/";
                                }
                                else if (firstPartName.Equals("action"))
                                {
                                    if (hasConstraint)
                                    {
                                        passConstraint =
                                            PassPolicyReference(actionMatchConfig.Action, policyReference);

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
                                        HasActionDescriptorParameter(actionDescriptor, firstPartName, 
                                            out var paramBindingInfo);

                                    if (hasActionDescParameter && hasConstraint)
                                    {
                                        var parameterInfo = actionDescriptor.Parameters.First(param =>
                                            param.Name.Equals(firstPartName,
                                                StringComparison.InvariantCultureIgnoreCase));

                                        passConstraint =
                                            IsValidRoutePolicy(parameterInfo, policyReference);

                                        if (!passConstraint && parameterInfo.BindingInfo?.BindingSource.Id != "Query")
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    if (hasActionDescParameter && paramBindingInfo != null)
                                    {
                                        actionParametersUsed.Add(firstPartName);
                                        template += $"{{{firstPartName}{(firstPartIsOptional ? "?" : string.Empty)}}}/";
                                    }
                                    else if (!firstPartIsOptional)
                                    {
                                        var param = route.Parameters.FirstOrDefault(param =>
                                            param.Name.Equals(firstPartName));
                                        if (hasConstraint && IsCustomPolicyReference(policyReference) ||
                                            route.Parameters.ToList().IndexOf(param) != route.Parameters.Count - 1)
                                        {
                                            template += $"{{{firstPartName}}}/";
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
                                            ((HttpMethodActionConstraint)httpMethod).HttpMethods.Count() == 1 &&
                                            ((HttpMethodActionConstraint)httpMethod).HttpMethods.First().Equals("GET"))
                                        {
                                            var unusedParameters = actionDescriptor.Parameters.Where(param =>
                                                !actionParametersUsed.Contains(param.Name));

                                            if (!string.IsNullOrEmpty(template) &&
                                                unusedParameters.All(param => param.BindingInfo == null))
                                            {
                                                break;
                                            }

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

        private string GetRoutePatternPartPropertyValue(RoutePatternPart part, out bool isOptional)
        {
            var value = string.Empty;

            if (part is RoutePatternLiteralPart literalPart)
            {
                value = literalPart.Content;
            }
            else if (part is RoutePatternParameterPart parameterPart)
            {
                value = parameterPart.Name;
            }

            bool.TryParse(part.GetType().GetProperty("IsOptional")?.GetValue(part).ToString(), out isOptional);

            return value;
        }

        private bool PassPolicyReference(string term, RoutePatternParameterPolicyReference policy)
        {
            var isValid = false;

            if (string.IsNullOrEmpty(term) || policy == null)
            {
                return false; // something is wrong here..?
            }

            if (policy.Content.StartsWith("regex"))
            {
                isValid = new Regex(policy.Content.Replace("regex", string.Empty)).IsMatch(term);
            }

            return isValid;
        }

        private bool IsValidRoutePolicy(ParameterDescriptor parameter, RoutePatternParameterPolicyReference policy)
        {
            var isValid = false;

            if(policy != null && string.IsNullOrEmpty(policy.Content) 
                              && policy.ParameterPolicy is RegexRouteConstraint regexRouteConstraint)
            {
                var parameterType = parameter.ParameterType;
                if (parameterType.IsValueType)
                {
                    var regex = regexRouteConstraint.Constraint;
                    var defaultTypeValue = Activator.CreateInstance(parameterType);
                    isValid = regex.IsMatch(defaultTypeValue.ToString());
                }
                else if (policy.ParameterPolicy is AlphaRouteConstraint && parameter.BindingInfo == null)
                {
                    isValid = true;
                }

                return isValid;
            }

            switch (policy.Content)
            {
                case "alpha":
                    isValid = parameter.ParameterType == typeof(string);
                    break;
                case "int":
                    isValid = parameter.ParameterType == typeof(int);
                    break;
                case "bool":
                    isValid = parameter.ParameterType == typeof(bool);
                    break;
                case "datetime":
                    isValid = parameter.ParameterType == typeof(DateTime);
                    break;
                case "decimal":
                    isValid = parameter.ParameterType == typeof(decimal);
                    break;
                case "double":
                    isValid = parameter.ParameterType == typeof(double);
                    break;
                case "float":
                    isValid = parameter.ParameterType == typeof(float);
                    break;
                case "guid":
                    isValid = parameter.ParameterType == typeof(Guid);
                    break;
                case "long":
                    isValid = parameter.ParameterType == typeof(long);
                    break;
            }

            return isValid;
        }

        private bool IsCustomPolicyReference(RoutePatternParameterPolicyReference policyReference)
        {
            if (policyReference.Content == null && policyReference.ParameterPolicy != null)
            {
                return true;
            }

            bool isCustomPolicyReference = !(policyReference.Content.Equals("alpha") ||
                                             policyReference.Content.StartsWith("regex"));

            return isCustomPolicyReference;
        }

        private string GetRouteArea(RoutePattern route, out bool isParameter)
        {
            string area = null;

            isParameter = route.Parameters.Any(p => p.Name.Equals("area"));

            if (route.Defaults.TryGetValue("area", out var areaObj))
            {
                return areaObj.ToString();
            }

            return area;
        }

        private string GetRouteController(RoutePattern route, out bool isParameter)
        {
            var controller = string.Empty;

            isParameter = route.Parameters.Any(p => p.Name.Equals("controller"));

            if (route.Defaults.TryGetValue("controller", out var controllerObj))
            {
                return WithSuffix(controllerObj.ToString(), "Controller");
            }

            return controller;
        }

        private string GetRouteAction(RoutePattern route, out bool isParameter)
        {
            var action = string.Empty;

            isParameter = route.Parameters.Any(p => p.Name.Equals("action"));

            if (route.Defaults.TryGetValue("action", out var controllerObj))
            {
                return controllerObj.ToString();
            }

            return action;
        }
        
        private string GetActionDescriptorArea(ActionDescriptor actionDescriptor)
        {
            string area = null;

            if (actionDescriptor.RouteValues.TryGetValue("area", out var areaObj))
            {
                return areaObj;
            }

            return area;
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

        private bool HasActionDescriptorParameter(ActionDescriptor actionDescriptor, string parameter, out BindingInfo bindingInfo)
        {
            var param = actionDescriptor.Parameters.FirstOrDefault(param =>
                param.Name.Equals(parameter, StringComparison.InvariantCultureIgnoreCase));

            bindingInfo = param?.BindingInfo;

            return param != null;
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
        string ResolveRouteTemplate(ActionDescriptor actionDescriptor, SwaggerRoutingOptions options);
    }
}

#endif