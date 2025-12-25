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
    public class CreateModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly HotelDbContext _context;

        public CreateModel(IStaffService staffService, HotelDbContext context)
        {
            _staffService = staffService;
            _context = context;
        }

        [BindProperty]
        public StaffInputModel Input { get; set; } = new();

        public List<SelectListItem> Departments { get; set; } = new();

        public class StaffInputModel
        {
            [Required][MaxLength(100)] public string FirstName { get; set; } = string.Empty;
            [Required][MaxLength(100)] public string LastName { get; set; } = string.Empty;
            public DateTime? DOB { get; set; }
            [MaxLength(10)] public string? Gender { get; set; }
            [Required][EmailAddress][MaxLength(255)] public string Email { get; set; } = string.Empty;
            [Range(0, 1000000)] public decimal? Salary { get; set; }
            public int? DeptId { get; set; }
            [MaxLength(100)] public string? Position { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
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

            var staff = new StaffEntity
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                DOB = Input.DOB,
                Gender = Input.Gender,
                Email = Input.Email,
                Salary = Input.Salary,
                DeptId = Input.DeptId,
                Position = Input.Position,
                HotelId = hotelId,
                IsActive = true
            };

            await _staffService.CreateStaffAsync(staff);
            TempData["SuccessMessage"] = "Staff member added successfully!";
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