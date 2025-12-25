using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;

namespace HotelManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly HotelDbContext _context;

        public DashboardModel(HotelDbContext context)
        {
            _context = context;
        }

        public int TotalHotels { get; set; }
        public int TotalRooms { get; set; }
        public int TotalReservations { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingReviews { get; set; }

        public async Task OnGetAsync()
        {
            TotalHotels = await _context.Hotels.CountAsync();
            TotalRooms = await _context.Rooms.CountAsync();
            TotalReservations = await _context.Reservations.CountAsync();
            TotalUsers = await _context.Users.CountAsync();

            // Calculate total revenue from payments
            var payments = await _context.Payments.ToListAsync();
            TotalRevenue = payments.Where(p => p.Method != "Pending").Sum(p => p.Amount);

            PendingReviews = await _context.Reviews
                .CountAsync(r => !r.IsApproved);
        }
    }
}