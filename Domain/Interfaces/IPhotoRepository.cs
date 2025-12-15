using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(string id);
    Task<IEnumerable<Photo>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Photo>> GetUnreviewedByUserIdAsync(string userId);
    Task<IEnumerable<Photo>> GetDeletedByUserIdAsync(string userId, int limit = 5);
    Task<Photo> CreateAsync(Photo photo);
    Task<Photo> UpdateAsync(Photo photo);
    Task DeleteAsync(string id);
    Task<int> GetDeletedCountByUserIdAsync(string userId);
}
