﻿#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NET5_0 || NET6_0)

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting
{
    public interface IConventionalRoutingApiDescriptionGroupCollectionProvider 
        : IApiDescriptionGroupCollectionProvider { }

    public class ConventionalRoutingApiDescriptionGroupCollectionProvider : ApiDescriptionGroupCollectionProvider, IConventionalRoutingApiDescriptionGroupCollectionProvider
    {
        public ConventionalRoutingApiDescriptionGroupCollectionProvider
        (IConventionalRoutingActionDescriptorCollectionProvider actionDescriptorCollectionProvider, 
            IEnumerable<IConventionalRoutingApiDescriptionProvider> apiDescriptionProviders) : 
            base(actionDescriptorCollectionProvider, apiDescriptionProviders)
        {
        }
    }
}

#endif