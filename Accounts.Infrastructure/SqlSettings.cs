using System.ComponentModel.DataAnnotations;

namespace Accounts.Infrastructure
{
    public sealed record SqlSettings
    {
        [Required]
        public string ConnectionString { get; init; } = string.Empty;
    }
}