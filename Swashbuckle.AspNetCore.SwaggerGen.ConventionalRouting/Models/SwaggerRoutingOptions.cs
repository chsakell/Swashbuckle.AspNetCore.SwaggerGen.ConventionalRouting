using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models
{
    public class SwaggerRoutingOptions
    {
        public Func<string, bool> IgnoreTemplateFunc { get; set; }
    }
}
