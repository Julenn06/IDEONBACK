using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IMatchResultRepository
{
    Task<MatchResult?> GetByRoomIdAsync(Guid roomId);
    Task<MatchResult> CreateAsync(MatchResult matchResult);
    Task<IEnumerable<MatchResult>> GetByPlayerIdAsync(Guid playerId);
}
