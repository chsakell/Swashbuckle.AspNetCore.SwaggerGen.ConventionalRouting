using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public class ConventionalRoutingSwaggerGenMiddleware
    {
        private readonly RequestDelegate _next;
        public static List<IRouter> ROUTES;

        public ConventionalRoutingSwaggerGenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            await _next(context);
        }

        public static void UseRoutes(List<IRouter> routes)
        {
            ROUTES = routes;
        }
    }
}
