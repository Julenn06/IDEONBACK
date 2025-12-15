namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio para generar códigos únicos de sala
/// </summary>
public class RoomCodeGeneratorService
{
    private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private readonly Random _random = new();

    public string GenerateCode(int length = 6)
    {
        var code = new char[length];
        for (int i = 0; i < length; i++)
        {
            code[i] = Characters[_random.Next(Characters.Length)];
        }
        return new string(code);
    }
}
