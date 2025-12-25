using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace HotelManagement.Services
{
    public interface IHotelService
    {
        Task<List<HotelEntity>> GetAllHotelsAsync();
        Task<HotelEntity?> GetHotelByIdAsync(int id);
        Task<HotelEntity> CreateHotelAsync(HotelEntity hotel);
        Task<bool> UpdateHotelAsync(HotelEntity hotel);
        Task<bool> DeleteHotelAsync(int id);
        Task<List<HotelEntity>> SearchHotelsAsync(string searchTerm);
        Task<decimal> GetHotelAverageRatingAsync(int hotelId);
    }
}