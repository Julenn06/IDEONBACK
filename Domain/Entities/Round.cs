namespace IdeonBack.Domain.Entities;

/// <summary>
/// Ronda dentro de una partida PhotoClash
/// </summary>
public class Round
{
    public string Id { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public string PromptPhrase { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    // Navegación
    public Room Room { get; set; } = null!;
    public ICollection<RoundPhoto> RoundPhotos { get; set; } = new List<RoundPhoto>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
