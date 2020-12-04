using System.ComponentModel.DataAnnotations;

namespace Accounts.Infrastructure
{
    public sealed record EventStoreSettings
    {
        [Required]
        public string ConnectionString { get; init; } = string.Empty;
    }
}