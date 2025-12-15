using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoundPhotoRepository
{
    Task<RoundPhoto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RoundPhoto>> GetByRoundIdAsync(Guid roundId);
    Task<RoundPhoto?> GetByRoundAndPlayerAsync(Guid roundId, Guid playerId);
    Task<RoundPhoto> CreateAsync(RoundPhoto roundPhoto);
    Task<int> GetPhotoCountByRoundIdAsync(Guid roundId);
}
