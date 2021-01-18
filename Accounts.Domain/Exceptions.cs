using System;

namespace Accounts.Domain
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) 
            : base(message) { }
    }

    public sealed class FrozenAccountException : DomainException
    {
        public FrozenAccountException() 
            : base("Account is frozen.") { }
    }

    public sealed class InsufficientBalanceException : DomainException
    {
        public InsufficientBalanceException()
            : base("Insufficient balance.") { }
    }
}