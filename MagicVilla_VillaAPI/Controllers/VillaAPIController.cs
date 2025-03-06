using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VillaAPIController : Controller
    {
        [HttpGet]
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa>
            {
                new Villa {Id = 1, Name ="Pool" },
                new Villa {Id = 2, Name ="Beach" }
            };
        }  
    }
}
