#if (NETCOREAPP3_0 || NETCOREAPP3_1)

using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Internals
{
    internal class ApiDescriptionActionData
    {
        /// <summary>
        /// The <c>ApiDescription.GroupName</c> of <c>ApiDescription</c> objects for the associated
        /// action.
        /// </summary>
        public string GroupName { get; set; }
    }
}

#endif