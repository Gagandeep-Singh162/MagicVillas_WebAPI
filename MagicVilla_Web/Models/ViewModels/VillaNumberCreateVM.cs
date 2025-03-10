using MagicVilla_Web.Models.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModels
{
    public class VillaNumberCreateVM
    {
        public VillaNumberCreateDto VillaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> villaList { get; set; }

        public VillaNumberCreateVM()
        {
            VillaNumber = new VillaNumberCreateDto();
        }
    }
}
