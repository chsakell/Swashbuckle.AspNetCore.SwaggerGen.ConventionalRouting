using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models
{
    public class MatchConfig
    {
        public string Controller { get; set; }
        public bool IsControllerParameter { get; set; }
        public string Action { get; set; }
        public bool IsActionParameter { get; set; }

        public MatchConfig(string controller, string action)
        {
            Controller = controller;
            Action = action;
        }

        public static bool Match(MatchConfig config1, MatchConfig config2)
        {
            return (
                (config1.Controller.Equals(config2.Controller) ||
                 config1.IsControllerParameter && !string.IsNullOrEmpty(config2.Controller))
                &&
                (config1.Action.Equals(config2.Action) ||
                 config1.IsActionParameter && !string.IsNullOrEmpty(config2.Action))
                );
        }
    }
}
