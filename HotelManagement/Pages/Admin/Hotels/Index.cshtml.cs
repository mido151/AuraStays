using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;

namespace HotelManagement.Pages.Admin.Hotels
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHotelService _hotelService;

        public IndexModel(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public List<HotelEntity> Hotels { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Hotels = await _hotelService.GetAllHotelsAsync();

                if (!string.IsNullOrEmpty((string?)TempData["SuccessMessage"]))
                {
                    SuccessMessage = (string?)TempData["SuccessMessage"];
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading hotels: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int hotelId)
        {
            try
            {
                var success = await _hotelService.DeleteHotelAsync(hotelId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Hotel deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete hotel.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting hotel: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}