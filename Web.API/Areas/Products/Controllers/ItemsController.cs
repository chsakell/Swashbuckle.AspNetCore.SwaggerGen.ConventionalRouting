using Microsoft.AspNetCore.Mvc;

namespace Web.API.Areas.Products.Controllers
{
    [Area("Products")]
    public class ItemsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Products Area Index");
        }

        [HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
            return Ok($"Products Get Item {id}");
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok("Products Area List");
        }
    }
}
