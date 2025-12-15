using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoomPlayerRepository
{
    Task<RoomPlayer?> GetByIdAsync(string id);
    Task<IEnumerable<RoomPlayer>> GetByRoomIdAsync(string roomId);
    Task<RoomPlayer?> GetByRoomAndUserAsync(string roomId, string userId);
    Task<RoomPlayer> CreateAsync(RoomPlayer roomPlayer);
    Task<RoomPlayer> UpdateAsync(RoomPlayer roomPlayer);
    Task<int> GetPlayerCountByRoomIdAsync(string roomId);
}
