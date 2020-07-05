using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Web.API.Models;

namespace Web.API.Controllers
{ 
    public class MyPetApiController : ControllerBase
    { 
        [HttpPost]
        public virtual IActionResult AddPet([FromBody]Pet body)
        {
            return Ok("Add pet");
        }

        [HttpDelete]
        //[Route("/v2/pet/{petId}")]
        [SwaggerOperation("DeletePet")]
        public virtual IActionResult DeletePet([FromRoute][Required]long? petId, [FromHeader]string apiKey)
        {
            return Ok("Delete pet");
        }

        [HttpGet]
        //[Route("/v2/pet/findByStatus")]
        [SwaggerOperation("FindPetsByStatus")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Pet>), description: "successful operation")]
        public virtual IActionResult FindPetsByStatus([FromQuery][Required()]List<string> status)
        {
            return Ok("Find pets by status");
        }

        [HttpGet]
        //[Route("/v2/pet/findByTags")]
        [SwaggerOperation("FindPetsByTags")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Pet>), description: "successful operation")]
        public virtual IActionResult FindPetsByTags([FromQuery][Required()]List<string> tags)
        {
            return Ok("Find pets by tags");
        }

        [HttpGet]
        //[Route("/v2/pet/{petId}")]
        [Authorize]
        [SwaggerOperation("GetPetById")]
        [SwaggerResponse(statusCode: 200, type: typeof(Pet), description: "successful operation")]
        public virtual IActionResult GetPetById([FromRoute][Required]long? petId)
        {
            return Ok("Get pet by id");
        }

        [HttpPut]
        //[Route("/v2/pet")]
        [SwaggerOperation("UpdatePet")]
        public virtual IActionResult UpdatePet([FromBody]Pet body)
        {
            return Ok("Update Pet");
        }

        [HttpPost]
        //[Route("/v2/pet/{petId}")]
        [SwaggerOperation("UpdatePetWithForm")]
        public virtual IActionResult UpdatePetWithForm([FromRoute][Required]long? petId, [FromForm]string name, [FromForm]string status)
        {
            return Ok("Update pet with form");
        }

        [HttpPost]
        //[Route("/v2/pet/{petId}/uploadImage")]
        [SwaggerOperation("UploadFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(ApiResponse), description: "successful operation")]
        public virtual IActionResult UploadFile([FromRoute][Required]long? petId, [FromForm]string additionalMetadata, [FromForm]System.IO.Stream _file)
        {
            return Ok("Upload file");
        }
    }
}
