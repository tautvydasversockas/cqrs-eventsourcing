using System;

namespace Infrastructure.Domain
{
    public abstract class Entity
    {
        protected Entity(Guid id)
        {
            Id = id;
        }

        public virtual Guid Id { get; }
        protected virtual object Actual => this;

        public override bool Equals(object obj)
        {
            if (!(obj is Entity other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Actual.GetType() != other.Actual.GetType())
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType() + Id.ToString()).GetHashCode();
        }
    }
}