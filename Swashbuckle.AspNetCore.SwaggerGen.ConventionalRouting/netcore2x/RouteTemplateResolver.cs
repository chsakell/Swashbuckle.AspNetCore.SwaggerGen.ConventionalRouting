#if NETCOREAPP2_2

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public class RouteTemplateResolver : IRouteTemplateResolver
    {
        public string ResolveRouteTemplate(ActionDescriptor actionDescriptor, SwaggerRoutingOptions options)
        {
            var routes = ConventionalRoutingSwaggerGen.ROUTES;
            var template = string.Empty;
            var templateDefaults = new Dictionary<string, string>();

            if (routes != null)
            {
                foreach (var router in routes)
                {
                    templateDefaults.Clear();
                    var isDefaultArea = false;
                    var isDefaultController = false;
                    var isDefaultAction = false;

                    var route = router as Route;
                    if (route != null)
                    {
                        if (options?.IgnoreTemplateFunc != null)
                        {
                            var ignoreRoute = options.IgnoreTemplateFunc(route.RouteTemplate);
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
                                else if (firstPart.Name.Equals("area"))
                                {
                                    if (string.IsNullOrEmpty(actionDescArea))
                                    {
                                        template = null;
                                        break;
                                    }

                                    if (hasConstraint &&
                                        (!IsCustomConstraint(routeConstraint) || IsRegexConstraint(routeConstraint)))
                                    {
                                        passConstraint =
                                            PassConstraint(actionMatchConfig.Area, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(actionMatchConfig.Area) && route.Defaults != null &&
                                        route.Defaults.TryGetValue("area", out var defaultArea) &&
                                        !string.IsNullOrEmpty(defaultArea?.ToString()))
                                    {
                                        isDefaultArea = actionMatchConfig.Area
                                            .Equals(defaultArea.ToString(),
                                                StringComparison.InvariantCultureIgnoreCase);
                                        if (isDefaultArea)
                                        {
                                            templateDefaults.Add("area", actionMatchConfig.Area);
                                        }
                                    }

                                    template += $"{actionMatchConfig.Area}/";
                                }
                                else if (firstPart.Name.Equals("controller"))
                                {
                                    if (hasConstraint && 
                                        (!IsCustomConstraint(routeConstraint) || IsRegexConstraint(routeConstraint)))
                                    {
                                        passConstraint =
                                            PassConstraint(actionMatchConfig.Controller, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    if (route.Defaults != null &&
                                        route.Defaults.TryGetValue("controller", out var defaultController) &&
                                        !string.IsNullOrEmpty(defaultController?.ToString()))
                                    {
                                        var configController = WithNoSuffix(actionMatchConfig.Controller, "Controller")
                                            .ToLower();
                                        isDefaultController = configController.Equals(defaultController.ToString(),
                                            StringComparison.InvariantCultureIgnoreCase);

                                        if (isDefaultController)
                                        {
                                            templateDefaults.Add("controller",
                                                $"{WithNoSuffix(actionMatchConfig.Controller, "Controller")}");
                                        }
                                    }

                                    template += $"{WithNoSuffix(actionMatchConfig.Controller, "Controller")}/";
                                }
                                else if (firstPart.Name.Equals("action"))
                                {
                                    if (hasConstraint &&
                                        (!IsCustomConstraint(routeConstraint) || IsRegexConstraint(routeConstraint)))
                                    {
                                        passConstraint =
                                            PassConstraint(actionMatchConfig.Action, routeConstraint);

                                        if (!passConstraint)
                                        {
                                            template = null;
                                            break;
                                        }
                                    }

                                    if (route.Defaults != null &&
                                        route.Defaults.TryGetValue("action", out var defaultAction) &&
                                        !string.IsNullOrEmpty(defaultAction?.ToString()))
                                    {
                                        isDefaultAction = actionMatchConfig.Action
                                            .Equals(defaultAction.ToString(),
                                                StringComparison.InvariantCultureIgnoreCase);
                                        if (isDefaultAction)
                                        {
                                            templateDefaults.Add("action", actionMatchConfig.Action);
                                        }
                                    }

                                    template += $"{actionMatchConfig.Action}/";
                                }
                                else if (firstPart.IsParameter)
                                {
                                    var hasActionDescParameter =
                                        HasActionDescriptorParameter(actionDescriptor, firstPart.Name, out var paramBindingInfo);

                                    if (hasActionDescParameter && hasConstraint)
                                    {
                                        var parameterInfo = actionDescriptor.Parameters.First(param =>
                                            param.Name.Equals(firstPart.Name,
                                                StringComparison.InvariantCultureIgnoreCase));

                                        passConstraint =
                                            PassConstraint(parameterInfo, routeConstraint);

                                        if (passConstraint && paramBindingInfo == null)
                                        {
                                            var routeParam = route.ParsedTemplate.Parameters[paramIndex];
                                            if (routeParam.Name.Equals(firstPart.Name,
                                                StringComparison.CurrentCultureIgnoreCase) && !routeParam.IsOptional)
                                            {
                                                template += $"{{{firstPart.Name}}}/";
                                                break;
                                            }
                                        }

                                        if (!passConstraint && parameterInfo.BindingInfo?.BindingSource.Id != "Query")
                                        {
                                            template = null;
                                            break;
                                        }

                                        if (parameterInfo?.BindingInfo?.BindingSource.Id == "Path")
                                        {
                                            templateDefaults.Clear();
                                        }
                                    }

                                    if (hasActionDescParameter && paramBindingInfo != null)
                                    {
                                        actionParametersUsed.Add(firstPart.Name);
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

            if (!string.IsNullOrEmpty(template) && options.SkipDefaults && templateDefaults.Any())
            {
                template = SkipDefaultsFromTemplate(templateDefaults, template);
            }

            return template;
        }

        private string SkipDefaultsFromTemplate(Dictionary<string, string> defaults, string template)
        {
            if (defaults.TryGetValue("action", out var defaultAction))
            {
                template = template.Replace(defaultAction, string.Empty,
                        StringComparison.InvariantCultureIgnoreCase)
                    .Replace("//", "/");

                if (defaults.TryGetValue("controller", out var defaultController))
                {
                    template = template.Replace(defaultController, string.Empty,
                            StringComparison.InvariantCultureIgnoreCase)
                        .Replace("//", "/");

                    if (defaults.TryGetValue("area", out var defaultArea))
                    {
                        template = template.Replace(defaultArea, string.Empty,
                                StringComparison.CurrentCultureIgnoreCase)
                            .Replace("//", "/");
                    }
                };
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
            else if (constraint is RegexRouteConstraint regexRouteConstraint)
            {
                var parameterType = parameter.ParameterType;
                if (parameterType.IsValueType)
                {
                    var regex = regexRouteConstraint.Constraint;
                    var defaultTypeValue = Activator.CreateInstance(parameterType);
                    isValid = regex.IsMatch(defaultTypeValue.ToString());
                }
                else if (constraint is AlphaRouteConstraint && parameter.BindingInfo == null)
                {
                    isValid = true;
                }
            }
            else if (constraint is DateTimeRouteConstraint)
            {
                isValid = parameter.ParameterType == typeof(DateTime);
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

        private bool IsRegexConstraint(IRouteConstraint constraint)
        {
            return constraint is RegexInlineRouteConstraint;
        }

        private string GetRouteArea(Route route, out bool isParameter)
        {
            string area = null;

            isParameter = route.ParsedTemplate.Parameters.Any(p => p.Name.Equals("area"));

            if (route.Defaults.TryGetValue("area", out var areaObj))
            {
                return areaObj.ToString();
            }

            return area;
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