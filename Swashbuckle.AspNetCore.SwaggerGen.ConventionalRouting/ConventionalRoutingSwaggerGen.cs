using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public static class ConventionalRoutingSwaggerGen
    {
        public static List<IRouter> ROUTES;

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

        public static void UseRoutes(List<IRouter> routes)
        {
            ROUTES = routes;
        }
    }
}
