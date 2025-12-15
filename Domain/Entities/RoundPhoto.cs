namespace IdeonBack.Domain.Entities;

/// <summary>
/// Foto subida por un jugador en una ronda específica
/// </summary>
public class RoundPhoto
{
    public string Id { get; set; } = string.Empty;
    public string RoundId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }

    // Navegación
    public Round Round { get; set; } = null!;
    public RoomPlayer Player { get; set; } = null!;
}
