using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.API.Models;

namespace Web.API.Controllers
{
    [ApiController]
    public class PetApiController : ControllerBase
    {
        [HttpPost]
        [Route("/pet")]
        [SwaggerOperation("AddPet")]
        public virtual IActionResult AddPet([FromBody] Pet body)
        {
            return Ok("Add pet");
        }

        [HttpDelete]
        [Route("/pet/{petId}")]
        [SwaggerOperation("DeletePet")]
        public virtual IActionResult DeletePet([FromRoute][Required] long? petId, [FromHeader] string apiKey)
        {
            return Ok("Delete Pet");
        }

        [HttpGet]
        [Route("/pet/findByStatus")]
        [SwaggerOperation("FindPetsByStatus")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Pet>), description: "successful operation")]
        public virtual IActionResult FindPetsByStatus([FromQuery][Required()] List<string> status)
        {
            return Ok("Find pets by status");
        }

        [HttpGet]
        [Route("/pet/findByTags")]
        [SwaggerOperation("FindPetsByTags")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Pet>), description: "successful operation")]
        public virtual IActionResult FindPetsByTags([FromQuery][Required()] List<string> tags)
        {
            return Ok("Find pets by tags");
        }

        [HttpGet]
        [Route("/pet/{petId}")]
        [Authorize]
        [SwaggerOperation("GetPetById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Pet), description: "successful operation")]
        public virtual IActionResult GetPetById([FromRoute][Required] long? petId)
        {
            return Ok("Bet pet by id");
        }

        [HttpPut]
        [Route("/pet")]
        [SwaggerOperation("UpdatePet")]
        public virtual IActionResult UpdatePet([FromBody] Pet body)
        {
            return Ok("Update pet");
        }

        [HttpPost]
        [Route("/pet/{petId}")]
        [SwaggerOperation("UpdatePetWithForm")]
        public virtual IActionResult UpdatePetWithForm([FromRoute][Required] long? petId, [FromForm] string name, [FromForm] string status)
        {
            return Ok("Update pet with form");
        }

        [HttpPost]
        [Route("/pet/{petId}/uploadImage")]
        [SwaggerOperation("UploadFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponse), description: "successful operation")]
        public virtual IActionResult UploadFile([FromRoute][Required] long? petId, [FromForm] string additionalMetadata, [FromForm] System.IO.Stream _file)
        {
            return Ok("Upload file");
        }
    }
}