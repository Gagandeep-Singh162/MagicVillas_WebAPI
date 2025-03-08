using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.log.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController : ControllerBase
    {
        private readonly Ilogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;

        public VillaAPIController(Ilogging logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _dbVilla = dbVilla;
        }

        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.Log("Getting all villas", "");
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            return Ok(_mapper.Map<List<VillaDto>>(villaList));
        }

        [HttpGet("id", Name = "GetVilla")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto?>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.Log("Getting villa error with Id " + id, "error");
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id);


            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto createDto)
        {
            //if (ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa model = _mapper.Map<Villa>(createDto);

            await _dbVilla.CreateAsync(model);

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            await _dbVilla.RemoveAsync(villa);
            return Ok();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDto);

            await _dbVilla.UpdateAsync(model);
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
            if (villa == null)
            {
                return BadRequest();
            }

            VillaUpdateDto villaDTO = _mapper.Map<VillaUpdateDto>(villa);

            patchDto.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}