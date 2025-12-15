namespace IdeonBack.Domain.Entities;

/// <summary>
/// Jugador dentro de una sala de juego
/// </summary>
public class RoomPlayer
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public int Score { get; set; } = 0;

    // Navegación
    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<RoundPhoto> RoundPhotos { get; set; } = new List<RoundPhoto>();
    public ICollection<Vote> VotesCast { get; set; } = new List<Vote>();
    public ICollection<Vote> VotesReceived { get; set; } = new List<Vote>();
}
