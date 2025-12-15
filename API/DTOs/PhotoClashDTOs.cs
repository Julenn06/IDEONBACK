namespace IdeonBack.API.DTOs;

// ============ PHOTOCLASH DTOs ============

public class CreateRoomRequest
{
    public Guid HostUserId { get; set; }
    public int RoundsTotal { get; set; }
    public int SecondsPerRound { get; set; }
    public bool NsfwAllowed { get; set; }
}

public class JoinRoomRequest
{
    public string Code { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}

public class StartGameRequest
{
    public Guid RoomId { get; set; }
    public string Language { get; set; } = "es";
}

public class UploadPhotoRequest
{
    public Guid RoundId { get; set; }
    public Guid PlayerId { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
}

public class VoteRequest
{
    public Guid RoundId { get; set; }
    public Guid VoterPlayerId { get; set; }
    public Guid VotedPlayerId { get; set; }
}

public class RoomResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RoundsTotal { get; set; }
    public int SecondsPerRound { get; set; }
    public bool NsfwAllowed { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RoomPlayerResponse> Players { get; set; } = new();
}

public class RoomPlayerResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int Score { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class RoundResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public int RoundNumber { get; set; }
    public string PromptPhrase { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}

public class RoundPhotoResponse
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerUsername { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}

public class VoteResponse
{
    public Guid Id { get; set; }
    public Guid RoundId { get; set; }
    public Guid VoterPlayerId { get; set; }
    public Guid VotedPlayerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MatchResultResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid WinnerPlayerId { get; set; }
    public string WinnerUsername { get; set; } = string.Empty;
    public int TotalRounds { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RoundScoresResponse
{
    public Dictionary<Guid, int> Scores { get; set; } = new();
}
