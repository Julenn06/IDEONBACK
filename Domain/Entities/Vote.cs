namespace IdeonBack.Domain.Entities;

/// <summary>
/// Voto emitido por un jugador hacia otro en una ronda
/// </summary>
public class Vote
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public Guid VoterPlayerId { get; set; }
    public Guid VotedPlayerId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Round Round { get; set; } = null!;
    public RoomPlayer VoterPlayer { get; set; } = null!;
    public RoomPlayer VotedPlayer { get; set; } = null!;
}
