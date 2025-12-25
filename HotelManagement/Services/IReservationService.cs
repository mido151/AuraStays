using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

public interface IReservationService
{
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<List<Reservation>> GetReservationsByGuestIdAsync(int guestId);
    Task<List<Reservation>> GetReservationsByHotelIdAsync(int hotelId);
    Task<Reservation?> GetReservationByIdAsync(int id);
    Task<Reservation?> CreateReservationAsync(Reservation reservation, List<int> roomIds);
    Task<bool> UpdateReservationAsync(Reservation reservation);
    Task<bool> CancelReservationAsync(int id);
    Task<bool> CheckOutReservationAsync(int id);
    Task<List<Reservation>> GetActiveReservationsByHotelAsync(int hotelId);
}

public class ReservationService : IReservationService
{
    private readonly HotelDbContext _context;

    public ReservationService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        return await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Rooms)
                .ThenInclude(rm => rm.Hotel)
            .Include(r => r.CreatedByStaff)
            .OrderByDescending(r => r.BookingDate)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetReservationsByGuestIdAsync(int guestId)
    {
        return await _context.Reservations
            .Where(r => r.GuestId == guestId)
            .Include(r => r.Rooms)
                .ThenInclude(rm => rm.Hotel)
            .OrderByDescending(r => r.BookingDate)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetReservationsByHotelIdAsync(int hotelId)
    {
        return await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Rooms)
            .Where(r => r.Rooms.Any(rm => rm.HotelId == hotelId))
            .OrderByDescending(r => r.CheckInDate)
            .ToListAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Rooms)
                .ThenInclude(rm => rm.Hotel)
            .Include(r => r.Payments)
            .FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task<Reservation?> CreateReservationAsync(Reservation reservation, List<int> roomIds)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var rooms = await _context.Rooms
                .Where(r => roomIds.Contains(r.RoomId))
                .ToListAsync();

            foreach (var room in rooms)
            {
                room.ReservationId = reservation.ReservationId;
                room.RoomStatus = "Occupied";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return reservation;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<bool> UpdateReservationAsync(Reservation reservation)
    {
        _context.Entry(reservation).State = EntityState.Modified;
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

    public async Task<bool> CancelReservationAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Rooms)
            .FirstOrDefaultAsync(r => r.ReservationId == id);

        if (reservation == null)
            return false;

        reservation.Status = "Cancelled";

        foreach (var room in reservation.Rooms)
        {
            room.ReservationId = null;
            room.RoomStatus = "Available";
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckOutReservationAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Rooms)
            .FirstOrDefaultAsync(r => r.ReservationId == id);

        if (reservation == null)
            return false;

        reservation.Status = "Completed";

        foreach (var room in reservation.Rooms)
        {
            room.ReservationId = null;
            room.RoomStatus = "Available";
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Reservation>> GetActiveReservationsByHotelAsync(int hotelId)
    {
        return await _context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Rooms)
            .Where(r => r.Rooms.Any(rm => rm.HotelId == hotelId) &&
                       r.Status == "Confirmed" &&
                       r.CheckOutDate >= DateTime.Today)
            .OrderBy(r => r.CheckOutDate)
            .ToListAsync();
    }
}