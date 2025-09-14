namespace Identity.Api.Options;

public sealed class JwtSettings
{
    public string AccessTokenSecret { get; set; } = null!;
    public string RefreshTokenSecret { get; set; } = null!;
    public int AccessTokenExpMinutes { get; set; }
    public int RefreshTokenExpMinutes { get; set; }
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}