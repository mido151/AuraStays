using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;

namespace HotelManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;

        public IndexModel(IHotelService hotelService, IRoomService roomService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
        }

        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Redirect authenticated users to their appropriate dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                else if (User.IsInRole("HotelAdmin"))
                {
                    return RedirectToPage("/HotelAdmin/Dashboard");
                }
                // Customer stays on Index page (home/landing page)
            }

            // Load data for guest/customer home page
            var hotels = await _hotelService.GetAllHotelsAsync();
            var rooms = await _roomService.GetAllRoomsAsync();

            TotalHotels = hotels.Count;
            TotalRooms = rooms.Count;
            TotalRevenue = rooms.Sum(r => r.RoomPrice);

            return Page();
        }
    }
}