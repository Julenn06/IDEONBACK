using IdeonBack.Domain.Entities;
using IdeonBack.Domain.Enums;
using IdeonBack.Domain.Interfaces;

namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio principal para gestionar partidas PhotoClash (PvP)
/// </summary>
public class PhotoClashService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomPlayerRepository _roomPlayerRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly IRoundPhotoRepository _roundPhotoRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IMatchResultRepository _matchResultRepository;
    private readonly IUserRepository _userRepository;
    private readonly PhraseGeneratorService _phraseGenerator;
    private readonly RoomCodeGeneratorService _codeGenerator;

    public PhotoClashService(
        IRoomRepository roomRepository,
        IRoomPlayerRepository roomPlayerRepository,
        IRoundRepository roundRepository,
        IRoundPhotoRepository roundPhotoRepository,
        IVoteRepository voteRepository,
        IMatchResultRepository matchResultRepository,
        IUserRepository userRepository,
        PhraseGeneratorService phraseGenerator,
        RoomCodeGeneratorService codeGenerator)
    {
        _roomRepository = roomRepository;
        _roomPlayerRepository = roomPlayerRepository;
        _roundRepository = roundRepository;
        _roundPhotoRepository = roundPhotoRepository;
        _voteRepository = voteRepository;
        _matchResultRepository = matchResultRepository;
        _userRepository = userRepository;
        _phraseGenerator = phraseGenerator;
        _codeGenerator = codeGenerator;
    }

    /// <summary>
    /// Crea una nueva sala de juego
    /// </summary>
    public async Task<Room> CreateRoomAsync(string hostUserId, int roundsTotal, int secondsPerRound, bool nsfwAllowed)
    {
        // Validaciones
        if (roundsTotal <= 0 || roundsTotal > 20)
            throw new ArgumentException("El número de rondas debe estar entre 1 y 20");

        if (secondsPerRound <= 0 || secondsPerRound > 300)
            throw new ArgumentException("Los segundos por ronda deben estar entre 1 y 300");

        // Generar código único
        string code;
        do
        {
            code = _codeGenerator.GenerateCode(6);
        } while (await _roomRepository.GetByCodeAsync(code) != null);

        var room = new Room
        {
            Code = code,
            Status = RoomStatus.Waiting,
            RoundsTotal = roundsTotal,
            SecondsPerRound = secondsPerRound,
            NsfwAllowed = nsfwAllowed
        };

        room = await _roomRepository.CreateAsync(room);

        // Añadir al host como primer jugador
        await JoinRoomAsync(room.Id, hostUserId);

        return room;
    }

    /// <summary>
    /// Unirse a una sala existente mediante código
    /// </summary>
    public async Task<RoomPlayer> JoinRoomAsync(string roomId, string userId)
    {
        var room = await _roomRepository.GetByIdWithPlayersAsync(roomId);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada");

        if (room.Status != RoomStatus.Waiting)
            throw new InvalidOperationException("No se puede unir a una sala en curso");

        // Verificar si ya está en la sala
        var existingPlayer = await _roomPlayerRepository.GetByRoomAndUserAsync(roomId, userId);
        if (existingPlayer != null)
            return existingPlayer;

        // Límite de jugadores (opcional, ajustar según necesidad)
        var playerCount = await _roomPlayerRepository.GetPlayerCountByRoomIdAsync(roomId);
        if (playerCount >= 8)
            throw new InvalidOperationException("La sala está llena");

        var roomPlayer = new RoomPlayer
        {
            RoomId = roomId,
            UserId = userId
        };

        return await _roomPlayerRepository.CreateAsync(roomPlayer);
    }

    /// <summary>
    /// Unirse a una sala mediante código
    /// </summary>
    public async Task<RoomPlayer> JoinRoomByCodeAsync(string code, string userId)
    {
        var room = await _roomRepository.GetByCodeAsync(code);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada con ese código");

        return await JoinRoomAsync(room.Id, userId);
    }

    /// <summary>
    /// Iniciar la partida y generar todas las rondas
    /// </summary>
    public async Task<Room> StartGameAsync(string roomId, string language = "es")
    {
        var room = await _roomRepository.GetByIdWithPlayersAsync(roomId);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada");

        if (room.Status != RoomStatus.Waiting)
            throw new InvalidOperationException("La partida ya ha comenzado");

        var playerCount = room.Players.Count;
        if (playerCount < 2)
            throw new InvalidOperationException("Se necesitan al menos 2 jugadores para comenzar");

        // Generar frases únicas para todas las rondas
        var phrases = _phraseGenerator.GenerateUniquePhrases(
            room.RoundsTotal, 
            language, 
            room.NsfwAllowed
        );

        // Crear todas las rondas
        for (int i = 0; i < room.RoundsTotal; i++)
        {
            var round = new Round
            {
                RoomId = roomId,
                RoundNumber = i + 1,
                PromptPhrase = phrases[i]
            };
            await _roundRepository.CreateAsync(round);
        }

        // Iniciar primera ronda
        var firstRound = await _roundRepository.GetByRoomIdAsync(roomId);
        var first = firstRound.First();
        first.StartedAt = DateTime.UtcNow;
        await _roundRepository.UpdateAsync(first);

        room.Status = RoomStatus.Playing;
        return await _roomRepository.UpdateAsync(room);
    }

    /// <summary>
    /// Subir foto a la ronda actual
    /// </summary>
    public async Task<RoundPhoto> UploadPhotoAsync(string roundId, string playerId, string photoUrl)
    {
        var round = await _roundRepository.GetByIdAsync(roundId);
        if (round == null)
            throw new InvalidOperationException("Ronda no encontrada");

        if (round.FinishedAt != null)
            throw new InvalidOperationException("La ronda ya ha terminado");

        // Verificar que el jugador no haya subido ya una foto
        var existingPhoto = await _roundPhotoRepository.GetByRoundAndPlayerAsync(roundId, playerId);
        if (existingPhoto != null)
            throw new InvalidOperationException("Ya has subido una foto para esta ronda");

        var roundPhoto = new RoundPhoto
        {
            RoundId = roundId,
            PlayerId = playerId,
            PhotoUrl = photoUrl
        };

        return await _roundPhotoRepository.CreateAsync(roundPhoto);
    }

    /// <summary>
    /// Iniciar fase de votación para una ronda
    /// </summary>
    public async Task<Room> StartVotingPhaseAsync(string roomId)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada");

        room.Status = RoomStatus.Voting;
        return await _roomRepository.UpdateAsync(room);
    }

    /// <summary>
    /// Registrar un voto
    /// </summary>
    public async Task<Vote> VoteAsync(string roundId, string voterPlayerId, string votedPlayerId)
    {
        // Validar que no se vote a sí mismo
        if (voterPlayerId == votedPlayerId)
            throw new InvalidOperationException("No puedes votarte a ti mismo");

        var round = await _roundRepository.GetByIdAsync(roundId);
        if (round == null)
            throw new InvalidOperationException("Ronda no encontrada");

        // Verificar que el votante no haya votado ya
        var existingVote = await _voteRepository.GetByRoundAndVoterAsync(roundId, voterPlayerId);
        if (existingVote != null)
            throw new InvalidOperationException("Ya has votado en esta ronda");

        var vote = new Vote
        {
            RoundId = roundId,
            VoterPlayerId = voterPlayerId,
            VotedPlayerId = votedPlayerId
        };

        return await _voteRepository.CreateAsync(vote);
    }

    /// <summary>
    /// Calcular puntuaciones de una ronda y actualizar scores
    /// </summary>
    public async Task<Dictionary<string, int>> CalculateRoundScoresAsync(string roundId)
    {
        var votes = await _voteRepository.GetVotesByPlayerInRoundAsync(roundId);
        var scores = new Dictionary<string, int>();

        if (votes.Count == 0)
            return scores;

        // Ordenar por votos
        var sortedVotes = votes.OrderByDescending(v => v.Value).ToList();

        // Asignar puntos: 1º = 3 puntos, 2º = 1 punto
        if (sortedVotes.Count > 0)
        {
            var firstPlace = sortedVotes[0];
            scores[firstPlace.Key] = 3;

            // Actualizar score del jugador
            var player = await _roomPlayerRepository.GetByIdAsync(firstPlace.Key);
            if (player != null)
            {
                player.Score += 3;
                await _roomPlayerRepository.UpdateAsync(player);
            }
        }

        if (sortedVotes.Count > 1 && sortedVotes[1].Value > 0)
        {
            var secondPlace = sortedVotes[1];
            scores[secondPlace.Key] = 1;

            var player = await _roomPlayerRepository.GetByIdAsync(secondPlace.Key);
            if (player != null)
            {
                player.Score += 1;
                await _roomPlayerRepository.UpdateAsync(player);
            }
        }

        return scores;
    }

    /// <summary>
    /// Finalizar una ronda
    /// </summary>
    public async Task<Round> FinishRoundAsync(string roundId)
    {
        var round = await _roundRepository.GetByIdAsync(roundId);
        if (round == null)
            throw new InvalidOperationException("Ronda no encontrada");

        round.FinishedAt = DateTime.UtcNow;
        return await _roundRepository.UpdateAsync(round);
    }

    /// <summary>
    /// Iniciar la siguiente ronda
    /// </summary>
    public async Task<Round?> StartNextRoundAsync(string roomId)
    {
        var room = await _roomRepository.GetByIdWithPlayersAsync(roomId);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada");

        var rounds = await _roundRepository.GetByRoomIdAsync(roomId);
        var currentRoundNumber = rounds.Count(r => r.FinishedAt != null);

        if (currentRoundNumber >= room.RoundsTotal)
            return null; // No hay más rondas

        var nextRound = rounds.FirstOrDefault(r => r.RoundNumber == currentRoundNumber + 1);
        if (nextRound != null)
        {
            nextRound.StartedAt = DateTime.UtcNow;
            await _roundRepository.UpdateAsync(nextRound);
            
            room.Status = RoomStatus.Playing;
            await _roomRepository.UpdateAsync(room);
        }

        return nextRound;
    }

    /// <summary>
    /// Finalizar partida y determinar ganador
    /// </summary>
    public async Task<MatchResult> FinishGameAsync(string roomId)
    {
        var room = await _roomRepository.GetByIdWithPlayersAsync(roomId);
        if (room == null)
            throw new InvalidOperationException("Sala no encontrada");

        // Encontrar jugador con mayor puntuación
        var winner = room.Players.OrderByDescending(p => p.Score).First();

        var matchResult = new MatchResult
        {
            RoomId = roomId,
            WinnerPlayerId = winner.Id,
            TotalRounds = room.RoundsTotal
        };

        matchResult = await _matchResultRepository.CreateAsync(matchResult);

        room.Status = RoomStatus.Finished;
        await _roomRepository.UpdateAsync(room);

        return matchResult;
    }

    /// <summary>
    /// Obtener estado completo de una sala
    /// </summary>
    public async Task<Room?> GetRoomStateAsync(string roomId)
    {
        return await _roomRepository.GetByIdWithPlayersAsync(roomId);
    }

    /// <summary>
    /// Obtener ronda actual de una sala
    /// </summary>
    public async Task<Round?> GetCurrentRoundAsync(string roomId)
    {
        return await _roundRepository.GetCurrentRoundByRoomIdAsync(roomId);
    }

    /// <summary>
    /// Obtener fotos de una ronda
    /// </summary>
    public async Task<IEnumerable<RoundPhoto>> GetRoundPhotosAsync(string roundId)
    {
        return await _roundPhotoRepository.GetByRoundIdAsync(roundId);
    }

    /// <summary>
    /// Obtener votos de una ronda
    /// </summary>
    public async Task<IEnumerable<Vote>> GetRoundVotesAsync(string roundId)
    {
        return await _voteRepository.GetByRoundIdAsync(roundId);
    }
}
