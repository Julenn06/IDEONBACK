namespace IdeonBack.Domain.Entities;

/// <summary>
/// Jugador dentro de una sala de juego
/// </summary>
public class RoomPlayer
{
    public string Id { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public int Score { get; set; } = 0;

    // Navegación
    public Room Room { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<RoundPhoto> RoundPhotos { get; set; } = new List<RoundPhoto>();
    public ICollection<Vote> VotesCast { get; set; } = new List<Vote>();
    public ICollection<Vote> VotesReceived { get; set; } = new List<Vote>();
}
