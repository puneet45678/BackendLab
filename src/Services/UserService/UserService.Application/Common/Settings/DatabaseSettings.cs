using System.ComponentModel.DataAnnotations;

namespace UserService.Application.Common.Settings;

public class DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}