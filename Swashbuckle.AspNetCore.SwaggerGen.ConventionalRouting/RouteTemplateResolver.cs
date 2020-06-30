using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

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
                        foreach (var parsedTemplateParameter in route.ParsedTemplate.Parameters)
                        {
                            actionDescriptor.RouteValues.TryGetValue(parsedTemplateParameter.Name, out var routeValue);
                            if (!string.IsNullOrEmpty(routeValue))
                            {
                                template += $"{routeValue}/";
                            }
                            else if (!parsedTemplateParameter.IsOptional)
                            {
                                template = null;
                                break;
                            }
                            else
                            {
                                var actionDescriptionParameter =
                                    actionDescriptor.Parameters.FirstOrDefault(param =>
                                        param.Name.Equals(parsedTemplateParameter.Name));
                                if (actionDescriptionParameter != null)
                                {
                                    template += $"{{{actionDescriptionParameter.Name}}}/";
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
    }

    public interface  IRouteTemplateResolver
    {
        string ResolveRouteTemplate(ActionDescriptor actionDescriptor);
    }
}
