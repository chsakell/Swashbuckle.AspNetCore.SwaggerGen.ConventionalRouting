﻿#if NETCOREAPP2_2

using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public interface IConventionalRoutingSwaggerProvider : ISwaggerProvider
    {

    }

    public class ConventionalRoutingSwaggerGenerator : SwaggerGenerator, IConventionalRoutingSwaggerProvider
    {
        public ConventionalRoutingSwaggerGenerator(SwaggerGeneratorOptions options, 
            IConventionalRoutingApiDescriptionGroupCollectionProvider apiDescriptionsProvider, 
            ISchemaGenerator schemaGenerator) : base(options, apiDescriptionsProvider, schemaGenerator)
        {
            if (options.TagsSelector.Method.Name.Equals("DefaultTagsSelector",
                StringComparison.InvariantCultureIgnoreCase))
            {
                options.TagsSelector = (apiDesc) =>
                {
                    if (apiDesc.GroupName != null)
                    {
                        return new[] {apiDesc.GroupName};
                    }

                    var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;

                    try
                    {
                        if (controllerActionDescriptor.RouteValues.TryGetValue("area", out string area) &&
                            !string.IsNullOrEmpty(area))
                        {
                            return new[] {$"{area}.{controllerActionDescriptor.ControllerName}"};
                        }

                        return new[] {controllerActionDescriptor.ControllerName};
                    }
                    catch (Exception ex)
                    {
                        return options.TagsSelector.Invoke(apiDesc);
                    }
                };
            }
        }
    }
}

#endif