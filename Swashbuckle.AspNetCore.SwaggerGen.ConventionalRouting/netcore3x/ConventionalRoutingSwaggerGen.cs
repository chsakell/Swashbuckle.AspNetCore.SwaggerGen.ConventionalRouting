#if (NETCOREAPP3_0 || NETCOREAPP3_1)

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public static class ConventionalRoutingSwaggerGen
    {
        public static List<RoutePattern> ROUTES;

        public static IServiceCollection AddSwaggerGenWithConventionalRoutes(
            this IServiceCollection services)
        {
            services.AddSingleton<IRouteTemplateResolver, RouteTemplateResolver>();
            services.AddSingleton<IConventionalRoutingSwaggerProvider, ConventionalRoutingSwaggerGenerator>();
            services.AddSingleton<IConventionalRoutingApiDescriptionGroupCollectionProvider,
                ConventionalRoutingApiDescriptionGroupCollectionProvider>();
            services
                .AddSingleton<IConventionalRoutingApiDescriptionProvider, ConventionalRoutingApiDescriptionProvider>();
            services
                .AddSingleton<IConventionalRoutingActionDescriptorCollectionProvider,
                    ConventionalRoutingActionDescriptorCollectionProvider>();

            var swaggerDescriptionProviderDescriptor =
                new ServiceDescriptor(
                    typeof(ISwaggerProvider),
                    typeof(ConventionalRoutingSwaggerGenerator),
                    ServiceLifetime.Singleton);

            services.Replace(swaggerDescriptionProviderDescriptor);

            return services;
        }

        public static void UseRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            ROUTES = new List<RoutePattern>();
            foreach (var endpointDataSource in endpointRouteBuilder.DataSources)
            {
                var conventionalRouteEndpoints = endpointDataSource.Endpoints.Where(e => e.DisplayName.StartsWith("Route:"));
                foreach (var conventionalRouteEndpoint in conventionalRouteEndpoints)
                {
                  var routePatternProp =  typeof(RouteEndpoint).GetProperty("RoutePattern")?.GetValue(conventionalRouteEndpoint);
                  if (routePatternProp is RoutePattern routePattern)
                  {
                      ROUTES.Add(routePattern);
                  }
                }
                
            }
        }
    }
}

#endif