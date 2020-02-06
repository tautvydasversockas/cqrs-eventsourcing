using System;

namespace Accounts.ReadModel
{
    public sealed class ActiveAccount
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public Guid ClientId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Balance { get; set; }
        public bool IsFrozen { get; set; }
    }
}