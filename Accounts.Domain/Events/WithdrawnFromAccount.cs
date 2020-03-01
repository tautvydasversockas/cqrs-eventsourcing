using Infrastructure.Domain;

namespace Accounts.Domain.Events
{
    public sealed class WithdrawnFromAccount : VersionedEvent
    {
        public decimal Amount { get; }

        public WithdrawnFromAccount(decimal amount)
        {
            Amount = amount;
        }
    }
}