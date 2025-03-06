using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(VillaStore.villaList); 
        }

        [HttpGet("id", Name = "GetVilla")]
        [ProducesResponseType(statusCode:StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode:StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto?> GetVilla(int id)
        {
            if (id == 0) {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);


            if (villa == null) { 
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
        {
            //if (ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists");
                return BadRequest(ModelState);
            }

            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }

            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDto);

            return CreatedAtRoute("GetVilla", new { id = villaDto.Id}, villaDto);
        }
    }
}
