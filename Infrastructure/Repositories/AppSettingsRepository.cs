using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdeonBack.Infrastructure.Repositories;

public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly IdeonDbContext _context;

    public AppSettingsRepository(IdeonDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings?> GetByUserIdAsync(Guid userId)
    {
        return await _context.AppSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<AppSettings> CreateAsync(AppSettings settings)
    {
        if (settings.Id == Guid.Empty)
            settings.Id = Guid.NewGuid();

        _context.AppSettings.Add(settings);
        await _context.SaveChangesAsync();
        return settings;
    }

    public async Task<AppSettings> UpdateAsync(AppSettings settings)
    {
        _context.AppSettings.Update(settings);
        await _context.SaveChangesAsync();
        return settings;
    }
}
