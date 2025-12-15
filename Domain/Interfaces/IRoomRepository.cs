using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(string id);
    Task<Room?> GetByCodeAsync(string code);
    Task<Room?> GetByIdWithPlayersAsync(string id);
    Task<Room?> GetByCodeWithPlayersAsync(string code);
    Task<Room> CreateAsync(Room room);
    Task<Room> UpdateAsync(Room room);
    Task<IEnumerable<Room>> GetActiveRoomsAsync();
}
