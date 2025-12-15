using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class RoundRepository : IRoundRepository
{
    private readonly IdeonDbContext _context;

    public RoundRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<Round?> GetByIdAsync(Guid id)
    {
        return await _context.Rounds.FindAsync(id);
    }

    public async Task<Round?> GetByIdWithPhotosAsync(Guid id)
    {
        return await _context.Rounds
            .Include(r => r.RoundPhotos)
                .ThenInclude(rp => rp.Player)
                    .ThenInclude(p => p.User)
            .Include(r => r.Votes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Round>> GetByRoomIdAsync(Guid roomId)
    {
        return await _context.Rounds
            .Where(r => r.RoomId == roomId)
            .OrderBy(r => r.RoundNumber)
            .ToListAsync();
    }

    public async Task<Round?> GetCurrentRoundByRoomIdAsync(Guid roomId)
    {
        return await _context.Rounds
            .Where(r => r.RoomId == roomId && r.FinishedAt == null)
            .OrderByDescending(r => r.RoundNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<Round> CreateAsync(Round round)
    {
        if (round.Id == Guid.Empty)
            round.Id = Guid.NewGuid();

        _context.Rounds.Add(round);
        await _context.SaveChangesAsync();
        return round;
    }

    public async Task<Round> UpdateAsync(Round round)
    {
        _context.Rounds.Update(round);
        await _context.SaveChangesAsync();
        return round;
    }
}
