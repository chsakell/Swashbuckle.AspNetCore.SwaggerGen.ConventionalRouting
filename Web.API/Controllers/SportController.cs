using Microsoft.AspNetCore.Mvc;

namespace PetStore.API.Controllers
{
    public class SportController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok($"Sport index");
        }

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

        [HttpGet]
        public IActionResult GetEvents([FromQuery(Name = "event_ids")] string eventIds)
        {
            return Ok($"events: {eventIds}");
        }

        [HttpGet]
        public IActionResult Live()
        {
            return Ok($"Live sports");
        }
    }
}
