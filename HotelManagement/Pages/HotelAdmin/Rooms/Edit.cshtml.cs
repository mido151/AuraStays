using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HotelManagement.Pages.HotelAdmin.Rooms
{
    [Authorize(Roles = "HotelAdmin")]
    public class EditModel : PageModel
    {
        private readonly IRoomService _roomService;

        public EditModel(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public int RoomId { get; set; }

            [Required(ErrorMessage = "Room number is required")]
            [MaxLength(20)]
            public string RoomNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Room type is required")]
            [MaxLength(50)]
            public string RoomType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, 100000)]
            public decimal RoomPrice { get; set; }

            [Range(1, 20)]
            public int Capacity { get; set; }

            [Range(1, 1000)]
            public decimal? Size { get; set; }

            [MaxLength(500)]
            public string? ImageUrl { get; set; }

            [Required]
            [MaxLength(20)]
            public string RoomStatus { get; set; } = "Available";
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
                return RedirectToPage("./Index");

            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            int hotelId = int.Parse(hotelIdClaim.Value);

            var room = await _roomService.GetRoomByIdAsync(id.Value);
            if (room == null || room.HotelId != hotelId)
                return NotFound();

            Input = new InputModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                RoomPrice = room.RoomPrice,
                Capacity = room.Capacity ?? 2,
                Size = room.Size,
                ImageUrl = room.ImageUrl,
                RoomStatus = room.RoomStatus
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            int hotelId = int.Parse(hotelIdClaim.Value);

            if (!ModelState.IsValid)
                return Page();

            try
            {
                var room = await _roomService.GetRoomByIdAsync(Input.RoomId);
                if (room == null || room.HotelId != hotelId)
                    return Forbid();

                room.RoomNumber = Input.RoomNumber;
                room.RoomType = Input.RoomType;
                room.RoomPrice = Input.RoomPrice;
                room.Capacity = Input.Capacity;
                room.Size = Input.Size;
                room.ImageUrl = Input.ImageUrl;
                room.RoomStatus = Input.RoomStatus;

                var success = await _roomService.UpdateRoomAsync(room);
                if (success)
                {
                    TempData["SuccessMessage"] = "Room updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = "Failed to update room. Please try again.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating room: {ex.Message}";
                return Page();
            }
        }
    }
}