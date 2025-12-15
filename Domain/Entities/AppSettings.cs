namespace IdeonBack.Domain.Entities;

/// <summary>
/// Configuración de la aplicación por usuario
/// </summary>
public class AppSettings
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool DarkMode { get; set; } = false;
    public bool Notifications { get; set; } = true;
    public string Language { get; set; } = "es";

    // Navegación
    public User User { get; set; } = null!;
}
