using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.API_3x_5x.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string id)
        {
            return Ok("$ Home index action");
        }

        public IActionResult Test()
        {
            return Ok("$ Home test action");
        }
    }
}
