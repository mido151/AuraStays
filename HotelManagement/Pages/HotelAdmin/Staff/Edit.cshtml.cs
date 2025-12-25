using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelManagement.Services;
using HotelManagement.Models;
using HotelManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HotelManagement.Pages.HotelAdmin.Staff
{
    [Authorize(Roles = "HotelAdmin")]
    public class EditModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly HotelDbContext _context;

        public EditModel(IStaffService staffService, HotelDbContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        [BindProperty]
        public StaffInputModel Input { get; set; } = new();

        public List<SelectListItem> Departments { get; set; } = new();

        public class StaffInputModel
        {
            public int StaffId { get; set; }
            [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
            [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;
            public DateTime? DOB { get; set; }
            [MaxLength(10)] public string? Gender { get; set; }
            [Required][EmailAddress][MaxLength(255)] public string Email { get; set; } = string.Empty;
            [Range(0, 1000000)] public decimal? Salary { get; set; }
            public int? DeptId { get; set; }
            [MaxLength(100)] public string? Position { get; set; }
            public bool IsActive { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
                return RedirectToPage("./Index");

            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim == null)
                return Forbid();

            int hotelId = int.Parse(hotelIdClaim.Value);
            var staff = await _staffService.GetStaffByIdAsync(id.Value);

            if (staff == null || staff.HotelId != hotelId)
                return NotFound();

            Input = new StaffInputModel
            {
                StaffId = staff.StaffId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                DOB = staff.DOB,
                Gender = staff.Gender,
                Email = staff.Email ?? "",
                Salary = staff.Salary,
                DeptId = staff.DeptId,
                Position = staff.Position,
                IsActive = staff.IsActive
            };

            await LoadDepartmentsAsync();
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
                await LoadDepartmentsAsync();
                return Page();
            }

            var staff = await _staffService.GetStaffByIdAsync(Input.StaffId);
            if (staff == null || staff.HotelId != hotelId)
                return Forbid();

            staff.FirstName = Input.FirstName;
            staff.LastName = Input.LastName;
            staff.DOB = Input.DOB;
            staff.Gender = Input.Gender;
            staff.Email = Input.Email;
            staff.Salary = Input.Salary;
            staff.DeptId = Input.DeptId;
            staff.Position = Input.Position;
            staff.IsActive = Input.IsActive;

            await _staffService.UpdateStaffAsync(staff);
            TempData["SuccessMessage"] = "Staff member updated successfully!";
            return RedirectToPage("./Index");
        }

        private async Task LoadDepartmentsAsync()
        {
            var hotelIdClaim = User.FindFirst("HotelId");
            if (hotelIdClaim != null)
            {
                int hotelId = int.Parse(hotelIdClaim.Value);
                var depts = await _context.Departments
                    .Where(d => d.HotelId == hotelId)
                    .ToListAsync();

                Departments = depts.Select(d => new SelectListItem
                {
                    Value = d.DeptId.ToString(),
                    Text = d.Name
                }).ToList();
            }
        }
    }
}