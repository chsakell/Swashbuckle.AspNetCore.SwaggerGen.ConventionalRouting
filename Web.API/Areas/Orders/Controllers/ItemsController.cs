using Microsoft.AspNetCore.Mvc;

namespace Web.API.Areas.Orders.Controllers
{
    [Area("Orders")]
    public class ItemsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Orders Area Index");
        }

        [HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
            return Ok($"Orders Area Get Item {id}");
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok("Orders Area Items List");
        }
    }
}
