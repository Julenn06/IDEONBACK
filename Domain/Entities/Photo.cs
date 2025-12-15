namespace IdeonBack.Domain.Entities;

/// <summary>
/// Foto del modo PhotoSweep
/// </summary>
public class Photo
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public DateTime? DateTaken { get; set; }
    public bool? KeepStatus { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navegación
    public User User { get; set; } = null!;
}
