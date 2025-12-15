using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Interfaces;

namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio para gestionar usuarios
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAppSettingsRepository _appSettingsRepository;

    public UserService(IUserRepository userRepository, IAppSettingsRepository appSettingsRepository)
    {
        _userRepository = userRepository;
        _appSettingsRepository = appSettingsRepository;
    }

    /// <summary>
    /// Crear un nuevo usuario
    /// </summary>
    public async Task<User> CreateUserAsync(string username, string? avatarUrl = null)
    {
        // Validar que el username no exista
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
            throw new InvalidOperationException("El nombre de usuario ya existe");

        var user = new User
        {
            Username = username,
            AvatarUrl = avatarUrl,
            LastLogin = DateTime.UtcNow
        };

        user = await _userRepository.CreateAsync(user);

        // Crear configuración por defecto
        var settings = new AppSettings
        {
            UserId = user.Id,
            DarkMode = false,
            Notifications = true,
            Language = "es"
        };
        await _appSettingsRepository.CreateAsync(settings);

        return user;
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    public async Task<User?> GetUserAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    /// <summary>
    /// Obtener usuario por nombre
    /// </summary>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    /// <summary>
    /// Actualizar último login
    /// </summary>
    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
    }

    /// <summary>
    /// Actualizar configuración del usuario
    /// </summary>
    public async Task<AppSettings> UpdateSettingsAsync(Guid userId, bool? darkMode = null, bool? notifications = null, string? language = null)
    {
        var settings = await _appSettingsRepository.GetByUserIdAsync(userId);
        
        if (settings == null)
        {
            settings = new AppSettings
            {
                UserId = userId,
                DarkMode = darkMode ?? false,
                Notifications = notifications ?? true,
                Language = language ?? "es"
            };
            return await _appSettingsRepository.CreateAsync(settings);
        }

        if (darkMode.HasValue) settings.DarkMode = darkMode.Value;
        if (notifications.HasValue) settings.Notifications = notifications.Value;
        if (language != null) settings.Language = language;

        return await _appSettingsRepository.UpdateAsync(settings);
    }

    /// <summary>
    /// Obtener configuración del usuario
    /// </summary>
    public async Task<AppSettings?> GetSettingsAsync(Guid userId)
    {
        return await _appSettingsRepository.GetByUserIdAsync(userId);
    }
}
