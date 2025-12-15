using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class RoundPhotoRepository : IRoundPhotoRepository
{
    private readonly IdeonDbContext _context;

    public RoundPhotoRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<RoundPhoto?> GetByIdAsync(Guid id)
    {
        return await _context.RoundPhotos.FindAsync(id);
    }

    public async Task<IEnumerable<RoundPhoto>> GetByRoundIdAsync(Guid roundId)
    {
        return await _context.RoundPhotos
            .Include(rp => rp.Player)
                .ThenInclude(p => p.User)
            .Where(rp => rp.RoundId == roundId)
            .ToListAsync();
    }

    public async Task<RoundPhoto?> GetByRoundAndPlayerAsync(Guid roundId, Guid playerId)
    {
        return await _context.RoundPhotos
            .FirstOrDefaultAsync(rp => rp.RoundId == roundId && rp.PlayerId == playerId);
    }

    public async Task<RoundPhoto> CreateAsync(RoundPhoto roundPhoto)
    {
        if (roundPhoto.Id == Guid.Empty)
            roundPhoto.Id = Guid.NewGuid();
        
        if (roundPhoto.UploadedAt == DateTime.MinValue)
            roundPhoto.UploadedAt = DateTime.UtcNow;

        _context.RoundPhotos.Add(roundPhoto);
        await _context.SaveChangesAsync();
        return roundPhoto;
    }

    public async Task<int> GetPhotoCountByRoundIdAsync(Guid roundId)
    {
        return await _context.RoundPhotos
            .Where(rp => rp.RoundId == roundId)
            .CountAsync();
    }
}
