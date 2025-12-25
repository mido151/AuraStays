using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.Models;

namespace HotelManagement.Services
{
    public class HotelService : IHotelService
    {
        private readonly HotelDbContext _context;

        public HotelService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<HotelEntity>> GetAllHotelsAsync()
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .OrderByDescending(h => h.Rating)
                .ToListAsync();
        }

        public async Task<HotelEntity?> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Staff)
                .Include(h => h.Reviews)
                    .ThenInclude(r => r.Guest)
                .FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task<HotelEntity> CreateHotelAsync(HotelEntity hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<bool> UpdateHotelAsync(HotelEntity hotel)
        {
            _context.Entry(hotel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HotelEntity>> SearchHotelsAsync(string searchTerm)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.Name.Contains(searchTerm) ||
                           h.City.Contains(searchTerm) ||
                           h.Country.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<decimal> GetHotelAverageRatingAsync(int hotelId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId && r.IsApproved)
                .ToListAsync();

            return reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : 0;
        }
    }
}