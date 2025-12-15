using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class RoomPlayerRepository : IRoomPlayerRepository
{
    private readonly IdeonDbContext _context;

    public RoomPlayerRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<RoomPlayer?> GetByIdAsync(Guid id)
    {
        return await _context.RoomPlayers
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<IEnumerable<RoomPlayer>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.RoomPlayers
            .Include(rp => rp.User)
            .Where(rp => rp.RoomId == roomId)
            .ToListAsync();
    }

    public async Task<RoomPlayer?> GetByRoomAndUserAsync(Guid roomId, Guid userId)
    {
        return await _context.RoomPlayers
            .Include(rp => rp.User)
            .FirstOrDefaultAsync(rp => rp.RoomId == roomId && rp.UserId == userId);
    }

    public async Task<RoomPlayer> CreateAsync(RoomPlayer roomPlayer)
    {
        if (roomPlayer.Id == Guid.Empty)
            roomPlayer.Id = Guid.NewGuid();
        
        if (roomPlayer.JoinedAt == DateTime.MinValue)
            roomPlayer.JoinedAt = DateTime.UtcNow;

        _context.RoomPlayers.Add(roomPlayer);
        await _context.SaveChangesAsync();
        return roomPlayer;
    }

    public async Task<RoomPlayer> UpdateAsync(RoomPlayer roomPlayer)
    {
        _context.RoomPlayers.Update(roomPlayer);
        await _context.SaveChangesAsync();
        return roomPlayer;
    }

    public async Task<int> GetPlayerCountByRoomIdAsync(Guid roomId)
    {
        return await _context.RoomPlayers
            .Where(rp => rp.RoomId == roomId)
            .CountAsync();
    }
}
