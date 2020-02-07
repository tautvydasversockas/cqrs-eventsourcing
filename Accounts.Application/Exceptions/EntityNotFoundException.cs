using System;

namespace Accounts.Application.Exceptions
{
    public sealed class EntityNotFoundException : Exception
    {
        public string Name { get; }
        public object Key { get; }

        public EntityNotFoundException(string name, object key)
            : base($"Entity '{name}' ({key}) was not found.")
        {
            Name = name;
            Key = key;
        }
    }
}