using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using HotelManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelManagement.Pages.HotelAdmin.Staff
{
    [Authorize(Roles = "HotelAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly HotelDbContext _context;

        public IndexModel(IStaffService staffService, HotelDbContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        public List<StaffEntity> StaffMembers { get; set; } = new();
        public List<Department> Departments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            int hotelId = int.Parse(hotelIdClaim.Value);

            StaffMembers = await _staffService.GetStaffByHotelIdAsync(hotelId);
            Departments = await _context.Departments
                .Where(d => d.HotelId == hotelId)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int staffId)
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            var staff = await _staffService.GetStaffByIdAsync(staffId);
            if (staff == null || staff.HotelId != int.Parse(hotelIdClaim.Value))
                return Forbid();

            await _staffService.DeleteStaffAsync(staffId);
            TempData["SuccessMessage"] = "Staff member deleted successfully!";
            return RedirectToPage();
        }
    }
}