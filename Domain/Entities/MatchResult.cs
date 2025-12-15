namespace IdeonBack.Domain.Entities;

/// <summary>
/// Resultado final de una partida PhotoClash
/// </summary>
public class MatchResult
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid WinnerPlayerId { get; set; }
    public int TotalRounds { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Room Room { get; set; } = null!;
    public RoomPlayer WinnerPlayer { get; set; } = null!;
}
