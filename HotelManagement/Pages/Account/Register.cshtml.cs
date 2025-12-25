using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using HotelManagement.Services;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly HotelDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IAuthService authService, HotelDbContext context, ILogger<RegisterModel> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please select account type")]
            [Display(Name = "Account Type")]
            public string AccountType { get; set; } = "Customer";

            [Display(Name = "Hotel Name")]
            public string? HotelName { get; set; }

            [Display(Name = "Hotel City")]
            public string? HotelCity { get; set; }

            [Display(Name = "Hotel Country")]
            public string? HotelCountry { get; set; }

            [Display(Name = "Hotel Address")]
            public string? HotelAddress { get; set; }

            [Display(Name = "Hotel Phone")]
            public string? HotelPhone { get; set; }

            [Display(Name = "Hotel Description")]
            public string? HotelDescription { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate hotel fields if user wants to be hotel admin
            if (Input.AccountType == "HotelAdmin")
            {
                if (string.IsNullOrWhiteSpace(Input.HotelName))
                {
                    ModelState.AddModelError("Input.HotelName", "Hotel name is required for Hotel Managers");
                }
                if (string.IsNullOrWhiteSpace(Input.HotelCity))
                {
                    ModelState.AddModelError("Input.HotelCity", "Hotel city is required for Hotel Managers");
                }
                if (string.IsNullOrWhiteSpace(Input.HotelCountry))
                {
                    ModelState.AddModelError("Input.HotelCountry", "Hotel country is required for Hotel Managers");
                }
                if (string.IsNullOrWhiteSpace(Input.HotelAddress))
                {
                    ModelState.AddModelError("Input.HotelAddress", "Hotel address is required for Hotel Managers");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if user already exists
            if (await _authService.UserExistsAsync(Input.Username, Input.Email))
            {
                ErrorMessage = "Username or email already exists.";
                return Page();
            }

            try
            {
                if (Input.AccountType == "HotelAdmin")
                {
                    // Create hotel first WITHOUT transaction (let EF handle it)
                    var hotel = new HotelEntity
                    {
                        Name = Input.HotelName!,
                        City = Input.HotelCity!,
                        Country = Input.HotelCountry!,
                        Address = Input.HotelAddress!,
                        Phone = Input.HotelPhone,
                        Description = Input.HotelDescription,
                        Rating = 4.0m,
                        PostalCode = ""
                    };

                    _context.Hotels.Add(hotel);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Hotel created successfully: {hotel.Name} (ID: {hotel.HotelId})");

                    // Register hotel admin user and link to hotel
                    // AuthService will handle its own transaction
                    var user = await _authService.RegisterAsync(
                        Input.Username,
                        Input.Email,
                        Input.Password,
                        "HotelAdmin",
                        hotel.HotelId
                    );

                    if (user == null)
                    {
                        // If user creation fails, delete the hotel
                        _context.Hotels.Remove(hotel);
                        await _context.SaveChangesAsync();

                        ErrorMessage = "Registration failed. Please try again.";
                        return Page();
                    }

                    _logger.LogInformation($"Hotel admin user created successfully: {user.Username} (ID: {user.UserId})");

                    TempData["SuccessMessage"] = "Registration successful! You can now log in and manage your hotel.";
                    return RedirectToPage("/Account/Login");
                }
                else
                {
                    // Register regular customer user
                    var user = await _authService.RegisterAsync(
                        Input.Username,
                        Input.Email,
                        Input.Password,
                        "Customer"
                    );

                    if (user == null)
                    {
                        ErrorMessage = "Registration failed. Username or email may already exist.";
                        return Page();
                    }

                    _logger.LogInformation($"Customer user created successfully: {user.Username} (ID: {user.UserId})");

                    TempData["SuccessMessage"] = "Registration successful! Please log in.";
                    return RedirectToPage("/Account/Login");
                }
            }
            catch (Exception ex)
            {
                // Log detailed error
                _logger.LogError(ex, "Registration error occurred");

                // Display user-friendly error message
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception details");

                    // Check for common SQL errors
                    var innerMessage = ex.InnerException.Message.ToLower();

                    if (innerMessage.Contains("foreign key"))
                    {
                        ErrorMessage = "Database constraint error. Please ensure all required fields are valid.";
                    }
                    else if (innerMessage.Contains("unique") || innerMessage.Contains("duplicate"))
                    {
                        ErrorMessage = "This username or email already exists. Please use different credentials.";
                    }
                    else if (innerMessage.Contains("null"))
                    {
                        ErrorMessage = "Required information is missing. Please fill all required fields.";
                    }
                    else if (innerMessage.Contains("transaction"))
                    {
                        ErrorMessage = "Database transaction error occurred. Please try again.";
                    }
                    else
                    {
                        ErrorMessage = $"Registration failed: {ex.InnerException.Message}";
                    }
                }
                else
                {
                    ErrorMessage = $"Registration failed: {ex.Message}";
                }

                return Page();
            }
        }
    }
}