using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

public interface IRoomService
{
    Task<List<RoomEntity>> GetAllRoomsAsync();
    Task<List<RoomEntity>> GetRoomsByHotelIdAsync(int hotelId);
    Task<List<RoomEntity>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut);
    Task<RoomEntity?> GetRoomByIdAsync(int id);
    Task<RoomEntity> CreateRoomAsync(RoomEntity room);
    Task<bool> UpdateRoomAsync(RoomEntity room);
    Task<bool> DeleteRoomAsync(int id);
    Task<bool> UpdateRoomStatusAsync(int roomId, string status);
}

public class RoomService : IRoomService
{
    private readonly HotelDbContext _context;

    public RoomService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomEntity>> GetAllRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservation)
            .ToListAsync();
    }

    public async Task<List<RoomEntity>> GetRoomsByHotelIdAsync(int hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .Include(r => r.Reservation)
            .ToListAsync();
    }

    public async Task<List<RoomEntity>> GetAvailableRoomsAsync(int hotelId, DateTime checkIn, DateTime checkOut)
    {
        var bookedRoomIds = await _context.Reservations
            .Where(r => r.Status == "Confirmed" &&
                       ((checkIn >= r.CheckInDate && checkIn < r.CheckOutDate) ||
                        (checkOut > r.CheckInDate && checkOut <= r.CheckOutDate) ||
                        (checkIn <= r.CheckInDate && checkOut >= r.CheckOutDate)))
            .SelectMany(r => r.Rooms.Select(rm => rm.RoomId))
            .ToListAsync();

        return await _context.Rooms
            .Where(r => r.HotelId == hotelId &&
                       r.RoomStatus == "Available" &&
                       !bookedRoomIds.Contains(r.RoomId))
            .ToListAsync();
    }

    public async Task<RoomEntity?> GetRoomByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Reservation)
            .FirstOrDefaultAsync(r => r.RoomId == id);
    }

    public async Task<RoomEntity> CreateRoomAsync(RoomEntity room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<bool> UpdateRoomAsync(RoomEntity room)
    {
        _context.Entry(room).State = EntityState.Modified;
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

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null || room.ReservationId != null)
            return false;

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRoomStatusAsync(int roomId, string status)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null)
            return false;

        room.RoomStatus = status;
        await _context.SaveChangesAsync();
        return true;
    }
}