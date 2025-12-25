using HotelManagement.Data;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;

public interface IStaffService
{
    Task<List<StaffEntity>> GetAllStaffAsync();
    Task<List<StaffEntity>> GetStaffByHotelIdAsync(int hotelId);
    Task<StaffEntity?> GetStaffByIdAsync(int id);
    Task<StaffEntity> CreateStaffAsync(StaffEntity staff);
    Task<bool> UpdateStaffAsync(StaffEntity staff);
    Task<bool> DeleteStaffAsync(int id);
    Task<bool> DeactivateStaffAsync(int id);
}

public class StaffService : IStaffService
{
    private readonly HotelDbContext _context;

    public StaffService(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<List<StaffEntity>> GetAllStaffAsync()
    {
        return await _context.Staff
            .Include(s => s.Hotel)
            .Include(s => s.Department)
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task<List<StaffEntity>> GetStaffByHotelIdAsync(int hotelId)
    {
        return await _context.Staff
            .Include(s => s.Department)
            .Where(s => s.HotelId == hotelId && s.IsActive)
            .OrderBy(s => s.Department!.Name)
            .ThenBy(s => s.LastName)
            .ToListAsync();
    }

    public async Task<StaffEntity?> GetStaffByIdAsync(int id)
    {
        return await _context.Staff
            .Include(s => s.Hotel)
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.StaffId == id);
    }

    public async Task<StaffEntity> CreateStaffAsync(StaffEntity staff)
    {
        staff.IsActive = true;
        _context.Staff.Add(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<bool> UpdateStaffAsync(StaffEntity staff)
    {
        _context.Entry(staff).State = EntityState.Modified;
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

    public async Task<bool> DeleteStaffAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
            return false;

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateStaffAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
            return false;

        staff.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}