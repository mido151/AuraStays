using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagement.Services;
using HotelManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Pages.Admin.Hotels
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IHotelService _hotelService;

        public EditModel(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [BindProperty]
        public HotelInputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class HotelInputModel
        {
            public int HotelId { get; set; }

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
            public decimal Rating { get; set; }

            [MaxLength(1000)]
            [Url]
            [Display(Name = "Image URL (Optional)")]
            public string? ImageUrl { get; set; }

            [MaxLength(2000)]
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue)
                return RedirectToPage("./Index");

            var hotel = await _hotelService.GetHotelByIdAsync(id.Value);
            if (hotel == null)
                return NotFound();

            Input = new HotelInputModel
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PostalCode = hotel.PostalCode,
                Phone = hotel.Phone,
                Rating = hotel.Rating,
                ImageUrl = hotel.ImageUrl,
                Description = hotel.Description
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(Input.HotelId);
                if (hotel == null)
                {
                    ErrorMessage = "Hotel not found.";
                    return Page();
                }

                hotel.Name = Input.Name;
                hotel.Address = Input.Address;
                hotel.City = Input.City;
                hotel.Country = Input.Country;
                hotel.PostalCode = Input.PostalCode;
                hotel.Phone = Input.Phone;
                hotel.Rating = Input.Rating;
                hotel.ImageUrl = Input.ImageUrl;
                hotel.Description = Input.Description;

                var success = await _hotelService.UpdateHotelAsync(hotel);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Hotel '{hotel.Name}' updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = "Failed to update hotel. Please try again.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating hotel: {ex.Message}";
                return Page();
            }
        }
    }
}