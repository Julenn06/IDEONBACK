namespace IdeonBack.API.DTOs;

// ============ USER DTOs ============

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class UserResponse
{
    public string Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UpdateSettingsRequest
{
    public bool? DarkMode { get; set; }
    public bool? Notifications { get; set; }
    public string? Language { get; set; }
}

public class SettingsResponse
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public bool DarkMode { get; set; }
    public bool Notifications { get; set; }
    public string Language { get; set; } = string.Empty;
}
