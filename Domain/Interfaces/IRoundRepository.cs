using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoundRepository
{
    Task<Round?> GetByIdAsync(string id);
    Task<Round?> GetByIdWithPhotosAsync(string id);
    Task<IEnumerable<Round>> GetByRoomIdAsync(string roomId);
    Task<Round?> GetCurrentRoundByRoomIdAsync(string roomId);
    Task<Round> CreateAsync(Round round);
    Task<Round> UpdateAsync(Round round);
}
