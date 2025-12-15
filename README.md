# IDEON Backend - Guía de Uso

## 📋 Tabla de Contenidos

1. [Requisitos Previos](#requisitos-previos)
2. [Configuración Inicial](#configuración-inicial)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Ejecutar el Proyecto](#ejecutar-el-proyecto)
5. [Endpoints API](#endpoints-api)
6. [SignalR Events](#signalr-events)
7. [Testing con Swagger](#testing-con-swagger)

---

## 🔧 Requisitos Previos

### Software Necesario

- **.NET 8 SDK** o superior
- **PostgreSQL 15+** instalado y ejecutándose
- **Visual Studio 2022** o **VS Code** con extensión C#
- **pgAdmin** o similar (opcional, para gestionar la BD)

### Verificar Instalaciones

```powershell
# Verificar .NET
dotnet --version

# Verificar PostgreSQL
psql --version
```

---

## ⚙️ Configuración Inicial

### 1. Crear Base de Datos

Conectarse a PostgreSQL y ejecutar el script `bd.sql`:

```powershell
psql -U postgres
CREATE DATABASE ideon_db;
\c ideon_db
\i 'c:/Users/in2dm3-d.ELORRIETA/Desktop/IA/ideonBack/bd.sql'
```

O importar desde pgAdmin.

### 2. Configurar Connection String

Editar `appsettings.json` y actualizar la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ideon_db;Username=postgres;Password=TU_PASSWORD_AQUI"
  }
}
```

**⚠️ IMPORTANTE:** Reemplazar `TU_PASSWORD_AQUI` con tu contraseña de PostgreSQL.

### 3. Restaurar Paquetes NuGet

```powershell
cd c:\Users\in2dm3-d.ELORRIETA\Desktop\IA\ideonBack
dotnet restore
```

---

## 📁 Estructura del Proyecto

```
ideonBack/
├── Domain/                    # Capa de Dominio
│   ├── Entities/             # Entidades (User, Room, Photo, etc.)
│   ├── Enums/                # Enumeraciones (RoomStatus, etc.)
│   └── Interfaces/           # Interfaces de repositorios
├── Infrastructure/            # Capa de Infraestructura
│   ├── Data/                 # DbContext
│   └── Repositories/         # Implementaciones de repositorios
├── Application/               # Capa de Aplicación
│   └── Services/             # Lógica de negocio
├── API/                       # Capa de Presentación
│   ├── Controllers/          # Controllers REST
│   ├── DTOs/                 # Data Transfer Objects
│   └── Hubs/                 # SignalR Hubs
├── Program.cs                 # Punto de entrada
├── appsettings.json          # Configuración
└── ideonBack.csproj          # Archivo de proyecto
```

### Arquitectura Limpia (Clean Architecture)

```
┌──────────────────────────────────────┐
│           API Layer                  │  ← Controllers, DTOs, Hubs
├──────────────────────────────────────┤
│       Application Layer              │  ← Services (Lógica de negocio)
├──────────────────────────────────────┤
│     Infrastructure Layer             │  ← DbContext, Repositories
├──────────────────────────────────────┤
│         Domain Layer                 │  ← Entities, Interfaces
└──────────────────────────────────────┘
```

---

## 🚀 Ejecutar el Proyecto

### Opción 1: Desde Visual Studio

1. Abrir `ideonBack.sln`
2. Presionar `F5` o click en "Run"

### Opción 2: Desde Terminal

```powershell
cd c:\Users\in2dm3-d.ELORRIETA\Desktop\IA\ideonBack
dotnet build
dotnet run
```

### Verificar que está funcionando

El servidor debería iniciar en:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000 (redirige automáticamente)

Deberías ver en consola:

```
╔════════════════════════════════════════════╗
║    IDEON Backend - Clean & Clash          ║
║    ASP.NET Core 8 + PostgreSQL            ║
╚════════════════════════════════════════════╝

🚀 Iniciando servidor en: 2025-12-15 10:30:00
📡 SignalR Hub: /hubs/photoclash
📚 Swagger UI: http://localhost:5000
```

---

## 📚 Endpoints API

### 👤 Users - `/api/users`

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/users` | Crear usuario |
| GET | `/api/users/{userId}` | Obtener usuario por ID |
| GET | `/api/users/username/{username}` | Obtener usuario por nombre |
| PUT | `/api/users/{userId}/settings` | Actualizar configuración |
| GET | `/api/users/{userId}/settings` | Obtener configuración |
| POST | `/api/users/{userId}/login` | Actualizar último login |

#### Ejemplo: Crear Usuario

```json
POST /api/users
{
  "username": "jugador1",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

---

### 🎮 PhotoClash - `/api/photoclash`

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/photoclash/rooms` | Crear sala |
| POST | `/api/photoclash/rooms/join` | Unirse a sala |
| GET | `/api/photoclash/rooms/{roomId}` | Obtener estado de sala |
| POST | `/api/photoclash/rooms/start` | Iniciar partida |
| GET | `/api/photoclash/rooms/{roomId}/current-round` | Obtener ronda actual |
| POST | `/api/photoclash/photos` | Subir foto |
| GET | `/api/photoclash/rounds/{roundId}/photos` | Obtener fotos de ronda |
| POST | `/api/photoclash/rooms/{roomId}/start-voting` | Iniciar votación |
| POST | `/api/photoclash/votes` | Votar |
| POST | `/api/photoclash/rounds/{roundId}/calculate-scores` | Calcular puntuaciones |
| POST | `/api/photoclash/rounds/{roundId}/finish` | Finalizar ronda |
| POST | `/api/photoclash/rooms/{roomId}/next-round` | Siguiente ronda |
| POST | `/api/photoclash/rooms/{roomId}/finish` | Finalizar partida |

#### Ejemplo: Crear Sala

```json
POST /api/photoclash/rooms
{
  "hostUserId": "guid-del-usuario",
  "roundsTotal": 5,
  "secondsPerRound": 60,
  "nsfwAllowed": false
}
```

#### Ejemplo: Unirse a Sala

```json
POST /api/photoclash/rooms/join
{
  "code": "ABC123",
  "userId": "guid-del-usuario"
}
```

#### Ejemplo: Subir Foto

```json
POST /api/photoclash/photos
{
  "roundId": "guid-de-ronda",
  "playerId": "guid-de-jugador",
  "photoUrl": "https://storage.example.com/photo123.jpg"
}
```

#### Ejemplo: Votar

```json
POST /api/photoclash/votes
{
  "roundId": "guid-de-ronda",
  "voterPlayerId": "guid-votante",
  "votedPlayerId": "guid-votado"
}
```

---

### 🧹 PhotoSweep - `/api/photosweep`

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/photosweep/photos` | Registrar foto |
| GET | `/api/photosweep/users/{userId}/unreviewed` | Obtener fotos sin revisar |
| POST | `/api/photosweep/photos/{photoId}/keep` | Marcar como mantenida |
| POST | `/api/photosweep/photos/{photoId}/delete` | Marcar como eliminada |
| POST | `/api/photosweep/photos/{photoId}/recover` | Recuperar de papelera |
| GET | `/api/photosweep/users/{userId}/deleted` | Obtener fotos eliminadas |
| GET | `/api/photosweep/users/{userId}/stats` | Obtener estadísticas |
| DELETE | `/api/photosweep/users/{userId}/permanent-delete` | Eliminar permanentemente |

#### Ejemplo: Registrar Foto

```json
POST /api/photosweep/photos
{
  "userId": "guid-del-usuario",
  "uri": "content://media/external/images/media/12345",
  "dateTaken": "2025-12-15T10:30:00Z"
}
```

---

## 📡 SignalR Events

### Conectarse al Hub

**Endpoint:** `/hubs/photoclash`

### Eventos que puede recibir el cliente Flutter

| Evento | Descripción | Payload |
|--------|-------------|---------|
| `PlayerJoined` | Un jugador se unió | `{ username, userId, timestamp }` |
| `PlayerLeft` | Un jugador salió | `{ connectionId }` |
| `RoomUpdated` | La sala se actualizó | `{ roomData }` |
| `GameStarted` | La partida comenzó | `{ gameData }` |
| `RoundStarted` | Nueva ronda comenzó | `{ roundNumber, promptPhrase, secondsPerRound, startedAt }` |
| `TimerTick` | Tick del temporizador | `{ remainingSeconds, timestamp }` |
| `PhotoUploaded` | Foto subida | `{ playerId, username, timestamp }` |
| `VotingStarted` | Votación iniciada | `{ photos, timestamp }` |
| `VoteRegistered` | Voto registrado | `{ voterId, timestamp }` |
| `RoundFinished` | Ronda finalizada | `{ scores, leaderboard, timestamp }` |
| `MatchFinished` | Partida finalizada | `{ matchResult, timestamp }` |
| `Error` | Error ocurrido | `{ error, timestamp }` |

### Métodos que puede llamar el cliente

```dart
// Unirse a sala
await hubConnection.invoke('JoinRoom', args: ['ABC123']);

// Notificar jugador unido
await hubConnection.invoke('NotifyPlayerJoined', args: [
  'ABC123',
  'JugadorX',
  'user-guid'
]);

// Notificar foto subida
await hubConnection.invoke('NotifyPhotoUploaded', args: [
  'ABC123',
  'player-guid',
  'username'
]);
```

---

## 🧪 Testing con Swagger

### Acceder a Swagger

Abrir navegador en: **http://localhost:5000**

### Flujo de Prueba Completo - PhotoClash

#### 1. Crear dos usuarios

```
POST /api/users
Body: { "username": "Player1" }
```

Guardar el `id` devuelto como `user1Id`

```
POST /api/users
Body: { "username": "Player2" }
```

Guardar el `id` como `user2Id`

#### 2. Crear sala

```
POST /api/photoclash/rooms
Body: {
  "hostUserId": "{user1Id}",
  "roundsTotal": 3,
  "secondsPerRound": 30,
  "nsfwAllowed": false
}
```

Guardar el `code` devuelto

#### 3. Unir segundo jugador

```
POST /api/photoclash/rooms/join
Body: {
  "code": "{code-de-sala}",
  "userId": "{user2Id}"
}
```

#### 4. Iniciar partida

```
POST /api/photoclash/rooms/start
Body: {
  "roomId": "{roomId}",
  "language": "es"
}
```

#### 5. Obtener ronda actual

```
GET /api/photoclash/rooms/{roomId}/current-round
```

Guardar `roundId` y observar `promptPhrase`

#### 6. Subir fotos (ambos jugadores)

```
POST /api/photoclash/photos
Body: {
  "roundId": "{roundId}",
  "playerId": "{player1Id}",
  "photoUrl": "https://example.com/photo1.jpg"
}
```

```
POST /api/photoclash/photos
Body: {
  "roundId": "{roundId}",
  "playerId": "{player2Id}",
  "photoUrl": "https://example.com/photo2.jpg"
}
```

#### 7. Iniciar votación

```
POST /api/photoclash/rooms/{roomId}/start-voting
```

#### 8. Votar (ambos jugadores votan al contrario)

```
POST /api/photoclash/votes
Body: {
  "roundId": "{roundId}",
  "voterPlayerId": "{player1Id}",
  "votedPlayerId": "{player2Id}"
}
```

```
POST /api/photoclash/votes
Body: {
  "roundId": "{roundId}",
  "voterPlayerId": "{player2Id}",
  "votedPlayerId": "{player1Id}"
}
```

#### 9. Calcular puntuaciones

```
POST /api/photoclash/rounds/{roundId}/calculate-scores
```

#### 10. Finalizar ronda

```
POST /api/photoclash/rounds/{roundId}/finish
```

#### 11. Siguiente ronda

```
POST /api/photoclash/rooms/{roomId}/next-round
```

Repetir pasos 5-11 para cada ronda

#### 12. Finalizar partida

```
POST /api/photoclash/rooms/{roomId}/finish
```

---

## 🔐 Validaciones Implementadas

### PhotoClash

- ✅ No se puede votar a uno mismo
- ✅ No se puede votar dos veces en la misma ronda
- ✅ No se puede subir más de una foto por ronda
- ✅ No se puede unir a sala en curso
- ✅ Mínimo 2 jugadores para iniciar
- ✅ Rondas entre 1 y 20
- ✅ Segundos por ronda entre 1 y 300

### PhotoSweep

- ✅ Validación de foto existente antes de marcar
- ✅ Recuperación de últimas 5 fotos eliminadas
- ✅ Cálculo de espacio liberado

---

## 🐛 Solución de Problemas

### Error de conexión a PostgreSQL

**Síntoma:** `Npgsql.NpgsqlException: Connection refused`

**Solución:**
1. Verificar que PostgreSQL esté ejecutándose
2. Comprobar puerto (defecto: 5432)
3. Verificar usuario/password en `appsettings.json`

### Tabla no existe

**Síntoma:** `relation "users" does not exist`

**Solución:**
1. Ejecutar el script `bd.sql` en la base de datos
2. Verificar que estás conectado a `ideon_db`

### Error de paquetes NuGet

**Síntoma:** `Package not found`

**Solución:**
```powershell
dotnet clean
dotnet restore
dotnet build
```

---

## 📖 Recursos Adicionales

- [Documentación ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [SignalR](https://docs.microsoft.com/aspnet/core/signalr)
- [PostgreSQL](https://www.postgresql.org/docs)

---

## ✅ Checklist de Verificación

- [ ] PostgreSQL instalado y ejecutándose
- [ ] Base de datos `ideon_db` creada
- [ ] Script `bd.sql` ejecutado
- [ ] Connection string actualizada en `appsettings.json`
- [ ] `dotnet restore` ejecutado sin errores
- [ ] Proyecto compila correctamente
- [ ] Swagger accesible en http://localhost:5000
- [ ] Endpoints responden correctamente
- [ ] SignalR Hub accesible

---

**¡Backend listo para ser consumido por Flutter! 🚀**
