using System;
using CSharpFunctionalExtensions;

namespace Accounts.Domain
{
    public sealed class ClientId : SimpleValueObject<Guid>
    {
        public ClientId(Guid value)
            : base(value) { }
    }
}