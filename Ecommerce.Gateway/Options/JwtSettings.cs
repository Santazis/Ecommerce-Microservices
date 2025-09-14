namespace EcommerceMicroservices.Options;

public class JwtSettings
{
    public string AccessTokenSecret { get; set; } = null!;
    public int AccessTokenExpMinutes { get; set; }
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}