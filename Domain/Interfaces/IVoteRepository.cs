using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IVoteRepository
{
    Task<Vote?> GetByIdAsync(string id);
    Task<IEnumerable<Vote>> GetByRoundIdAsync(string roundId);
    Task<Vote?> GetByRoundAndVoterAsync(string roundId, string voterPlayerId);
    Task<Vote> CreateAsync(Vote vote);
    Task<int> GetVoteCountByRoundIdAsync(string roundId);
    Task<Dictionary<string, int>> GetVotesByPlayerInRoundAsync(string roundId);
}
