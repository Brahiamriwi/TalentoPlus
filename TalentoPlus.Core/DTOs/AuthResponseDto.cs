namespace TalentoPlus.Core.DTOs;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
}
