using Microsoft.AspNetCore.Mvc;
using Web.API.Models;

namespace Web.API.Controllers
{
    public class DefaultHttpMethodsController : Controller
    {
        public IActionResult Get()
        {
            return Ok("$ GET action");
        }

        public IActionResult Post([FromBody] Tag tag)
        {
            return Ok("$ GET action");
        }

        public IActionResult Put([FromRoute] int id, [FromBody] Tag tag)
        {
            return Ok("$ Put action");
        }

        public IActionResult UpdateTag(int id, [FromBody] Tag tag)
        {
            return Ok("$ Update action");
        }

        public IActionResult Patch([FromBody] Tag tag)
        {
            return Ok("$ Patch action");
        }

        public IActionResult Delete(int id)
        {
            return Ok("$ Delete action");
        }

        public IActionResult Remove([FromRoute] int id)
        {
            return Ok("$ Remove action");
        }
    }
}
