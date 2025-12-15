using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly IdeonDbContext _context;

    public RoomRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id)
    {
        return await _context.Rooms.FindAsync(id);
    }

    public async Task<Room?> GetByCodeAsync(string code)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Code == code);
    }

    public async Task<Room?> GetByIdWithPlayersAsync(Guid id)
    {
        return await _context.Rooms
            .Include(r => r.Players)
                .ThenInclude(p => p.User)
            .Include(r => r.Rounds)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room?> GetByCodeWithPlayersAsync(string code)
    {
        return await _context.Rooms
            .Include(r => r.Players)
                .ThenInclude(p => p.User)
            .Include(r => r.Rounds)
            .FirstOrDefaultAsync(r => r.Code == code);
    }

    public async Task<Room> CreateAsync(Room room)
    {
        if (room.Id == Guid.Empty)
            room.Id = Guid.NewGuid();
        
        if (room.CreatedAt == DateTime.MinValue)
            room.CreatedAt = DateTime.UtcNow;

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<Room> UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<IEnumerable<Room>> GetActiveRoomsAsync()
    {
        return await _context.Rooms
            .Where(r => r.Status != Domain.Enums.RoomStatus.Finished)
            .Include(r => r.Players)
            .ToListAsync();
    }
}
