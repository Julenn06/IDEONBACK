namespace IdeonBack.Domain.Entities;

/// <summary>
/// Foto subida por un jugador en una ronda específica
/// </summary>
public class RoundPhoto
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public Guid PlayerId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }

    // Navegación
    public Round Round { get; set; } = null!;
    public RoomPlayer Player { get; set; } = null!;
}
