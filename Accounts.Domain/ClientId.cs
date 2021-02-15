using CSharpFunctionalExtensions;
using System;

namespace Accounts.Domain
{
    public sealed class ClientId : SimpleValueObject<Guid>
    {
        public ClientId(Guid value)
            : base(value) { }
    }
}