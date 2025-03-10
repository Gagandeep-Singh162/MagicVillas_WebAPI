using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModels
{
    public class VillaNumberDeleteVM
    {
        public VillaNumberDto VillaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> villaList { get; set; }

        public VillaNumberDeleteVM()
        {
            VillaNumber = new VillaNumberDto();
        }
    }
}
