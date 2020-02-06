using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.ReadModel.Configurations
{
    public sealed class OpenAccountConfiguration : IEntityTypeConfiguration<ActiveAccount>
    {
        public void Configure(EntityTypeBuilder<ActiveAccount> builder)
        {
            builder.
                ToTable("Accounts").
                HasKey(o => o.Id);

            builder.
                Property(o => o.Version).
                IsRequired();

            builder.
                Property(o => o.ClientId).
                IsRequired();

            builder.
                Property(o => o.InterestRate).
                IsRequired();

            builder.
                Property(o => o.Balance).
                IsRequired();

            builder.
                Property(o => o.IsFrozen).
                IsRequired();
        }
    }
}