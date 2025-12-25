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
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
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
            _logger.LogInformation("Registration page accessed");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Registration attempt for username: {Username}, AccountType: {AccountType}",
                Input.Username, Input.AccountType);

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
                ErrorMessage = "Please correct the errors and try again.";
                return Page();
            }

            // Check if user already exists
            if (await _authService.UserExistsAsync(Input.Username, Input.Email))
            {
                _logger.LogWarning("Registration failed - Username or email already exists: {Username}, {Email}",
                    Input.Username, Input.Email);
                ErrorMessage = "Username or email already exists. Please choose different credentials.";
                return Page();
            }

            try
            {
                if (Input.AccountType == "HotelAdmin")
                {
                    _logger.LogInformation("Creating hotel for new hotel admin: {HotelName}", Input.HotelName);

                    // Create hotel first
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

                    _logger.LogInformation("Hotel created successfully: {HotelName} (ID: {HotelId})",
                        hotel.Name, hotel.HotelId);

                    // Register hotel admin user and link to hotel
                    var user = await _authService.RegisterAsync(
                        Input.Username,
                        Input.Email,
                        Input.Password,
                        "HotelAdmin",
                        hotel.HotelId
                    );

                    if (user == null)
                    {
                        _logger.LogError("Failed to create hotel admin user, rolling back hotel creation");
                        _context.Hotels.Remove(hotel);
                        await _context.SaveChangesAsync();

                        ErrorMessage = "Registration failed. Please try again.";
                        return Page();
                    }

                    _logger.LogInformation("Hotel admin user created successfully: {Username} (ID: {UserId})",
                        user.Username, user.UserId);

                    // SUCCESS - Set TempData for toast notification
                    TempData["SuccessMessage"] = $"? Account created successfully! Welcome to AuraStays, {user.Username}! You can now log in and manage your hotel.";

                    return RedirectToPage("/Account/Login");
                }
                else
                {
                    _logger.LogInformation("Creating customer user: {Username}", Input.Username);

                    // Register regular customer user
                    var user = await _authService.RegisterAsync(
                        Input.Username,
                        Input.Email,
                        Input.Password,
                        "Customer"
                    );

                    if (user == null)
                    {
                        _logger.LogError("Failed to create customer user: {Username}", Input.Username);
                        ErrorMessage = "Registration failed. Username or email may already exist.";
                        return Page();
                    }

                    _logger.LogInformation("Customer user created successfully: {Username} (ID: {UserId})",
                        user.Username, user.UserId);

                    // SUCCESS - Set TempData for toast notification
                    TempData["SuccessMessage"] = $"?? Account created successfully! Welcome to AuraStays, {user.Username}! Please log in to start booking.";

                    return RedirectToPage("/Account/Login");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error occurred for user: {Username}", Input.Username);

                // Provide user-friendly error messages
                if (ex.InnerException != null)
                {
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
                    ErrorMessage = "An unexpected error occurred during registration. Please try again.";
                }

                return Page();
            }
        }
    }
}