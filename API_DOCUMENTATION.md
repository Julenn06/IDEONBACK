# 🎮 IDEON Backend - Documentación API

## 📋 Descripción General

Backend completo para **IDEON - Clean & Clash**, una aplicación móvil Android desarrollada en Flutter que combina:
- **PhotoSweep**: Modo de limpieza inteligente de fotos
- **PhotoClash**: Modo social PVP basado en fotos y rondas

## 🏗️ Arquitectura

El backend sigue los principios de **Clean Architecture**:

```
ideonBack/
├── API/                      # Capa de presentación
│   ├── Controllers/         # Endpoints REST
│   ├── DTOs/               # Objetos de transferencia
│   ├── Hubs/               # SignalR para tiempo real
│   └── Middleware/         # Middleware personalizado
├── Application/             # Lógica de negocio
│   └── Services/           # Servicios de aplicación
├── Domain/                  # Núcleo del dominio
│   ├── Entities/           # Entidades del modelo
│   ├── Enums/              # Enumeraciones
│   └── Interfaces/         # Contratos de repositorios
└── Infrastructure/          # Implementación técnica
    ├── Data/               # DbContext
    └── Repositories/       # Implementación de repositorios
```

## 🛠️ Tecnologías

- **ASP.NET Core 8.0**
- **PostgreSQL** (CrateDB compatible)
- **Entity Framework Core 8.0**
- **SignalR** para comunicación en tiempo real
- **Swagger** para documentación interactiva
- **Npgsql** para PostgreSQL

## 🚀 Inicio Rápido

### Prerrequisitos

- .NET 8.0 SDK o superior
- PostgreSQL o CrateDB
- Editor de código (Visual Studio, VS Code, Rider)

### Instalación

1. **Clonar el repositorio**
```bash
cd ideonBack
```

2. **Configurar la base de datos**

Editar `appsettings.json` con tu cadena de conexión:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ideon;Username=postgres;Password=yourpassword"
  }
}
```

3. **Ejecutar las migraciones** (si es necesario)
```bash
dotnet ef database update
```

O ejecutar el script SQL directamente:
```bash
psql -U postgres -d ideon -f bd.sql
```

4. **Ejecutar el proyecto**
```bash
dotnet run
```

El servidor estará disponible en:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000

## 📡 Endpoints API

### 👤 Usuarios

#### Crear Usuario
```http
POST /api/users
Content-Type: application/json

{
  "username": "jugador123",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

#### Obtener Usuario
```http
GET /api/users/{userId}
```

#### Obtener Usuario por Nombre
```http
GET /api/users/username/{username}
```

#### Actualizar Configuración
```http
PUT /api/users/{userId}/settings
Content-Type: application/json

{
  "darkMode": true,
  "notifications": true,
  "language": "es"
}
```

---

### 🎮 PhotoClash (Modo PVP)

#### Crear Sala
```http
POST /api/photoclash/rooms
Content-Type: application/json

{
  "hostUserId": "user-uuid",
  "roundsTotal": 5,
  "secondsPerRound": 60,
  "nsfwAllowed": false
}
```

**Respuesta:**
```json
{
  "id": "room-uuid",
  "code": "ABC123",
  "status": "Waiting",
  "roundsTotal": 5,
  "secondsPerRound": 60,
  "nsfwAllowed": false,
  "createdAt": "2025-12-16T10:00:00Z",
  "players": [
    {
      "id": "player-uuid",
      "userId": "user-uuid",
      "username": "jugador123",
      "avatarUrl": "https://example.com/avatar.jpg",
      "score": 0,
      "joinedAt": "2025-12-16T10:00:00Z"
    }
  ]
}
```

#### Unirse a Sala
```http
POST /api/photoclash/rooms/join
Content-Type: application/json

{
  "code": "ABC123",
  "userId": "user-uuid"
}
```

#### Obtener Estado de Sala
```http
GET /api/photoclash/rooms/{roomId}
```

#### Iniciar Partida
```http
POST /api/photoclash/rooms/start
Content-Type: application/json

{
  "roomId": "room-uuid",
  "language": "es"
}
```

#### Obtener Ronda Actual
```http
GET /api/photoclash/rooms/{roomId}/current-round
```

**Respuesta:**
```json
{
  "id": "round-uuid",
  "roomId": "room-uuid",
  "roundNumber": 1,
  "promptPhrase": "Algo que te haga reír",
  "startedAt": "2025-12-16T10:05:00Z",
  "finishedAt": null
}
```

#### Subir Foto
```http
POST /api/photoclash/photos
Content-Type: application/json

{
  "roundId": "round-uuid",
  "playerId": "player-uuid",
  "photoUrl": "https://example.com/photo.jpg"
}
```

#### Obtener Fotos de Ronda
```http
GET /api/photoclash/rounds/{roundId}/photos
```

#### Iniciar Votación
```http
POST /api/photoclash/rooms/{roomId}/start-voting
```

#### Votar
```http
POST /api/photoclash/votes
Content-Type: application/json

{
  "roundId": "round-uuid",
  "voterPlayerId": "player-uuid-1",
  "votedPlayerId": "player-uuid-2"
}
```

**Validaciones:**
- No puedes votarte a ti mismo
- Solo puedes votar una vez por ronda

#### Calcular Puntuaciones
```http
POST /api/photoclash/rounds/{roundId}/calculate-scores
```

**Sistema de puntuación:**
- 1º lugar: 3 puntos
- 2º lugar: 1 punto

#### Finalizar Ronda
```http
POST /api/photoclash/rounds/{roundId}/finish
```

#### Siguiente Ronda
```http
POST /api/photoclash/rooms/{roomId}/next-round
```

#### Finalizar Partida
```http
POST /api/photoclash/rooms/{roomId}/finish
```

**Respuesta:**
```json
{
  "id": "match-uuid",
  "roomId": "room-uuid",
  "winnerPlayerId": "player-uuid",
  "winnerUsername": "jugador123",
  "totalRounds": 5,
  "createdAt": "2025-12-16T10:30:00Z"
}
```

---

### 🧹 PhotoSweep (Limpieza de Fotos)

#### Registrar Foto
```http
POST /api/photosweep/photos
Content-Type: application/json

{
  "userId": "user-uuid",
  "uri": "file:///storage/photo.jpg",
  "dateTaken": "2025-12-16T10:00:00Z"
}
```

#### Obtener Fotos Sin Revisar
```http
GET /api/photosweep/users/{userId}/unreviewed
```

#### Marcar como Mantenida
```http
POST /api/photosweep/photos/{photoId}/keep
```

#### Marcar como Eliminada
```http
POST /api/photosweep/photos/{photoId}/delete
```

#### Recuperar de Papelera
```http
POST /api/photosweep/photos/{photoId}/recover
```

#### Obtener Fotos Eliminadas
```http
GET /api/photosweep/users/{userId}/deleted?limit=5
```

#### Obtener Estadísticas
```http
GET /api/photosweep/users/{userId}/stats
```

**Respuesta:**
```json
{
  "totalPhotos": 1000,
  "reviewedPhotos": 250,
  "keptPhotos": 200,
  "deletedPhotos": 50,
  "estimatedSpaceFreed": 157286400,
  "formattedSpaceFreed": "150 MB"
}
```

#### Eliminar Permanentemente
```http
POST /api/photosweep/users/{userId}/permanent-delete
```

---

## 🔌 SignalR Hub - Eventos en Tiempo Real

### Endpoint
```
ws://localhost:5000/hubs/photoclash
```

### Métodos del Cliente → Servidor

#### Unirse a Sala
```javascript
connection.invoke("JoinRoom", roomCode, username, userId);
```

#### Salir de Sala
```javascript
connection.invoke("LeaveRoom", roomCode, username);
```

#### Notificar Jugador Unido
```javascript
connection.invoke("NotifyPlayerJoined", roomCode, username, userId);
```

#### Notificar Actualización de Sala
```javascript
connection.invoke("NotifyRoomUpdated", roomCode, roomData);
```

#### Notificar Inicio de Partida
```javascript
connection.invoke("NotifyGameStarted", roomCode, gameData);
```

#### Notificar Inicio de Ronda
```javascript
connection.invoke("NotifyRoundStarted", roomCode, roundNumber, promptPhrase, secondsPerRound);
```

#### Notificar Foto Subida
```javascript
connection.invoke("NotifyPhotoUploaded", roomCode, playerId, username);
```

#### Notificar Inicio de Votación
```javascript
connection.invoke("NotifyVotingStarted", roomCode, photos);
```

#### Notificar Voto Registrado
```javascript
connection.invoke("NotifyVoteRegistered", roomCode, voterUsername, votedUsername);
```

#### Notificar Fin de Ronda
```javascript
connection.invoke("NotifyRoundFinished", roomCode, scores);
```

#### Notificar Fin de Partida
```javascript
connection.invoke("NotifyMatchFinished", roomCode, winnerData);
```

### Eventos del Servidor → Cliente

Los clientes deben suscribirse a estos eventos:

```javascript
connection.on("PlayerJoined", (data) => {
  console.log(`${data.username} se unió a la sala`);
});

connection.on("PlayerLeft", (data) => {
  console.log(`${data.username} salió de la sala`);
});

connection.on("RoomUpdated", (roomData) => {
  // Actualizar UI con nuevo estado de sala
});

connection.on("GameStarted", (gameData) => {
  // Iniciar partida
});

connection.on("RoundStarted", (data) => {
  // Nueva ronda iniciada con frase prompt
  console.log(`Ronda ${data.roundNumber}: ${data.promptPhrase}`);
});

connection.on("TimerTick", (data) => {
  // Actualizar temporizador
  console.log(`Tiempo restante: ${data.remainingSeconds}s`);
});

connection.on("TimerExpired", () => {
  // Tiempo agotado
});

connection.on("PhotoUploaded", (data) => {
  // Jugador subió foto
});

connection.on("VotingStarted", (photos) => {
  // Mostrar fotos para votar
});

connection.on("VoteRegistered", (data) => {
  // Voto registrado
});

connection.on("RoundFinished", (scores) => {
  // Mostrar puntuaciones
});

connection.on("MatchFinished", (data) => {
  // Mostrar ganador
});
```

---

## 🗄️ Modelo de Base de Datos

### Tablas Principales

#### users
```sql
id              TEXT PRIMARY KEY
username        TEXT NOT NULL
avatar_url      TEXT
created_at      TIMESTAMP DEFAULT NOW()
last_login      TIMESTAMP
```

#### photos (PhotoSweep)
```sql
id              TEXT PRIMARY KEY
user_id         TEXT NOT NULL
uri             TEXT NOT NULL
date_taken      TIMESTAMP
keep_status     BOOLEAN
reviewed_at     TIMESTAMP
```

#### rooms (PhotoClash)
```sql
id              TEXT PRIMARY KEY
code            TEXT NOT NULL
status          TEXT DEFAULT 'waiting'
rounds_total    INTEGER
seconds_per_round INTEGER
nsfw_allowed    BOOLEAN DEFAULT FALSE
created_at      TIMESTAMP DEFAULT NOW()
```

#### room_players
```sql
id              TEXT PRIMARY KEY
room_id         TEXT NOT NULL
user_id         TEXT NOT NULL
joined_at       TIMESTAMP DEFAULT NOW()
score           INTEGER DEFAULT 0
```

#### rounds
```sql
id              TEXT PRIMARY KEY
room_id         TEXT NOT NULL
round_number    INTEGER NOT NULL
prompt_phrase   TEXT NOT NULL
started_at      TIMESTAMP
finished_at     TIMESTAMP
```

#### round_photos
```sql
id              TEXT PRIMARY KEY
round_id        TEXT NOT NULL
player_id       TEXT NOT NULL
photo_url       TEXT NOT NULL
uploaded_at     TIMESTAMP DEFAULT NOW()
```

#### votes
```sql
id              TEXT PRIMARY KEY
round_id        TEXT NOT NULL
voter_player_id TEXT NOT NULL
voted_player_id TEXT NOT NULL
created_at      TIMESTAMP DEFAULT NOW()
```

#### match_results
```sql
id              TEXT PRIMARY KEY
room_id         TEXT NOT NULL
winner_player_id TEXT NOT NULL
total_rounds    INTEGER
created_at      TIMESTAMP DEFAULT NOW()
```

#### app_settings
```sql
id              TEXT PRIMARY KEY
user_id         TEXT NOT NULL
dark_mode       BOOLEAN DEFAULT FALSE
notifications   BOOLEAN DEFAULT TRUE
language        TEXT DEFAULT 'es'
```

---

## 🔒 Validaciones de Negocio

### PhotoClash
- **Crear sala**: 1-20 rondas, 1-300 segundos por ronda
- **Unirse**: Máximo 8 jugadores, solo en estado "Waiting"
- **Iniciar**: Mínimo 2 jugadores
- **Subir foto**: Solo una por jugador por ronda
- **Votar**: No votarse a sí mismo, una vez por ronda

### PhotoSweep
- **Papelera**: Últimas 5 fotos eliminadas recuperables
- **Estimación**: ~3MB por foto promedio

---

## 🧪 Pruebas con Swagger

Una vez iniciado el servidor, accede a:
```
http://localhost:5000
```

Swagger UI te permite:
- Ver todos los endpoints
- Probar cada endpoint directamente
- Ver ejemplos de request/response
- Validar el esquema de datos

---

## 📦 Servicios Principales

### PhotoClashService
- `CreateRoomAsync`: Crear sala con código único
- `JoinRoomAsync`: Unirse a sala
- `StartGameAsync`: Generar rondas y frases
- `UploadPhotoAsync`: Subir foto con validaciones
- `VoteAsync`: Registrar voto
- `CalculateRoundScoresAsync`: Sistema de puntuación
- `FinishGameAsync`: Determinar ganador

### PhotoSweepService
- `RegisterPhotoAsync`: Registrar foto
- `KeepPhotoAsync`: Marcar como mantenida
- `DeletePhotoAsync`: Marcar como eliminada
- `RecoverPhotoAsync`: Recuperar de papelera
- `GetStatsAsync`: Estadísticas de limpieza

### PhraseGeneratorService
- Frases en español e inglés
- Soporte NSFW opcional
- Generación de frases únicas

### TimerService
- Temporizadores por sala
- Eventos SignalR cada segundo
- Cancelación y pausa

---

## 🎯 Flujo de Juego PhotoClash

1. **Host crea sala** → Obtiene código
2. **Jugadores se unen** mediante código
3. **Host inicia partida** → Se generan todas las rondas
4. **Por cada ronda:**
   - Se muestra frase prompt
   - Temporizador cuenta regresivamente
   - Jugadores suben fotos
   - Se inicia votación
   - Jugadores votan (excepto a sí mismos)
   - Se calculan puntuaciones
   - Se finaliza ronda
5. **Al terminar todas las rondas:**
   - Se calcula ganador (mayor puntuación)
   - Se guarda resultado
   - Sala pasa a estado "Finished"

---

## 🐛 Manejo de Errores

El backend incluye un middleware global de manejo de errores que retorna:

```json
{
  "error": "Mensaje descriptivo del error",
  "statusCode": 400,
  "timestamp": "2025-12-16T10:00:00Z"
}
```

### Códigos de Estado
- **200**: OK
- **400**: Bad Request (validación fallida)
- **404**: Not Found
- **500**: Internal Server Error

---

## 📝 Logs

Los logs se registran en la consola e incluyen:
- Conexiones/desconexiones SignalR
- Errores de aplicación
- Operaciones importantes

---

## 🔮 Próximas Mejoras

- [ ] Autenticación JWT
- [ ] Rate limiting
- [ ] Caché con Redis
- [ ] Paginación en endpoints
- [ ] Filtros avanzados
- [ ] Métricas y monitoreo
- [ ] Tests unitarios y de integración
- [ ] CI/CD pipeline

---

## 📞 Soporte

Para preguntas o problemas:
- Revisar logs del servidor
- Consultar Swagger UI
- Verificar conexión a base de datos

---

## 📄 Licencia

Proyecto IDEON - Clean & Clash
Backend desarrollado con ASP.NET Core 8.0
