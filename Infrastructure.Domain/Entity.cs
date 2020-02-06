namespace Infrastructure.Domain
{
    public abstract class Entity<TId>
    {
        protected Entity(TId id)
        {
            Id = id;
        }

        public virtual TId Id { get; }
        protected virtual object Actual => this;

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TId> other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Actual.GetType() != other.Actual.GetType())
                return false;

            if (Id == null || other.Id == null)
                return false;

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity<TId> a, Entity<TId> b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TId> a, Entity<TId> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType() + Id.ToString()).GetHashCode();
        }
    }
}