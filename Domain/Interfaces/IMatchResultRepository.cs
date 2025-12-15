using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IMatchResultRepository
{
    Task<MatchResult?> GetByRoomIdAsync(string roomId);
    Task<MatchResult> CreateAsync(MatchResult matchResult);
    Task<IEnumerable<MatchResult>> GetByPlayerIdAsync(string playerId);
}
