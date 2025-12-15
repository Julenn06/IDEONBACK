# ======================================
# IDEON Backend - Scripts de Utilidad
# ======================================

Write-Host "╔════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    IDEON Backend - Scripts Manager        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Función para mostrar menú
function Show-Menu {
    Write-Host "Selecciona una opción:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  [1] Restaurar paquetes NuGet" -ForegroundColor Green
    Write-Host "  [2] Compilar proyecto" -ForegroundColor Green
    Write-Host "  [3] Ejecutar proyecto" -ForegroundColor Green
    Write-Host "  [4] Limpiar y compilar" -ForegroundColor Green
    Write-Host "  [5] Verificar conexión a PostgreSQL" -ForegroundColor Green
    Write-Host "  [6] Abrir Swagger en navegador" -ForegroundColor Green
    Write-Host "  [7] Ver logs en tiempo real" -ForegroundColor Green
    Write-Host "  [0] Salir" -ForegroundColor Red
    Write-Host ""
}

# Función para restaurar paquetes
function Restore-Packages {
    Write-Host "📦 Restaurando paquetes NuGet..." -ForegroundColor Cyan
    dotnet restore
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Paquetes restaurados correctamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al restaurar paquetes" -ForegroundColor Red
    }
}

# Función para compilar
function Build-Project {
    Write-Host "🔨 Compilando proyecto..." -ForegroundColor Cyan
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Proyecto compilado correctamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al compilar" -ForegroundColor Red
    }
}

# Función para ejecutar
function Run-Project {
    Write-Host "🚀 Ejecutando proyecto..." -ForegroundColor Cyan
    dotnet run
}

# Función para limpiar y compilar
function Clean-Build {
    Write-Host "🧹 Limpiando proyecto..." -ForegroundColor Cyan
    dotnet clean
    Write-Host "📦 Restaurando paquetes..." -ForegroundColor Cyan
    dotnet restore
    Write-Host "🔨 Compilando..." -ForegroundColor Cyan
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Proyecto limpiado y compilado correctamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error durante el proceso" -ForegroundColor Red
    }
}

# Función para verificar PostgreSQL
function Test-PostgreSQL {
    Write-Host "🔍 Verificando conexión a PostgreSQL..." -ForegroundColor Cyan
    try {
        $result = psql -U postgres -c "SELECT version();" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ PostgreSQL está ejecutándose" -ForegroundColor Green
            Write-Host $result
        } else {
            Write-Host "❌ No se puede conectar a PostgreSQL" -ForegroundColor Red
        }
    } catch {
        Write-Host "❌ PostgreSQL no está disponible o no está en el PATH" -ForegroundColor Red
    }
}

# Función para abrir Swagger
function Open-Swagger {
    Write-Host "🌐 Abriendo Swagger UI..." -ForegroundColor Cyan
    Start-Process "http://localhost:5000"
}

# Función para ver logs
function Show-Logs {
    Write-Host "📋 Mostrando logs (Ctrl+C para salir)..." -ForegroundColor Cyan
    dotnet run | Select-String -Pattern "."
}

# Bucle principal
do {
    Show-Menu
    $option = Read-Host "Opción"
    
    switch ($option) {
        "1" { Restore-Packages; Pause }
        "2" { Build-Project; Pause }
        "3" { Run-Project }
        "4" { Clean-Build; Pause }
        "5" { Test-PostgreSQL; Pause }
        "6" { Open-Swagger; Pause }
        "7" { Show-Logs }
        "0" { Write-Host "👋 ¡Hasta luego!" -ForegroundColor Cyan; break }
        default { Write-Host "❌ Opción no válida" -ForegroundColor Red; Pause }
    }
    
    Clear-Host
} while ($option -ne "0")
