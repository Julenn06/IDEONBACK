namespace IdeonBack.Domain.Entities;

/// <summary>
/// Representa un usuario de la aplicación IDEON
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }

    // Navegación
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<RoomPlayer> RoomPlayers { get; set; } = new List<RoomPlayer>();
    public AppSettings? AppSettings { get; set; }
}
