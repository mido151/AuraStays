using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;
using HotelManagement.Services;
using System.Security.Claims;

namespace HotelManagement.Pages.Customer
{
    [Authorize(Roles = "Customer")]
    public class MyReservationsModel : PageModel
    {
        private readonly HotelDbContext _context;
        private readonly IReservationService _reservationService;

        public MyReservationsModel(HotelDbContext context, IReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }

        public List<Reservation> Reservations { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);

            if (guest == null)
            {
                ErrorMessage = "Guest profile not found.";
                return Page();
            }

            try
            {
                Reservations = await _reservationService.GetReservationsByGuestIdAsync(guest.GuestId);

                if (!string.IsNullOrEmpty((string?)TempData["SuccessMessage"]))
                {
                    SuccessMessage = (string?)TempData["SuccessMessage"];
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading reservations: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int reservationId)
        {
            try
            {
                // Verify reservation belongs to current user
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);

                if (guest == null)
                    return Forbid();

                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId && r.GuestId == guest.GuestId);

                if (reservation == null)
                    return Forbid();

                var success = await _reservationService.CancelReservationAsync(reservationId);
                if (success)
                {
                    TempData["SuccessMessage"] = "Reservation cancelled successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to cancel reservation.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling reservation: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}