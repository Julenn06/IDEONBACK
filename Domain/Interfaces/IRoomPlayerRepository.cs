using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoomPlayerRepository
{
    Task<RoomPlayer?> GetByIdAsync(Guid id);
    Task<IEnumerable<RoomPlayer>> GetByRoomIdAsync(Guid roomId);
    Task<RoomPlayer?> GetByRoomAndUserAsync(Guid roomId, Guid userId);
    Task<RoomPlayer> CreateAsync(RoomPlayer roomPlayer);
    Task<RoomPlayer> UpdateAsync(RoomPlayer roomPlayer);
    Task<int> GetPlayerCountByRoomIdAsync(Guid roomId);
}
