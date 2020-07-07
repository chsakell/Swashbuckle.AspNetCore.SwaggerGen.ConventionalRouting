using System;
using System.Collections.Generic;
using System.Text;

namespace Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models
{
    public class MatchConfig
    {
        public string Area { get; set; }
        public bool IsAreaParameter { get; set; }

        public string Controller { get; set; }
        public bool IsControllerParameter { get; set; }

        public string Action { get; set; }
        public bool IsActionParameter { get; set; }

        public MatchConfig(string area, string controller, string action)
        {
            Area = area;
            Controller = controller;
            Action = action;
        }

        public static bool Match(MatchConfig config1, MatchConfig config2)
        {
            return (
                (config1.Area == config2.Area ||
                 config1.IsAreaParameter && !string.IsNullOrEmpty(config2.Area))
                &&
                (config1.Controller.Equals(config2.Controller) ||
                 config1.IsControllerParameter && !string.IsNullOrEmpty(config2.Controller))
                &&
                (config1.Action.Equals(config2.Action) ||
                 config1.IsActionParameter && !string.IsNullOrEmpty(config2.Action))
                );
        }
    }
}
