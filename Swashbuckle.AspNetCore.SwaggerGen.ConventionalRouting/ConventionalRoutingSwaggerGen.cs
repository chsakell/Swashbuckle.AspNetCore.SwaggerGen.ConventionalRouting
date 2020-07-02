using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public static class ConventionalRoutingSwaggerGen
    {
        public static List<IRouter> ROUTES;

        public static IServiceCollection AddSwaggerGenWithConventionalRoutes(
            this IServiceCollection services)
        {
            var apiDescriptionProviderDescriptor =
                new ServiceDescriptor(
                    typeof(IApiDescriptionProvider),
                    typeof(ConventionalRoutingApiDescriptionProvider),
                    ServiceLifetime.Transient);

            var actionDescriptorCollectionProviderDescriptor =
                new ServiceDescriptor(
                    typeof(IActionDescriptorCollectionProvider),
                    typeof(ConventionalRoutingActionDescriptorCollectionProvider),
                    ServiceLifetime.Transient);

            services.Replace(apiDescriptionProviderDescriptor);
            services.Replace(actionDescriptorCollectionProviderDescriptor);

            services.AddSingleton<IRouteTemplateResolver, RouteTemplateResolver>();

            return services;
        }

        public static void UseRoutes(List<IRouter> routes)
        {
            ROUTES = routes;
        }
    }
}
