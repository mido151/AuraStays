using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

public interface IReviewService
{
    Task<List<Review>> GetReviewsByHotelIdAsync(int hotelId, bool approvedOnly = true);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<Review> CreateReviewAsync(Review review);
    Task<bool> ApproveReviewAsync(int id);
    Task<bool> DeleteReviewAsync(int id);
    Task<bool> CanGuestReviewHotelAsync(int guestId, int hotelId);
}

public class ReviewService : IReviewService
{
    private readonly HotelDbContext _context;

    public ReviewService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetReviewsByHotelIdAsync(int hotelId, bool approvedOnly = true)
    {
        var query = _context.Reviews
            .Include(r => r.Guest)
            .Where(r => r.HotelId == hotelId);

        if (approvedOnly)
            query = query.Where(r => r.IsApproved);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.Guest)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.ReviewId == id);
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        review.CreatedAt = DateTime.Now;
        review.IsApproved = false;
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<bool> ApproveReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return false;

        review.IsApproved = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanGuestReviewHotelAsync(int guestId, int hotelId)
    {
        // Check if guest has completed stay at this hotel
        var hasStayed = await _context.Reservations
            .AnyAsync(r => r.GuestId == guestId &&
                          r.Status == "Completed" &&
                          r.Rooms.Any(rm => rm.HotelId == hotelId));

        if (!hasStayed)
            return false;

        // Check if already reviewed
        var hasReviewed = await _context.Reviews
            .AnyAsync(r => r.GuestId == guestId && r.HotelId == hotelId);

        return !hasReviewed;
    }
}