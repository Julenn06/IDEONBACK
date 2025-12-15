using IdeonBack.Domain.Enums;

namespace IdeonBack.Domain.Entities;

/// <summary>
/// Sala de juego para el modo PhotoClash PvP
/// </summary>
public class Room
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public RoomStatus Status { get; set; } = RoomStatus.Waiting;
    public int RoundsTotal { get; set; }
    public int SecondsPerRound { get; set; }
    public bool NsfwAllowed { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public ICollection<RoomPlayer> Players { get; set; } = new List<RoomPlayer>();
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public MatchResult? MatchResult { get; set; }
}
