using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Users = await _context.Users
                    .Include(u => u.Hotel)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();

                if (!string.IsNullOrEmpty((string?)TempData["SuccessMessage"]))
                {
                    SuccessMessage = (string?)TempData["SuccessMessage"];
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading users: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage();
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = user.IsActive
                    ? $"User {user.Username} activated successfully!"
                    : $"User {user.Username} deactivated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating user: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int userId, string newRole)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage();
                }

                var oldRole = user.Role;
                user.Role = newRole;

                // If changing to HotelAdmin, clear hotel assignment until they add one
                if (newRole == "HotelAdmin" && user.HotelId == null)
                {
                    TempData["SuccessMessage"] = $"User {user.Username} role changed to Hotel Admin. Please assign them to a hotel.";
                }
                else if (newRole == "Customer" || newRole == "Admin")
                {
                    user.HotelId = null; // Clear hotel assignment for customers and admins
                    TempData["SuccessMessage"] = $"User {user.Username} role changed from {oldRole} to {newRole}.";
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error changing role: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage();
                }

                // Don't allow deleting admin users
                if (user.Role == "Admin")
                {
                    TempData["ErrorMessage"] = "Cannot delete admin users.";
                    return RedirectToPage();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"User {user.Username} deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}