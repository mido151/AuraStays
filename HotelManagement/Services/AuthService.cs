using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly HotelDbContext _context;

        public AuthService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Hotel)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            if (user == null)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isPasswordValid ? user : null;
        }

        public async Task<User?> RegisterAsync(string username, string email, string password, string role, int? hotelId = null)
        {
            // Check if user already exists
            if (await UserExistsAsync(username, email))
                return null;

            try
            {
                // Step 1: Create User
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = role,
                    HotelId = hotelId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Save to get UserId

                // Step 2: Create Guest profile for Customer role
                if (role == "Customer")
                {
                    var guest = new GuestEntity
                    {
                        FirstName = username,  // Use username as first name initially
                        LastName = "User",     // Default last name
                        UserId = user.UserId,  // Now we have the UserId
                        DOB = null,
                        Nationality = null,
                        Gender = null,
                        AddressStreet = null,
                        AddressCity = null,
                        AddressCountry = null,
                        LoyaltyId = $"CUST{user.UserId:D6}"  // Generate loyalty ID
                    };

                    _context.Guests.Add(guest);
                    await _context.SaveChangesAsync();
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Registration Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                throw new Exception($"Registration failed: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Hotel)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}