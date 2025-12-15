using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly IdeonDbContext _context;

    public VoteRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<Vote?> GetByIdAsync(Guid id)
    {
        return await _context.Votes.FindAsync(id);
    }

    public async Task<IEnumerable<Vote>> GetByRoundIdAsync(Guid roundId)
    {
        return await _context.Votes
            .Where(v => v.RoundId == roundId)
            .ToListAsync();
    }

    public async Task<Vote?> GetByRoundAndVoterAsync(Guid roundId, Guid voterPlayerId)
    {
        return await _context.Votes
            .FirstOrDefaultAsync(v => v.RoundId == roundId && v.VoterPlayerId == voterPlayerId);
    }

    public async Task<Vote> CreateAsync(Vote vote)
    {
        if (vote.Id == Guid.Empty)
            vote.Id = Guid.NewGuid();
        
        if (vote.CreatedAt == DateTime.MinValue)
            vote.CreatedAt = DateTime.UtcNow;

        _context.Votes.Add(vote);
        await _context.SaveChangesAsync();
        return vote;
    }

    public async Task<int> GetVoteCountByRoundIdAsync(Guid roundId)
    {
        return await _context.Votes
            .Where(v => v.RoundId == roundId)
            .CountAsync();
    }

    public async Task<Dictionary<Guid, int>> GetVotesByPlayerInRoundAsync(Guid roundId)
    {
        return await _context.Votes
            .Where(v => v.RoundId == roundId)
            .GroupBy(v => v.VotedPlayerId)
            .Select(g => new { PlayerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PlayerId, x => x.Count);
    }
}
