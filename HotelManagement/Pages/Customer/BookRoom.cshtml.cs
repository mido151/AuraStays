using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HotelManagement.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class BookRoomModel : PageModel
    {
        private readonly HotelDbContext _context;
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IHotelService _hotelService;

        public BookRoomModel(
            HotelDbContext context,
            IRoomService roomService,
            IReservationService reservationService,
            IHotelService hotelService)
        {
            _context = context;
            _roomService = roomService;
            _reservationService = reservationService;
            _hotelService = hotelService;
        }

        [BindProperty]
        public BookingInputModel Input { get; set; } = new();

        public List<HotelEntity> Hotels { get; set; } = new();
        public List<RoomEntity> AvailableRooms { get; set; } = new();
        public HotelEntity? SelectedHotel { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool ShowResults { get; set; }

        public class BookingInputModel
        {
            [Required(ErrorMessage = "Please select a hotel")]
            public int HotelId { get; set; }

            [Required(ErrorMessage = "Check-in date is required")]
            [Display(Name = "Check-In Date")]
            public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

            [Required(ErrorMessage = "Check-out date is required")]
            [Display(Name = "Check-Out Date")]
            public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);

            [Required(ErrorMessage = "Number of guests is required")]
            [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20")]
            [Display(Name = "Number of Guests")]
            public int NumGuests { get; set; } = 2;

            public List<int> SelectedRoomIds { get; set; } = new();
        }

        public async Task OnGetAsync()
        {
            Hotels = await _hotelService.GetAllHotelsAsync();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            Hotels = await _hotelService.GetAllHotelsAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.CheckOutDate <= Input.CheckInDate)
            {
                ErrorMessage = "Check-out date must be after check-in date.";
                return Page();
            }

            if (Input.CheckInDate < DateTime.Today)
            {
                ErrorMessage = "Check-in date cannot be in the past.";
                return Page();
            }

            try
            {
                AvailableRooms = await _roomService.GetAvailableRoomsAsync(
                    Input.HotelId,
                    Input.CheckInDate,
                    Input.CheckOutDate);

                SelectedHotel = await _hotelService.GetHotelByIdAsync(Input.HotelId);
                ShowResults = true;

                if (!AvailableRooms.Any())
                {
                    ErrorMessage = "No rooms available for the selected dates.";
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error searching rooms: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostBookAsync()
        {
            if (Input.SelectedRoomIds == null || !Input.SelectedRoomIds.Any())
            {
                ErrorMessage = "Please select at least one room.";
                Hotels = await _hotelService.GetAllHotelsAsync();
                return await OnPostSearchAsync();
            }

            try
            {
                // Get current user's guest ID
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);

                if (guest == null)
                {
                    ErrorMessage = "Guest profile not found. Please contact support.";
                    Hotels = await _hotelService.GetAllHotelsAsync();
                    return Page();
                }

                // Validate dates again
                if (Input.CheckOutDate <= Input.CheckInDate)
                {
                    ErrorMessage = "Check-out date must be after check-in date.";
                    Hotels = await _hotelService.GetAllHotelsAsync();
                    return await OnPostSearchAsync();
                }

                // Create reservation
                var reservation = new Reservation
                {
                    GuestId = guest.GuestId,
                    CheckInDate = Input.CheckInDate,
                    CheckOutDate = Input.CheckOutDate,
                    NumGuests = Input.NumGuests,
                    Status = "Confirmed",
                    BookingDate = DateTime.Now,
                    CreatedByStaffId = null
                };

                var result = await _reservationService.CreateReservationAsync(
                    reservation,
                    Input.SelectedRoomIds);

                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Booking created successfully! Reservation #{result.ReservationId}";
                    return RedirectToPage("/Customer/MyReservations");
                }
                else
                {
                    ErrorMessage = "Failed to create booking. The selected rooms may no longer be available.";
                    Hotels = await _hotelService.GetAllHotelsAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating booking: {ex.Message}";
                Hotels = await _hotelService.GetAllHotelsAsync();
                return Page();
            }
        }
    }
}