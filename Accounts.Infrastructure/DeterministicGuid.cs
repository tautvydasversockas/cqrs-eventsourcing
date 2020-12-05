using System;
using Vlingo.UUID;

namespace Accounts.Infrastructure
{
    public static class DeterministicGuid
    {
        public static Guid Create(Guid val)
        {
            using var generator = new NameBasedGenerator(HashType.Sha1);
            return generator.GenerateGuid(UUIDNameSpace.Url, val.ToString());
        }
    }
}