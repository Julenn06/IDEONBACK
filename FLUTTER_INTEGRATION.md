# Integración Flutter - IDEON Backend

## 📱 Configuración en Flutter

### 1. Dependencias necesarias

Agregar a `pubspec.yaml`:

```yaml
dependencies:
  http: ^1.1.0
  signalr_netcore: ^1.3.3
  shared_preferences: ^2.2.2
```

### 2. Configuración de la URL base

```dart
class ApiConfig {
  static const String baseUrl = 'http://10.0.2.2:5000'; // Android Emulator
  // static const String baseUrl = 'http://localhost:5000'; // iOS Simulator
  // static const String baseUrl = 'http://192.168.1.X:5000'; // Dispositivo físico
  
  static const String signalRHub = '$baseUrl/hubs/photoclash';
}
```

## 🔌 Servicio HTTP

### Crear servicio base

```dart
import 'package:http/http.dart' as http;
import 'dart:convert';

class ApiService {
  final String baseUrl = ApiConfig.baseUrl;
  
  Future<Map<String, dynamic>> get(String endpoint) async {
    final response = await http.get(
      Uri.parse('$baseUrl$endpoint'),
      headers: {'Content-Type': 'application/json'},
    );
    
    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else {
      throw Exception('Error: ${response.statusCode}');
    }
  }
  
  Future<Map<String, dynamic>> post(
    String endpoint,
    Map<String, dynamic> body,
  ) async {
    final response = await http.post(
      Uri.parse('$baseUrl$endpoint'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(body),
    );
    
    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else {
      final error = json.decode(response.body);
      throw Exception(error['error'] ?? 'Error desconocido');
    }
  }
}
```

## 👤 Servicio de Usuario

```dart
class UserService {
  final ApiService _api = ApiService();
  
  Future<User> createUser(String username, String? avatarUrl) async {
    final response = await _api.post('/api/users', {
      'username': username,
      'avatarUrl': avatarUrl,
    });
    
    return User.fromJson(response);
  }
  
  Future<User> getUser(String userId) async {
    final response = await _api.get('/api/users/$userId');
    return User.fromJson(response);
  }
  
  Future<AppSettings> updateSettings({
    required String userId,
    bool? darkMode,
    bool? notifications,
    String? language,
  }) async {
    final response = await _api.put('/api/users/$userId/settings', {
      if (darkMode != null) 'darkMode': darkMode,
      if (notifications != null) 'notifications': notifications,
      if (language != null) 'language': language,
    });
    
    return AppSettings.fromJson(response);
  }
}
```

## 🎮 Servicio PhotoClash

```dart
class PhotoClashService {
  final ApiService _api = ApiService();
  
  // Crear sala
  Future<Room> createRoom({
    required String hostUserId,
    required int roundsTotal,
    required int secondsPerRound,
    bool nsfwAllowed = false,
  }) async {
    final response = await _api.post('/api/photoclash/rooms', {
      'hostUserId': hostUserId,
      'roundsTotal': roundsTotal,
      'secondsPerRound': secondsPerRound,
      'nsfwAllowed': nsfwAllowed,
    });
    
    return Room.fromJson(response);
  }
  
  // Unirse a sala
  Future<Room> joinRoom(String code, String userId) async {
    final response = await _api.post('/api/photoclash/rooms/join', {
      'code': code,
      'userId': userId,
    });
    
    return Room.fromJson(response);
  }
  
  // Iniciar partida
  Future<Room> startGame(String roomId, String language) async {
    final response = await _api.post('/api/photoclash/rooms/start', {
      'roomId': roomId,
      'language': language,
    });
    
    return Room.fromJson(response);
  }
  
  // Obtener ronda actual
  Future<Round?> getCurrentRound(String roomId) async {
    try {
      final response = await _api.get('/api/photoclash/rooms/$roomId/current-round');
      return Round.fromJson(response);
    } catch (e) {
      return null;
    }
  }
  
  // Subir foto
  Future<RoundPhoto> uploadPhoto({
    required String roundId,
    required String playerId,
    required String photoUrl,
  }) async {
    final response = await _api.post('/api/photoclash/photos', {
      'roundId': roundId,
      'playerId': playerId,
      'photoUrl': photoUrl,
    });
    
    return RoundPhoto.fromJson(response);
  }
  
  // Votar
  Future<Vote> vote({
    required String roundId,
    required String voterPlayerId,
    required String votedPlayerId,
  }) async {
    final response = await _api.post('/api/photoclash/votes', {
      'roundId': roundId,
      'voterPlayerId': voterPlayerId,
      'votedPlayerId': votedPlayerId,
    });
    
    return Vote.fromJson(response);
  }
  
  // Finalizar partida
  Future<MatchResult> finishGame(String roomId) async {
    final response = await _api.post('/api/photoclash/rooms/$roomId/finish', {});
    return MatchResult.fromJson(response);
  }
}
```

## 📡 SignalR Hub Service

```dart
import 'package:signalr_netcore/signalr_client.dart';

class SignalRService {
  HubConnection? _hubConnection;
  final String hubUrl = ApiConfig.signalRHub;
  
  // Callbacks
  Function(String username, String userId)? onPlayerJoined;
  Function(int roundNumber, String phrase, int seconds)? onRoundStarted;
  Function(int remaining)? onTimerTick;
  Function(String playerId, String username)? onPhotoUploaded;
  Function(dynamic photos)? onVotingStarted;
  Function(dynamic scores)? onRoundFinished;
  Function(dynamic result)? onMatchFinished;
  
  Future<void> connect() async {
    _hubConnection = HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();
    
    // Registrar eventos
    _hubConnection!.on('PlayerJoined', _handlePlayerJoined);
    _hubConnection!.on('RoundStarted', _handleRoundStarted);
    _hubConnection!.on('TimerTick', _handleTimerTick);
    _hubConnection!.on('PhotoUploaded', _handlePhotoUploaded);
    _hubConnection!.on('VotingStarted', _handleVotingStarted);
    _hubConnection!.on('RoundFinished', _handleRoundFinished);
    _hubConnection!.on('MatchFinished', _handleMatchFinished);
    
    await _hubConnection!.start();
    print('SignalR conectado');
  }
  
  Future<void> joinRoom(String roomCode) async {
    await _hubConnection!.invoke('JoinRoom', args: [roomCode]);
  }
  
  Future<void> notifyPlayerJoined(String roomCode, String username, String userId) async {
    await _hubConnection!.invoke('NotifyPlayerJoined', args: [
      roomCode,
      username,
      userId
    ]);
  }
  
  Future<void> notifyRoundStarted(
    String roomCode,
    int roundNumber,
    String promptPhrase,
    int secondsPerRound,
  ) async {
    await _hubConnection!.invoke('NotifyRoundStarted', args: [
      roomCode,
      roundNumber,
      promptPhrase,
      secondsPerRound,
    ]);
  }
  
  Future<void> notifyPhotoUploaded(
    String roomCode,
    String playerId,
    String username,
  ) async {
    await _hubConnection!.invoke('NotifyPhotoUploaded', args: [
      roomCode,
      playerId,
      username,
    ]);
  }
  
  Future<void> notifyVotingStarted(String roomCode, dynamic photos) async {
    await _hubConnection!.invoke('NotifyVotingStarted', args: [
      roomCode,
      photos,
    ]);
  }
  
  void _handlePlayerJoined(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onPlayerJoined?.call(data['username'], data['userId']);
    }
  }
  
  void _handleRoundStarted(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onRoundStarted?.call(
        data['roundNumber'],
        data['promptPhrase'],
        data['secondsPerRound'],
      );
    }
  }
  
  void _handleTimerTick(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onTimerTick?.call(data['remainingSeconds']);
    }
  }
  
  void _handlePhotoUploaded(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onPhotoUploaded?.call(data['playerId'], data['username']);
    }
  }
  
  void _handleVotingStarted(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onVotingStarted?.call(data['photos']);
    }
  }
  
  void _handleRoundFinished(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onRoundFinished?.call(data['scores']);
    }
  }
  
  void _handleMatchFinished(List<Object>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      onMatchFinished?.call(data['matchResult']);
    }
  }
  
  Future<void> disconnect() async {
    await _hubConnection?.stop();
  }
}
```

## 📊 Modelos de Datos

```dart
class User {
  final String id;
  final String username;
  final String? avatarUrl;
  final DateTime createdAt;
  final DateTime? lastLogin;
  
  User({
    required this.id,
    required this.username,
    this.avatarUrl,
    required this.createdAt,
    this.lastLogin,
  });
  
  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      username: json['username'],
      avatarUrl: json['avatarUrl'],
      createdAt: DateTime.parse(json['createdAt']),
      lastLogin: json['lastLogin'] != null 
          ? DateTime.parse(json['lastLogin']) 
          : null,
    );
  }
}

class Room {
  final String id;
  final String code;
  final String status;
  final int roundsTotal;
  final int secondsPerRound;
  final bool nsfwAllowed;
  final List<RoomPlayer> players;
  
  Room({
    required this.id,
    required this.code,
    required this.status,
    required this.roundsTotal,
    required this.secondsPerRound,
    required this.nsfwAllowed,
    required this.players,
  });
  
  factory Room.fromJson(Map<String, dynamic> json) {
    return Room(
      id: json['id'],
      code: json['code'],
      status: json['status'],
      roundsTotal: json['roundsTotal'],
      secondsPerRound: json['secondsPerRound'],
      nsfwAllowed: json['nsfwAllowed'],
      players: (json['players'] as List)
          .map((p) => RoomPlayer.fromJson(p))
          .toList(),
    );
  }
}

class Round {
  final String id;
  final String roomId;
  final int roundNumber;
  final String promptPhrase;
  final DateTime? startedAt;
  final DateTime? finishedAt;
  
  Round({
    required this.id,
    required this.roomId,
    required this.roundNumber,
    required this.promptPhrase,
    this.startedAt,
    this.finishedAt,
  });
  
  factory Round.fromJson(Map<String, dynamic> json) {
    return Round(
      id: json['id'],
      roomId: json['roomId'],
      roundNumber: json['roundNumber'],
      promptPhrase: json['promptPhrase'],
      startedAt: json['startedAt'] != null 
          ? DateTime.parse(json['startedAt']) 
          : null,
      finishedAt: json['finishedAt'] != null 
          ? DateTime.parse(json['finishedAt']) 
          : null,
    );
  }
}
```

## 🎯 Ejemplo de Uso en Screen

```dart
class PhotoClashGameScreen extends StatefulWidget {
  final String roomCode;
  
  @override
  _PhotoClashGameScreenState createState() => _PhotoClashGameScreenState();
}

class _PhotoClashGameScreenState extends State<PhotoClashGameScreen> {
  final PhotoClashService _gameService = PhotoClashService();
  final SignalRService _signalR = SignalRService();
  
  Room? _room;
  Round? _currentRound;
  int _remainingSeconds = 0;
  
  @override
  void initState() {
    super.initState();
    _initializeGame();
  }
  
  Future<void> _initializeGame() async {
    // Conectar SignalR
    await _signalR.connect();
    await _signalR.joinRoom(widget.roomCode);
    
    // Configurar callbacks
    _signalR.onRoundStarted = _onRoundStarted;
    _signalR.onTimerTick = _onTimerTick;
    _signalR.onPhotoUploaded = _onPhotoUploaded;
    _signalR.onVotingStarted = _onVotingStarted;
    
    // Cargar estado de la sala
    // Implementar lógica...
  }
  
  void _onRoundStarted(int roundNumber, String phrase, int seconds) {
    setState(() {
      _currentRound = Round(
        id: '',
        roomId: _room!.id,
        roundNumber: roundNumber,
        promptPhrase: phrase,
      );
      _remainingSeconds = seconds;
    });
  }
  
  void _onTimerTick(int remaining) {
    setState(() {
      _remainingSeconds = remaining;
    });
  }
  
  // Más callbacks...
  
  @override
  void dispose() {
    _signalR.disconnect();
    super.dispose();
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('PhotoClash - ${widget.roomCode}')),
      body: Column(
        children: [
          if (_currentRound != null)
            Text(
              _currentRound!.promptPhrase,
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
          Text('Tiempo restante: $_remainingSeconds s'),
          // Más UI...
        ],
      ),
    );
  }
}
```

## 🧹 PhotoSweep Service

```dart
class PhotoSweepService {
  final ApiService _api = ApiService();
  
  Future<Photo> registerPhoto(String userId, String uri, DateTime? dateTaken) async {
    final response = await _api.post('/api/photosweep/photos', {
      'userId': userId,
      'uri': uri,
      'dateTaken': dateTaken?.toIso8601String(),
    });
    
    return Photo.fromJson(response);
  }
  
  Future<List<Photo>> getUnreviewedPhotos(String userId) async {
    final response = await _api.get('/api/photosweep/users/$userId/unreviewed');
    return (response as List).map((p) => Photo.fromJson(p)).toList();
  }
  
  Future<Photo> keepPhoto(String photoId) async {
    final response = await _api.post('/api/photosweep/photos/$photoId/keep', {});
    return Photo.fromJson(response);
  }
  
  Future<Photo> deletePhoto(String photoId) async {
    final response = await _api.post('/api/photosweep/photos/$photoId/delete', {});
    return Photo.fromJson(response);
  }
  
  Future<PhotoStats> getStats(String userId) async {
    final response = await _api.get('/api/photosweep/users/$userId/stats');
    return PhotoStats.fromJson(response);
  }
}
```

---

**✅ Backend y Flutter completamente integrados!**
