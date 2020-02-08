using System;
using Vlingo.UUID;

namespace Infrastructure
{
    public static class DeterministicGuid
    {
        public static Guid Create(Guid val)
        {
            return Create(val.ToString());
        }

        public static Guid Create(string val)
        {
            using var generator = new NameBasedGenerator(HashType.SHA1);
            return generator.GenerateGuid(UUIDNameSpace.URL, val);
        }
    }
}