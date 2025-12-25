using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using System.Security.Claims;

namespace HotelManagement.Pages.HotelAdmin.Rooms
{
    [Authorize(Roles = "HotelAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IRoomService _roomService;
        private readonly IHotelService _hotelService;

        public IndexModel(IRoomService roomService, IHotelService hotelService)
        {
            _roomService = roomService;
            _hotelService = hotelService;
        }

        public List<RoomEntity> Rooms { get; set; } = new();
        public HotelEntity? Hotel { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
            {
                ErrorMessage = "Hotel assignment not found.";
                return Page();
            }

            int hotelId = int.Parse(hotelIdClaim.Value);

            try
            {
                Hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                Rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading rooms: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int roomId)
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            try
            {
                // Verify room belongs to this hotel
                var room = await _roomService.GetRoomByIdAsync(roomId);
                if (room == null || room.HotelId != int.Parse(hotelIdClaim.Value))
                    return Forbid();

                var success = await _roomService.DeleteRoomAsync(roomId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Room deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot delete room. It may be currently reserved.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting room: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}