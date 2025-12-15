using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly IdeonDbContext _context;

    public PhotoRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<Photo?> GetByIdAsync(Guid id)
    {
        return await _context.Photos.FindAsync(id);
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Photos
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.DateTaken)
            .ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetUnreviewedByUserIdAsync(Guid userId)
    {
        return await _context.Photos
            .Where(p => p.UserId == userId && p.KeepStatus == null)
            .OrderBy(p => p.DateTaken)
            .ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetDeletedByUserIdAsync(Guid userId, int limit = 5)
    {
        return await _context.Photos
            .Where(p => p.UserId == userId && p.KeepStatus == false)
            .OrderByDescending(p => p.ReviewedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Photo> CreateAsync(Photo photo)
    {
        if (photo.Id == Guid.Empty)
            photo.Id = Guid.NewGuid();

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task<Photo> UpdateAsync(Photo photo)
    {
        _context.Photos.Update(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task DeleteAsync(Guid id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo != null)
        {
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetDeletedCountByUserIdAsync(Guid userId)
    {
        return await _context.Photos
            .Where(p => p.UserId == userId && p.KeepStatus == false)
            .CountAsync();
    }
}
