using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen.ConventionalRouting.Models;

namespace PetStore.API.Controllers
{
    // Test regex in swagger gen
    public class HelloWorldController : Controller
    {
        [HttpGet]
        [SwaggerConfig(IgnoreApi = false)]
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

        [HttpGet]
        public IActionResult Test2([FromRoute] DateTime date, string message)
        {
            return Ok($"date: {date} - message: {message}");
        }

        [HttpGet]
        public IActionResult Test3(decimal num1, long num2, Guid token)
        {
            return Ok($"num1: {num1} - num2: {num2} token: {token}");
        }
    }
}
