namespace IdeonBack.Domain.Entities;

/// <summary>
/// Voto emitido por un jugador hacia otro en una ronda
/// </summary>
public class Vote
{
    public string Id { get; set; } = string.Empty;
    public string RoundId { get; set; } = string.Empty;
    public string VoterPlayerId { get; set; } = string.Empty;
    public string VotedPlayerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Round Round { get; set; } = null!;
    public RoomPlayer VoterPlayer { get; set; } = null!;
    public RoomPlayer VotedPlayer { get; set; } = null!;
}
