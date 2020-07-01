using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public class RouteTemplateResolver : IRouteTemplateResolver
    {
        public string ResolveRouteTemplate(ActionDescriptor actionDescriptor)
        {
            var routes = ConventionalRoutingSwaggerGenMiddleware.ROUTES;
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
                            foreach (var segment in route.ParsedTemplate.Segments)
                            {
                                var firstPart = segment.Parts.First();
                                if (firstPart.Name.Equals("controller"))
                                {
                                    template += $"{WithNoSuffix(actionMatchConfig.Controller, "Controller")}/";
                                }
                                else if (firstPart.Name.Equals("action"))
                                {
                                    template += $"{actionMatchConfig.Action}/";
                                }
                                else if(firstPart.IsParameter)
                                {
                                    if (HasActionDescriptorParameter(actionDescriptor, firstPart.Name))
                                    {
                                        template += $"{{{firstPart.Name}{(firstPart.IsOptional ? "?" : string.Empty)}}}/";
                                    }
                                    else if (!firstPart.IsOptional)
                                    {
                                        template = null;
                                        break;
                                    }
                                }
                                
                            }
                        }

                    }
                }
            }

            if (!string.IsNullOrEmpty(template))
            {
                template = template.TrimEnd('/').ToLower();
            }

            return template;
        }

        private bool ControllersActionsMatch(List<Dictionary<string, string>> routeValues)
        {
            var match = false;

            

            return match;
        }

        private string GetRouteController(Route route, out bool isParameter)
        {
            var controller = string.Empty;

            isParameter = route.ParsedTemplate.Parameters.Any(p => p.Name.Equals("controller"));

            if(route.Defaults.TryGetValue("controller", out var controllerObj))
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

    public interface  IRouteTemplateResolver
    {
        string ResolveRouteTemplate(ActionDescriptor actionDescriptor);
    }
}
