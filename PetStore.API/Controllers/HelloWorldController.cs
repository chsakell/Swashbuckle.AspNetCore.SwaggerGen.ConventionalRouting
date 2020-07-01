using Microsoft.AspNetCore.Mvc;

namespace PetStore.API.Controllers
{
    // Test regex in swagger gen
    public class HelloWorldController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello World!");
        }

        [HttpGet]
        public IActionResult About()
        {
            return Ok("Hello World!");
        }
    }

    public class HelloFriendController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Hello Friend!");
        }

        [HttpGet]
        public IActionResult NotAbout()
        {
            return Ok("Hello Friend!");
        }
    }
}
