using IdeonBack.Domain.Entities;

namespace IdeonBack.Domain.Interfaces;

public interface IAppSettingsRepository
{
    Task<AppSettings?> GetByUserIdAsync(Guid userId);
    Task<AppSettings> CreateAsync(AppSettings settings);
    Task<AppSettings> UpdateAsync(AppSettings settings);
}
