using Microsoft.AspNetCore.SignalR;

namespace IdeonBack.API.Hubs;

/// <summary>
/// Hub de SignalR para gestionar eventos en tiempo real de PhotoClash
/// </summary>
public class PhotoClashHub : Hub
{
    /// <summary>
    /// Unirse a un grupo de sala específico
    /// </summary>
    public async Task JoinRoom(string roomCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomCode}");
        await Clients.Group($"room_{roomCode}").SendAsync("PlayerJoined", Context.ConnectionId);
    }

    /// <summary>
    /// Salir de un grupo de sala
    /// </summary>
    public async Task LeaveRoom(string roomCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomCode}");
        await Clients.Group($"room_{roomCode}").SendAsync("PlayerLeft", Context.ConnectionId);
    }

    /// <summary>
    /// Notificar que un jugador se unió
    /// </summary>
    public async Task NotifyPlayerJoined(string roomCode, string username, string userId)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("PlayerJoined", new
        {
            username,
            userId,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar actualización de sala
    /// </summary>
    public async Task NotifyRoomUpdated(string roomCode, object roomData)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("RoomUpdated", roomData);
    }

    /// <summary>
    /// Notificar inicio de partida
    /// </summary>
    public async Task NotifyGameStarted(string roomCode, object gameData)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("GameStarted", gameData);
    }

    /// <summary>
    /// Notificar inicio de ronda
    /// </summary>
    public async Task NotifyRoundStarted(string roomCode, int roundNumber, string promptPhrase, int secondsPerRound)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("RoundStarted", new
        {
            roundNumber,
            promptPhrase,
            secondsPerRound,
            startedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar tick del temporizador
    /// </summary>
    public async Task NotifyTimerTick(string roomCode, int remainingSeconds)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("TimerTick", new
        {
            remainingSeconds,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar que una foto fue subida
    /// </summary>
    public async Task NotifyPhotoUploaded(string roomCode, string playerId, string username)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("PhotoUploaded", new
        {
            playerId,
            username,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar inicio de votación
    /// </summary>
    public async Task NotifyVotingStarted(string roomCode, object photos)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("VotingStarted", new
        {
            photos,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar que se registró un voto
    /// </summary>
    public async Task NotifyVoteRegistered(string roomCode, string voterId)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("VoteRegistered", new
        {
            voterId,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar resultados de ronda
    /// </summary>
    public async Task NotifyRoundFinished(string roomCode, object scores, object leaderboard)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("RoundFinished", new
        {
            scores,
            leaderboard,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Notificar fin de partida
    /// </summary>
    public async Task NotifyMatchFinished(string roomCode, object matchResult)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("MatchFinished", new
        {
            matchResult,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Enviar mensaje de error
    /// </summary>
    public async Task NotifyError(string roomCode, string errorMessage)
    {
        await Clients.Group($"room_{roomCode}").SendAsync("Error", new
        {
            error = errorMessage,
            timestamp = DateTime.UtcNow
        });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
    }
}
