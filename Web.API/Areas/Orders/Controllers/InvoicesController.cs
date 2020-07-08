using Microsoft.AspNetCore.Mvc;

namespace Web.API.Areas.Orders.Controllers
{
    [Area("Orders")]
    public class InvoicesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Orders Area Invoices Index");
        }

        [HttpGet]
        public IActionResult Get([FromRoute] int id)
        {
            return Ok($"Orders Get order {id}");
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok("Orders Area List");
        }
    }
}
