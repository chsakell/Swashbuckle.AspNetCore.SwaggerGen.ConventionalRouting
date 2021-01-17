#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0)

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public static class ConventionalRoutingSwaggerGen
    {
        public static List<RoutePattern> ROUTES;

        public static IServiceCollection AddSwaggerGenWithConventionalRoutes(
            this IServiceCollection services, Action<SwaggerRoutingOptions> options = null)
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

            if (options != null)
            {
                services.Configure(options);
            }

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