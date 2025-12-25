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
    public class CreateModel : PageModel
    {
        private readonly IRoomService _roomService;
        private readonly IHotelService _hotelService;

        public CreateModel(IRoomService roomService, IHotelService hotelService)
        {
            _roomService = roomService;
            _hotelService = hotelService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public HotelEntity? Hotel { get; set; }
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Room number is required")]
            [MaxLength(20)]
            [Display(Name = "Room Number")]
            public string RoomNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Room type is required")]
            [MaxLength(50)]
            [Display(Name = "Room Type")]
            public string RoomType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Price is required")]
            [Range(0.01, 100000, ErrorMessage = "Price must be between $0.01 and $100,000")]
            [Display(Name = "Price per Night")]
            public decimal RoomPrice { get; set; }

            [Range(1, 20, ErrorMessage = "Capacity must be between 1 and 20")]
            [Display(Name = "Guest Capacity")]
            public int Capacity { get; set; } = 2;

            [Range(1, 1000, ErrorMessage = "Size must be between 1 and 1000 m²")]
            [Display(Name = "Size (m²)")]
            public decimal? Size { get; set; }

            [MaxLength(500)]
            [Display(Name = "Image URL (Optional)")]
            public string? ImageUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
            {
                ErrorMessage = "Hotel assignment not found.";
                return Page();
            }

            int hotelId = int.Parse(hotelIdClaim.Value);
            Hotel = await _hotelService.GetHotelByIdAsync(hotelId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            int hotelId = int.Parse(hotelIdClaim.Value);

            if (!ModelState.IsValid)
            {
                Hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                return Page();
            }

            try
            {
                var room = new RoomEntity
                {
                    RoomNumber = Input.RoomNumber,
                    RoomType = Input.RoomType,
                    RoomPrice = Input.RoomPrice,
                    Capacity = Input.Capacity,
                    Size = Input.Size,
                    ImageUrl = Input.ImageUrl,
                    RoomStatus = "Available",
                    HotelId = hotelId
                };

                await _roomService.CreateRoomAsync(room);
                TempData["SuccessMessage"] = "Room created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating room: {ex.Message}";
                Hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                return Page();
            }
        }
    }
}