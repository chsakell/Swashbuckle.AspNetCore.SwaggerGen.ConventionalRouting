using Microsoft.AspNetCore.Mvc;

namespace PetStore.API.Controllers
{
    // Test regex in swagger gen
    public class HelloWorldController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromRoute] string message)
        {
            return Ok(message ?? "Hello World!");
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
        public IActionResult Index([FromRoute] string message)
        {
            return Ok(message ?? "Hello Friend!");
        }

        [HttpGet]
        public IActionResult About([FromRoute] string id)
        {
            return Ok($"Hello Friend! {id}");
        }

        [HttpGet]
        public IActionResult NotAbout()
        {
            return Ok("Hello Friend!");
        }

        [HttpGet]
        public IActionResult Test(int id, string message)
        {
            return Ok($"id: {id} - message: {message}");
        }
    }
}
