using IdeonBack.API.Hubs;
using IdeonBack.API.Middleware;
using IdeonBack.Application.Services;
using IdeonBack.Domain.Interfaces;
using IdeonBack.Infrastructure.Data;
using IdeonBack.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// CONFIGURACIÓN DE SERVICIOS
// ============================================================

// CrateDB (compatible con protocolo PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IdeonDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.EnableRetryOnFailure()));

// Configuración para manejar DateTime como UTC en PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomPlayerRepository, RoomPlayerRepository>();
builder.Services.AddScoped<IRoundRepository, RoundRepository>();
builder.Services.AddScoped<IRoundPhotoRepository, RoundPhotoRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IMatchResultRepository, MatchResultRepository>();
builder.Services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();

// Servicios de aplicación
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PhotoClashService>();
builder.Services.AddScoped<PhotoSweepService>();
builder.Services.AddSingleton<PhraseGeneratorService>();
builder.Services.AddSingleton<RoomCodeGeneratorService>();
builder.Services.AddSingleton<TimerService>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// SignalR
builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "IDEON API",
        Version = "v1",
        Description = "API Backend para IDEON - Clean & Clash",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "IDEON Team"
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddPolicy("AllowFlutter", policy =>
    {
        policy.WithOrigins("http://localhost:*", "https://localhost:*")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ============================================================
// CONFIGURACIÓN DEL PIPELINE
// ============================================================

// Middleware de manejo de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IDEON API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// SignalR Hub
app.MapHub<PhotoClashHub>("/hubs/photoclash");

// Endpoint de health check
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    service = "IDEON Backend"
}));

// ============================================================
// INICIAR APLICACIÓN
// ============================================================

Console.WriteLine("╔════════════════════════════════════════════╗");
Console.WriteLine("║    IDEON Backend - Clean & Clash          ║");
Console.WriteLine("║    ASP.NET Core 8 + CrateDB               ║");
Console.WriteLine("╚════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine($"🚀 Iniciando servidor en: {DateTime.UtcNow}");
Console.WriteLine($"📡 SignalR Hub: /hubs/photoclash");
Console.WriteLine($"📚 Swagger UI: http://localhost:{builder.Configuration["Urls"]?.Split(':').Last() ?? "5000"}");
Console.WriteLine();

app.Run();
