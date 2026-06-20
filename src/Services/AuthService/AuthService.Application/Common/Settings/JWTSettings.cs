using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Common.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Secret { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int AccessTokenExpiryMinutes { get; init; } = 15;

    [Range(1, int.MaxValue)]
    public int RefreshTokenExpiryDays { get; init; } = 7;
}