using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages.Admin.Reviews
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public List<Review> PendingReviews { get; set; } = new();
        public List<Review> ApprovedReviews { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                PendingReviews = await _context.Reviews
                    .Include(r => r.Hotel)
                    .Include(r => r.Guest)
                    .Where(r => !r.IsApproved)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                ApprovedReviews = await _context.Reviews
                    .Include(r => r.Hotel)
                    .Include(r => r.Guest)
                    .Where(r => r.IsApproved)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                if (!string.IsNullOrEmpty((string?)TempData["SuccessMessage"]))
                {
                    SuccessMessage = (string?)TempData["SuccessMessage"];
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading reviews: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToPage();
                }

                review.IsApproved = true;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review approved successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving review: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToPage();
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review rejected and deleted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting review: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    TempData["ErrorMessage"] = "Review not found.";
                    return RedirectToPage();
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Review deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting review: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}