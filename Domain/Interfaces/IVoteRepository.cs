using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vote>> GetByRoundIdAsync(Guid roundId);
    Task<Vote?> GetByRoundAndVoterAsync(Guid roundId, Guid voterPlayerId);
    Task<Vote> CreateAsync(Vote vote);
    Task<int> GetVoteCountByRoundIdAsync(Guid roundId);
    Task<Dictionary<Guid, int>> GetVotesByPlayerInRoundAsync(Guid roundId);
}
