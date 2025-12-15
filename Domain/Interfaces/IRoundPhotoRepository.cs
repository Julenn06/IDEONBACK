using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoundPhotoRepository
{
    Task<RoundPhoto?> GetByIdAsync(string id);
    Task<IEnumerable<RoundPhoto>> GetByRoundIdAsync(string roundId);
    Task<RoundPhoto?> GetByRoundAndPlayerAsync(string roundId, string playerId);
    Task<RoundPhoto> CreateAsync(RoundPhoto roundPhoto);
    Task<int> GetPhotoCountByRoundIdAsync(string roundId);
}
