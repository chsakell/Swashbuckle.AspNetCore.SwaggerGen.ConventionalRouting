using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerConfigAttribute : Attribute
    {
        public bool IgnoreApi { get; set; }
    }
}
