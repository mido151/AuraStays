using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Models;
using HotelManagement.Services;

namespace HotelManagement.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly IHotelService _hotelService;

        public DetailsModel(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public HotelEntity? Hotel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToPage("/Hotels");
            }

            Hotel = await _hotelService.GetHotelByIdAsync(id.Value);

            if (Hotel == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}