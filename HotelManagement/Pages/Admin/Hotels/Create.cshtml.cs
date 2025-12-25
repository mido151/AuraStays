using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Pages.Admin.Hotels
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IHotelService _hotelService;

        public CreateModel(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [BindProperty]
        public HotelInputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class HotelInputModel
        {
            [Required(ErrorMessage = "Hotel name is required")]
            [MaxLength(255)]
            [Display(Name = "Hotel Name")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Address is required")]
            [MaxLength(500)]
            public string Address { get; set; } = string.Empty;

            [Required(ErrorMessage = "City is required")]
            [MaxLength(100)]
            public string City { get; set; } = string.Empty;

            [Required(ErrorMessage = "Country is required")]
            [MaxLength(100)]
            public string Country { get; set; } = string.Empty;

            [MaxLength(20)]
            [Display(Name = "Postal Code")]
            public string? PostalCode { get; set; }

            [MaxLength(20)]
            [Phone]
            public string? Phone { get; set; }

            [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
            [Display(Name = "Rating (0-5)")]
            public decimal Rating { get; set; } = 4.0m;

            [MaxLength(1000)]
            [Url]
            [Display(Name = "Image URL (Optional)")]
            public string? ImageUrl { get; set; }

            [MaxLength(2000)]
            public string? Description { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var hotel = new HotelEntity
                {
                    Name = Input.Name,
                    Address = Input.Address,
                    City = Input.City,
                    Country = Input.Country,
                    PostalCode = Input.PostalCode,
                    Phone = Input.Phone,
                    Rating = Input.Rating,
                    ImageUrl = Input.ImageUrl,
                    Description = Input.Description
                };

                await _hotelService.CreateHotelAsync(hotel);
                TempData["SuccessMessage"] = $"Hotel '{hotel.Name}' created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating hotel: {ex.Message}";
                return Page();
            }
        }
    }
}