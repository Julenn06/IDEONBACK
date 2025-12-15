using IdeonBack.API.DTOs;
using IdeonBack.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdeonBack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoClashController : ControllerBase
{
    private readonly PhotoClashService _photoClashService;

    public PhotoClashController(PhotoClashService photoClashService)
    {
        _photoClashService = photoClashService;
    }

    /// <summary>
    /// Crear una nueva sala de juego
    /// </summary>
    [HttpPost("rooms")]
    public async Task<ActionResult<RoomResponse>> CreateRoom([FromBody] CreateRoomRequest request)
    {
        try
        {
            var room = await _photoClashService.CreateRoomAsync(
                request.HostUserId,
                request.RoundsTotal,
                request.SecondsPerRound,
                request.NsfwAllowed
            );

            var roomWithPlayers = await _photoClashService.GetRoomStateAsync(room.Id);
            return Ok(MapRoomToResponse(roomWithPlayers!));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Unirse a una sala mediante código
    /// </summary>
    [HttpPost("rooms/join")]
    public async Task<ActionResult<RoomResponse>> JoinRoom([FromBody] JoinRoomRequest request)
    {
        try
        {
            await _photoClashService.JoinRoomByCodeAsync(request.Code, request.UserId);
            var room = await _photoClashService.GetRoomStateAsync(
                (await _photoClashService.JoinRoomByCodeAsync(request.Code, request.UserId)).RoomId
            );
            
            return Ok(MapRoomToResponse(room!));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener estado de una sala
    /// </summary>
    [HttpGet("rooms/{roomId}")]
    public async Task<ActionResult<RoomResponse>> GetRoom(string roomId)
    {
        var room = await _photoClashService.GetRoomStateAsync(roomId);
        if (room == null)
            return NotFound(new { error = "Sala no encontrada" });

        return Ok(MapRoomToResponse(room));
    }

    /// <summary>
    /// Iniciar partida
    /// </summary>
    [HttpPost("rooms/start")]
    public async Task<ActionResult<RoomResponse>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            var room = await _photoClashService.StartGameAsync(request.RoomId, request.Language);
            var roomWithPlayers = await _photoClashService.GetRoomStateAsync(room.Id);
            
            return Ok(MapRoomToResponse(roomWithPlayers!));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener ronda actual
    /// </summary>
    [HttpGet("rooms/{roomId}/current-round")]
    public async Task<ActionResult<RoundResponse>> GetCurrentRound(string roomId)
    {
        var round = await _photoClashService.GetCurrentRoundAsync(roomId);
        if (round == null)
            return NotFound(new { error = "No hay ronda activa" });

        return Ok(new RoundResponse
        {
            Id = round.Id,
            RoomId = round.RoomId,
            RoundNumber = round.RoundNumber,
            PromptPhrase = round.PromptPhrase,
            StartedAt = round.StartedAt,
            FinishedAt = round.FinishedAt
        });
    }

    /// <summary>
    /// Subir foto a una ronda
    /// </summary>
    [HttpPost("photos")]
    public async Task<ActionResult<RoundPhotoResponse>> UploadPhoto([FromBody] UploadPhotoRequest request)
    {
        try
        {
            var roundPhoto = await _photoClashService.UploadPhotoAsync(
                request.RoundId,
                request.PlayerId,
                request.PhotoUrl
            );

            return Ok(new RoundPhotoResponse
            {
                Id = roundPhoto.Id,
                RoundId = roundPhoto.RoundId,
                PlayerId = roundPhoto.PlayerId,
                PhotoUrl = roundPhoto.PhotoUrl,
                UploadedAt = roundPhoto.UploadedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener fotos de una ronda
    /// </summary>
    [HttpGet("rounds/{roundId}/photos")]
    public async Task<ActionResult<List<RoundPhotoResponse>>> GetRoundPhotos(string roundId)
    {
        var photos = await _photoClashService.GetRoundPhotosAsync(roundId);
        
        var response = photos.Select(p => new RoundPhotoResponse
        {
            Id = p.Id,
            RoundId = p.RoundId,
            PlayerId = p.PlayerId,
            PlayerUsername = p.Player?.User?.Username ?? "",
            PhotoUrl = p.PhotoUrl,
            UploadedAt = p.UploadedAt
        }).ToList();

        return Ok(response);
    }

    /// <summary>
    /// Iniciar fase de votación
    /// </summary>
    [HttpPost("rooms/{roomId}/start-voting")]
    public async Task<ActionResult<RoomResponse>> StartVoting(string roomId)
    {
        try
        {
            var room = await _photoClashService.StartVotingPhaseAsync(roomId);
            var roomWithPlayers = await _photoClashService.GetRoomStateAsync(room.Id);
            
            return Ok(MapRoomToResponse(roomWithPlayers!));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Votar en una ronda
    /// </summary>
    [HttpPost("votes")]
    public async Task<ActionResult<VoteResponse>> Vote([FromBody] VoteRequest request)
    {
        try
        {
            var vote = await _photoClashService.VoteAsync(
                request.RoundId,
                request.VoterPlayerId,
                request.VotedPlayerId
            );

            return Ok(new VoteResponse
            {
                Id = vote.Id,
                RoundId = vote.RoundId,
                VoterPlayerId = vote.VoterPlayerId,
                VotedPlayerId = vote.VotedPlayerId,
                CreatedAt = vote.CreatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Calcular puntuaciones de una ronda
    /// </summary>
    [HttpPost("rounds/{roundId}/calculate-scores")]
    public async Task<ActionResult<RoundScoresResponse>> CalculateScores(string roundId)
    {
        var scores = await _photoClashService.CalculateRoundScoresAsync(roundId);
        return Ok(new RoundScoresResponse { Scores = scores });
    }

    /// <summary>
    /// Finalizar ronda
    /// </summary>
    [HttpPost("rounds/{roundId}/finish")]
    public async Task<ActionResult<RoundResponse>> FinishRound(string roundId)
    {
        try
        {
            var round = await _photoClashService.FinishRoundAsync(roundId);
            
            return Ok(new RoundResponse
            {
                Id = round.Id,
                RoomId = round.RoomId,
                RoundNumber = round.RoundNumber,
                PromptPhrase = round.PromptPhrase,
                StartedAt = round.StartedAt,
                FinishedAt = round.FinishedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Iniciar siguiente ronda
    /// </summary>
    [HttpPost("rooms/{roomId}/next-round")]
    public async Task<ActionResult<RoundResponse>> NextRound(string roomId)
    {
        try
        {
            var round = await _photoClashService.StartNextRoundAsync(roomId);
            if (round == null)
                return Ok(new { message = "No hay más rondas", finished = true });

            return Ok(new RoundResponse
            {
                Id = round.Id,
                RoomId = round.RoomId,
                RoundNumber = round.RoundNumber,
                PromptPhrase = round.PromptPhrase,
                StartedAt = round.StartedAt,
                FinishedAt = round.FinishedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Finalizar partida
    /// </summary>
    [HttpPost("rooms/{roomId}/finish")]
    public async Task<ActionResult<MatchResultResponse>> FinishGame(string roomId)
    {
        try
        {
            var result = await _photoClashService.FinishGameAsync(roomId);
            
            return Ok(new MatchResultResponse
            {
                Id = result.Id,
                RoomId = result.RoomId,
                WinnerPlayerId = result.WinnerPlayerId,
                WinnerUsername = result.WinnerPlayer?.User?.Username ?? "",
                TotalRounds = result.TotalRounds,
                CreatedAt = result.CreatedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private RoomResponse MapRoomToResponse(Domain.Entities.Room room)
    {
        return new RoomResponse
        {
            Id = room.Id,
            Code = room.Code,
            Status = room.Status.ToString(),
            RoundsTotal = room.RoundsTotal,
            SecondsPerRound = room.SecondsPerRound,
            NsfwAllowed = room.NsfwAllowed,
            CreatedAt = room.CreatedAt,
            Players = room.Players.Select(p => new RoomPlayerResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                Username = p.User?.Username ?? "",
                AvatarUrl = p.User?.AvatarUrl,
                Score = p.Score,
                JoinedAt = p.JoinedAt
            }).ToList()
        };
    }
}
