namespace Application.Shared;

public record RefreshTokenRequestDto
{
    public string RefreshToken { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
}
