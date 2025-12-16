using IdeonBack.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio para gestionar temporizadores de rondas
/// </summary>
public class TimerService
{
    private readonly IHubContext<PhotoClashHub> _hubContext;
    private readonly Dictionary<string, Timer> _timers = new();
    private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();

    public TimerService(IHubContext<PhotoClashHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Iniciar temporizador para una sala
    /// </summary>
    public void StartTimer(string roomCode, int durationSeconds, Action onComplete)
    {
        // Cancelar temporizador existente si lo hay
        StopTimer(roomCode);

        var cts = new CancellationTokenSource();
        _cancellationTokens[roomCode] = cts;

        Task.Run(async () =>
        {
            var remainingSeconds = durationSeconds;

            while (remainingSeconds > 0 && !cts.Token.IsCancellationRequested)
            {
                await _hubContext.Clients.Group($"room_{roomCode}")
                    .SendAsync("TimerTick", new
                    {
                        remainingSeconds,
                        timestamp = DateTime.UtcNow
                    }, cts.Token);

                await Task.Delay(1000, cts.Token);
                remainingSeconds--;
            }

            if (!cts.Token.IsCancellationRequested)
            {
                await _hubContext.Clients.Group($"room_{roomCode}")
                    .SendAsync("TimerExpired", new
                    {
                        timestamp = DateTime.UtcNow
                    }, cts.Token);

                onComplete?.Invoke();
            }
        }, cts.Token);
    }

    /// <summary>
    /// Detener temporizador de una sala
    /// </summary>
    public void StopTimer(string roomCode)
    {
        if (_cancellationTokens.TryGetValue(roomCode, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _cancellationTokens.Remove(roomCode);
        }

        if (_timers.TryGetValue(roomCode, out var timer))
        {
            timer.Dispose();
            _timers.Remove(roomCode);
        }
    }

    /// <summary>
    /// Pausar temporizador
    /// </summary>
    public void PauseTimer(string roomCode)
    {
        if (_cancellationTokens.TryGetValue(roomCode, out var cts))
        {
            cts.Cancel();
        }
    }
}
