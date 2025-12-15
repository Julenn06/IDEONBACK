using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(Guid id);
    Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Photo>> GetUnreviewedByUserIdAsync(Guid userId);
    Task<IEnumerable<Photo>> GetDeletedByUserIdAsync(Guid userId, int limit = 5);
    Task<Photo> CreateAsync(Photo photo);
    Task<Photo> UpdateAsync(Photo photo);
    Task DeleteAsync(Guid id);
    Task<int> GetDeletedCountByUserIdAsync(Guid userId);
}
