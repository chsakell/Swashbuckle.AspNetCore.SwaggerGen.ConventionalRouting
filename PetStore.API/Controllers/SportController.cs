using Microsoft.AspNetCore.Mvc;

namespace PetStore.API.Controllers
{
    public class SportController : Controller
    {
        [HttpGet]
        public IActionResult Details(int eventId)
        {
            return Ok($"Event {eventId}");
        }

        [HttpGet]
        public IActionResult UpcomingEvents(string sport, bool? liveOnly)
        {
            return Ok($"sport: {sport} liveOnly: {liveOnly}");
        }
    }
}
