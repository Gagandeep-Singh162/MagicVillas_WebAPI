using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModels
{
    public class VillaNumberUpdateVM
    {
        public VillaNumberUpdateDto VillaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> villaList { get; set; }

        public VillaNumberUpdateVM()
        {
            VillaNumber = new VillaNumberUpdateDto();
        }
    }
}
