# ⚡ Quick Start - IDEON Backend

## 🚀 Inicio Rápido en 5 Pasos

### Paso 1: Configurar PostgreSQL ⚙️

```powershell
# Abrir PostgreSQL
psql -U postgres

# Crear base de datos
CREATE DATABASE ideon_db;

# Conectar a la BD
\c ideon_db

# Ejecutar script SQL
\i 'c:/Users/in2dm3-d.ELORRIETA/Desktop/IA/ideonBack/bd.sql'

# Verificar tablas creadas
\dt

# Salir
\q
```

### Paso 2: Configurar Connection String 🔐

Editar: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ideon_db;Username=postgres;Password=TU_PASSWORD_AQUI"
  }
}
```

**⚠️ IMPORTANTE:** Reemplazar `TU_PASSWORD_AQUI`

### Paso 3: Compilar ⚙️

```powershell
cd c:\Users\in2dm3-d.ELORRIETA\Desktop\IA\ideonBack
dotnet build
```

Deberías ver: `✅ Compilación realizado correctamente`

### Paso 4: Ejecutar 🚀

```powershell
dotnet run
```

Deberías ver:

```
╔════════════════════════════════════════════╗
║    IDEON Backend - Clean & Clash          ║
║    ASP.NET Core 8 + PostgreSQL            ║
╚════════════════════════════════════════════╝

🚀 Iniciando servidor en: 2025-12-15 ...
📡 SignalR Hub: /hubs/photoclash
📚 Swagger UI: http://localhost:5000
```

### Paso 5: Probar con Swagger 📚

Abrir navegador en: **http://localhost:5000**

---

## ✅ Verificación Rápida

### Test 1: Health Check
```
GET http://localhost:5000/health
```

Respuesta esperada:
```json
{
  "status": "healthy",
  "timestamp": "2025-12-15T...",
  "service": "IDEON Backend"
}
```

### Test 2: Crear Usuario
```
POST http://localhost:5000/api/users
Content-Type: application/json

{
  "username": "test_user",
  "avatarUrl": null
}
```

Respuesta esperada: Usuario creado con ID GUID

### Test 3: Crear Sala PhotoClash
```
POST http://localhost:5000/api/photoclash/rooms
Content-Type: application/json

{
  "hostUserId": "{GUID_DEL_USUARIO}",
  "roundsTotal": 3,
  "secondsPerRound": 60,
  "nsfwAllowed": false
}
```

Respuesta esperada: Sala creada con código de 6 caracteres

---

## 🎯 Endpoints Más Usados

| Acción | Método | Endpoint |
|--------|--------|----------|
| Crear usuario | POST | `/api/users` |
| Crear sala | POST | `/api/photoclash/rooms` |
| Unirse a sala | POST | `/api/photoclash/rooms/join` |
| Iniciar partida | POST | `/api/photoclash/rooms/start` |
| Subir foto | POST | `/api/photoclash/photos` |
| Votar | POST | `/api/photoclash/votes` |

---

## 📡 SignalR - Conexión desde Flutter

```dart
import 'package:signalr_netcore/signalr_client.dart';

final hubConnection = HubConnectionBuilder()
    .withUrl('http://localhost:5000/hubs/photoclash')
    .build();

await hubConnection.start();

// Suscribirse a eventos
hubConnection.on('RoundStarted', (args) {
  print('Ronda iniciada: ${args}');
});

// Unirse a sala
await hubConnection.invoke('JoinRoom', args: ['ABC123']);
```

---

## 🐛 Problemas Comunes

### ❌ "Connection refused"
**Causa:** PostgreSQL no está ejecutándose  
**Solución:**
```powershell
# Windows (verificar servicio)
Get-Service postgresql*

# Si está detenido
Start-Service postgresql-x64-15
```

### ❌ "relation does not exist"
**Causa:** No se ejecutó el script SQL  
**Solución:** Ejecutar `bd.sql` (ver Paso 1)

### ❌ "Password authentication failed"
**Causa:** Contraseña incorrecta en `appsettings.json`  
**Solución:** Verificar password de PostgreSQL

### ❌ "Port 5000 is already in use"
**Solución:**
```powershell
# Cambiar puerto en appsettings.json
"Urls": "http://localhost:5050;https://localhost:5051"
```

---

## 📋 Comandos Útiles

```powershell
# Restaurar paquetes
dotnet restore

# Limpiar proyecto
dotnet clean

# Compilar
dotnet build

# Ejecutar
dotnet run

# Ejecutar con watch (auto-reload)
dotnet watch run

# Ver logs detallados
dotnet run --verbosity detailed
```

---

## 🎓 Scripts Automáticos

Ejecutar menú interactivo:

```powershell
.\scripts.ps1
```

Opciones disponibles:
1. Restaurar paquetes
2. Compilar proyecto
3. Ejecutar proyecto
4. Limpiar y compilar
5. Verificar PostgreSQL
6. Abrir Swagger
7. Ver logs en tiempo real

---

## 📚 Documentación Completa

- **README.md** - Guía completa del backend
- **FLUTTER_INTEGRATION.md** - Integración con Flutter
- **RESUMEN_EJECUTIVO.md** - Vista general del proyecto

---

## ✨ ¡Listo!

Ya tienes el backend funcionando. Ahora puedes:

1. ✅ Probar endpoints en Swagger
2. ✅ Conectar desde Flutter
3. ✅ Implementar la lógica del frontend

**🎉 Happy Coding!**
