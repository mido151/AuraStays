using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using System.Security.Claims;

namespace HotelManagement.Pages.HotelAdmin
{
    [Authorize(Roles = "HotelAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly IRoomService _roomService;
        private readonly IReservationService _reservationService;
        private readonly IStaffService _staffService;
        private readonly IHotelService _hotelService;

        public DashboardModel(
            IRoomService roomService,
            IReservationService reservationService,
            IStaffService staffService,
            IHotelService hotelService)
        {
            _roomService = roomService;
            _reservationService = reservationService;
            _staffService = staffService;
            _hotelService = hotelService;
        }

        public HotelEntity? Hotel { get; set; }
        public List<CurrentGuestViewModel> CurrentGuests { get; set; } = new();
        public List<RoomEntity> Rooms { get; set; } = new();
        public List<StaffEntity> Staff { get; set; } = new();
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TodayCheckIns { get; set; }
        public int TodayCheckOuts { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
            {
                ErrorMessage = "Hotel assignment not found. Please contact administrator.";
                return Page();
            }

            int hotelId = int.Parse(hotelIdClaim.Value);

            try
            {
                // Load hotel details
                Hotel = await _hotelService.GetHotelByIdAsync(hotelId);

                // Load rooms
                Rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);
                TotalRooms = Rooms.Count;
                OccupiedRooms = Rooms.Count(r => r.RoomStatus == "Occupied");
                AvailableRooms = Rooms.Count(r => r.RoomStatus == "Available");

                // Load current guests
                var activeReservations = await _reservationService.GetActiveReservationsByHotelAsync(hotelId);
                var today = DateTime.Today;

                CurrentGuests = activeReservations
                    .Where(r => r.CheckInDate <= today && r.CheckOutDate > today)
                    .SelectMany(r => r.Rooms
                        .Where(rm => rm.HotelId == hotelId)
                        .Select(rm => new CurrentGuestViewModel
                        {
                            ReservationId = r.ReservationId,
                            GuestName = $"{r.Guest.FirstName} {r.Guest.LastName}",
                            RoomNumber = rm.RoomNumber,
                            CheckInDate = r.CheckInDate,
                            CheckOutDate = r.CheckOutDate,
                            RemainingDays = (r.CheckOutDate - today).Days,
                            TotalNights = (r.CheckOutDate - r.CheckInDate).Days
                        }))
                    .OrderBy(g => g.RemainingDays)
                    .ToList();

                // Calculate today's check-ins and check-outs
                TodayCheckIns = activeReservations.Count(r => r.CheckInDate == today);
                TodayCheckOuts = activeReservations.Count(r => r.CheckOutDate == today);

                // Load staff
                Staff = await _staffService.GetStaffByHotelIdAsync(hotelId);

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dashboard: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCheckOutAsync(int reservationId)
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            try
            {
                var success = await _reservationService.CheckOutReservationAsync(reservationId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Guest checked out successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to check out guest. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error during check-out: {ex.Message}";
            }

            return RedirectToPage();
        }
    }

    public class CurrentGuestViewModel
    {
        public int ReservationId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RemainingDays { get; set; }
        public int TotalNights { get; set; }
    }
}