namespace IdeonBack.Domain.Entities;

/// <summary>
/// Resultado final de una partida PhotoClash
/// </summary>
public class MatchResult
{
    public string Id { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public string WinnerPlayerId { get; set; } = string.Empty;
    public int TotalRounds { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Room Room { get; set; } = null!;
    public RoomPlayer WinnerPlayer { get; set; } = null!;
}
