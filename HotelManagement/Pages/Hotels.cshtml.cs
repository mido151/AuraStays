using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Models;
using HotelManagement.Services;

namespace HotelManagement.Pages
{
    public class HotelsModel : PageModel
    {
        private readonly IHotelService _hotelService;

        public HotelsModel(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public List<HotelEntity> Hotels { get; set; } = new List<HotelEntity>();

        public async Task OnGetAsync()
        {
            Hotels = await _hotelService.GetAllHotelsAsync();
        }
    }
}