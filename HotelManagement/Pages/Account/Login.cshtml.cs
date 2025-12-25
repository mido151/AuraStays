using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using HotelManagement.Services;

namespace HotelManagement.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null, string? message = null)
        {
            ReturnUrl = returnUrl;

            // Check for success message from registration or other pages
            if (!string.IsNullOrEmpty(message))
            {
                SuccessMessage = message;
            }
            else if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"]?.ToString();
            }

            _logger.LogInformation("Login page accessed. Success message: {Message}", SuccessMessage);
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields.";
                return Page();
            }

            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", Input.Username);

                var user = await _authService.AuthenticateAsync(Input.Username, Input.Password);

                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt for user: {Username}", Input.Username);
                    ErrorMessage = "Invalid username or password.";
                    return Page();
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Username}", Input.Username);
                    ErrorMessage = "Your account has been deactivated. Please contact support.";
                    return Page();
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                // Add hotel claim for hotel admins
                if (user.HotelId.HasValue)
                {
                    claims.Add(new Claim("HotelId", user.HotelId.Value.ToString()));
                    _logger.LogInformation("Hotel claim added for user {Username}: HotelId={HotelId}",
                        user.Username, user.HotelId.Value);
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Input.RememberMe,
                    ExpiresUtc = Input.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("User {Username} logged in successfully with role {Role}",
                    user.Username, user.Role);

                // Redirect based on role with welcome message
                var welcomeMessage = $"Welcome back, {user.Username}!";

                // If returnUrl is provided and valid, use it
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                // Otherwise redirect based on role
                return user.Role switch
                {
                    "Admin" => RedirectToPage("/Admin/Dashboard"),
                    "HotelAdmin" => RedirectToPage("/HotelAdmin/Dashboard"),
                    "Customer" => RedirectToPage("/Index"),
                    _ => RedirectToPage("/Index")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", Input.Username);
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }
}