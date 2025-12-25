using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;
using System.Security.Claims;

namespace HotelManagement.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly HotelDbContext _context;

        public ProfileModel(HotelDbContext context)
        {
            _context = context;
        }

        public User? CurrentUser { get; set; }
        public GuestEntity? GuestInfo { get; set; }
        public string? HotelName { get; set; }
        public int TotalReservations { get; set; }
        public int CompletedReservations { get; set; }
        public int PendingReservations { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            CurrentUser = await _context.Users
                .Include(u => u.Hotel)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (CurrentUser == null)
                return RedirectToPage("/Account/Login");

            // Get guest info if customer
            if (CurrentUser.Role == "Customer")
            {
                GuestInfo = await _context.Guests
                    .FirstOrDefaultAsync(g => g.UserId == userId);

                if (GuestInfo != null)
                {
                    var reservations = await _context.Reservations
                        .Where(r => r.GuestId == GuestInfo.GuestId)
                        .ToListAsync();

                    TotalReservations = reservations.Count;
                    CompletedReservations = reservations.Count(r => r.Status == "Completed");
                    PendingReservations = reservations.Count(r => r.Status == "Confirmed");
                }
            }

            // Get hotel name if hotel admin
            if (CurrentUser.Role == "HotelAdmin" && CurrentUser.HotelId.HasValue)
            {
                var hotel = await _context.Hotels.FindAsync(CurrentUser.HotelId.Value);
                HotelName = hotel?.Name ?? "Unknown Hotel";
            }

            return Page();
        }
    }
}