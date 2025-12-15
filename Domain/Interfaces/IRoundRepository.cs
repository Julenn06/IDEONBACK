using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoundRepository
{
    Task<Round?> GetByIdAsync(Guid id);
    Task<Round?> GetByIdWithPhotosAsync(Guid id);
    Task<IEnumerable<Round>> GetByRoomIdAsync(Guid roomId);
    Task<Round?> GetCurrentRoundByRoomIdAsync(Guid roomId);
    Task<Round> CreateAsync(Round round);
    Task<Round> UpdateAsync(Round round);
}
