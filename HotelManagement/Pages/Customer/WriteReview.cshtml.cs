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
    public class WriteReviewModel : PageModel
    {
        private readonly HotelDbContext _context;
        private readonly IReviewService _reviewService;
        private readonly IHotelService _hotelService;

        public WriteReviewModel(
            HotelDbContext context,
            IReviewService reviewService,
            IHotelService hotelService)
        {
            _context = context;
            _reviewService = reviewService;
            _hotelService = hotelService;
        }

        [BindProperty]
        public ReviewInputModel Input { get; set; } = new();

        public List<HotelEntity> EligibleHotels { get; set; } = new();
        public HotelEntity? SelectedHotel { get; set; }
        public string? ErrorMessage { get; set; }

        public class ReviewInputModel
        {
            [Required(ErrorMessage = "Please select a hotel")]
            public int HotelId { get; set; }

            [Required(ErrorMessage = "Rating is required")]
            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
            public int Rating { get; set; }

            [Required(ErrorMessage = "Comment is required")]
            [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
            [MinLength(10, ErrorMessage = "Comment must be at least 10 characters")]
            public string Comment { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int? hotelId)
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
                // Get hotels where guest has completed stays
                var completedReservations = await _context.Reservations
                    .Include(r => r.Rooms)
                        .ThenInclude(rm => rm.Hotel)
                    .Where(r => r.GuestId == guest.GuestId && r.Status == "Completed")
                    .ToListAsync();

                var hotelIds = completedReservations
                    .SelectMany(r => r.Rooms.Select(rm => rm.HotelId))
                    .Distinct()
                    .ToList();

                // Get hotels that haven't been reviewed yet
                var reviewedHotelIds = await _context.Reviews
                    .Where(r => r.GuestId == guest.GuestId)
                    .Select(r => r.HotelId)
                    .ToListAsync();

                EligibleHotels = await _context.Hotels
                    .Where(h => hotelIds.Contains(h.HotelId) && !reviewedHotelIds.Contains(h.HotelId))
                    .ToListAsync();

                if (hotelId.HasValue)
                {
                    Input.HotelId = hotelId.Value;
                    SelectedHotel = await _hotelService.GetHotelByIdAsync(hotelId.Value);
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading page: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.UserId == userId);

            if (guest == null)
            {
                ErrorMessage = "Guest profile not found.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await LoadEligibleHotelsAsync(guest.GuestId);
                return Page();
            }

            try
            {
                // Verify guest can review this hotel
                var canReview = await _reviewService.CanGuestReviewHotelAsync(
                    guest.GuestId,
                    Input.HotelId);

                if (!canReview)
                {
                    ErrorMessage = "You can only review hotels where you've completed a stay and haven't already reviewed.";
                    await LoadEligibleHotelsAsync(guest.GuestId);
                    return Page();
                }

                var review = new Review
                {
                    HotelId = Input.HotelId,
                    GuestId = guest.GuestId,
                    Rating = Input.Rating,
                    Comment = Input.Comment,
                    CreatedAt = DateTime.Now,
                    IsApproved = false
                };

                await _reviewService.CreateReviewAsync(review);
                TempData["SuccessMessage"] = "Review submitted successfully! It will be visible after approval.";
                return RedirectToPage("/Customer/MyReservations");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error submitting review: {ex.Message}";
                await LoadEligibleHotelsAsync(guest.GuestId);
                return Page();
            }
        }

        private async Task LoadEligibleHotelsAsync(int guestId)
        {
            var completedReservations = await _context.Reservations
                .Include(r => r.Rooms)
                    .ThenInclude(rm => rm.Hotel)
                .Where(r => r.GuestId == guestId && r.Status == "Completed")
                .ToListAsync();

            var hotelIds = completedReservations
                .SelectMany(r => r.Rooms.Select(rm => rm.HotelId))
                .Distinct()
                .ToList();

            var reviewedHotelIds = await _context.Reviews
                .Where(r => r.GuestId == guestId)
                .Select(r => r.HotelId)
                .ToListAsync();

            EligibleHotels = await _context.Hotels
                .Where(h => hotelIds.Contains(h.HotelId) && !reviewedHotelIds.Contains(h.HotelId))
                .ToListAsync();
        }
    }
}