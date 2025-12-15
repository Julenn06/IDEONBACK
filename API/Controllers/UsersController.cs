using IdeonBack.API.DTOs;
using IdeonBack.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdeonBack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Crear un nuevo usuario
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request.Username, request.AvatarUrl);
            
            return Ok(new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<UserResponse>> GetUser(string userId)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        });
    }

    /// <summary>
    /// Obtener usuario por nombre
    /// </summary>
    [HttpGet("username/{username}")]
    public async Task<ActionResult<UserResponse>> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        });
    }

    /// <summary>
    /// Actualizar configuración del usuario
    /// </summary>
    [HttpPut("{userId}/settings")]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings(string userId, [FromBody] UpdateSettingsRequest request)
    {
        var settings = await _userService.UpdateSettingsAsync(
            userId, 
            request.DarkMode, 
            request.Notifications, 
            request.Language
        );

        return Ok(new SettingsResponse
        {
            Id = settings.Id,
            UserId = settings.UserId,
            DarkMode = settings.DarkMode,
            Notifications = settings.Notifications,
            Language = settings.Language
        });
    }

    /// <summary>
    /// Obtener configuración del usuario
    /// </summary>
    [HttpGet("{userId}/settings")]
    public async Task<ActionResult<SettingsResponse>> GetSettings(string userId)
    {
        var settings = await _userService.GetSettingsAsync(userId);
        if (settings == null)
            return NotFound(new { error = "Configuración no encontrada" });

        return Ok(new SettingsResponse
        {
            Id = settings.Id,
            UserId = settings.UserId,
            DarkMode = settings.DarkMode,
            Notifications = settings.Notifications,
            Language = settings.Language
        });
    }

    /// <summary>
    /// Actualizar último login
    /// </summary>
    [HttpPost("{userId}/login")]
    public async Task<IActionResult> UpdateLastLogin(string userId)
    {
        await _userService.UpdateLastLoginAsync(userId);
        return Ok(new { message = "Login actualizado" });
    }
}
