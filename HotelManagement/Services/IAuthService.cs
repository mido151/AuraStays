using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace HotelManagement.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User?> RegisterAsync(string username, string email, string password, string role, int? hotelId = null);
        Task<bool> UserExistsAsync(string username, string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }
}