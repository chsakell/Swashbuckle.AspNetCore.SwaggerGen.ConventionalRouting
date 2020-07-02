using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PetStore.API
{
    public class GenericMatchRouteConstraint : IRouteConstraint
    {
        private readonly IEnumerable<string> _matches;

        public GenericMatchRouteConstraint(IEnumerable<string> matches)
        {
            _matches = matches;
        }
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            values.TryGetValue(routeKey, out var routeValue);

            if (routeValue == null || routeValue is string value && string.IsNullOrEmpty(value))
            {
                return false;
            }

            return _matches.Any(match =>
                match.Equals(routeValue.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase));

        }
    }
}
