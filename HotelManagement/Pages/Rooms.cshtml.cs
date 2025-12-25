using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Models;
using HotelManagement.Services;

namespace HotelManagement.Pages
{
    public class RoomsModel : PageModel
    {
        private readonly IRoomService _roomService;

        public RoomsModel(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public List<RoomEntity> Rooms { get; set; } = new List<RoomEntity>();

        public async Task OnGetAsync()
        {
            Rooms = await _roomService.GetAllRoomsAsync();
        }
    }
}