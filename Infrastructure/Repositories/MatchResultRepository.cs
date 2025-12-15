using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class MatchResultRepository : IMatchResultRepository
{
    private readonly IdeonDbContext _context;

    public MatchResultRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<MatchResult?> GetByRoomIdAsync(string roomId)
    {
        return await _context.MatchResults
            .Include(mr => mr.WinnerPlayer)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(mr => mr.RoomId == roomId);
    }

    public async Task<MatchResult> CreateAsync(MatchResult matchResult)
    {
        if (matchResult.Id == string.Empty)
            matchResult.Id = Guid.NewGuid().ToString();
        
        if (matchResult.CreatedAt == DateTime.MinValue)
            matchResult.CreatedAt = DateTime.UtcNow;

        _context.MatchResults.Add(matchResult);
        await _context.SaveChangesAsync();
        return matchResult;
    }

    public async Task<IEnumerable<MatchResult>> GetByPlayerIdAsync(string playerId)
    {
        return await _context.MatchResults
            .Where(mr => mr.WinnerPlayerId == playerId)
            .Include(mr => mr.Room)
            .OrderByDescending(mr => mr.CreatedAt)
            .ToListAsync();
    }
}
