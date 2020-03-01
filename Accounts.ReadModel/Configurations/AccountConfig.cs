using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.ReadModel.Configurations
{
    public sealed class AccountConfig : IEntityTypeConfiguration<ActiveAccount>
    {
        public const string TableName = "Accounts";
        public const string IdColumnName = "Id";
        public const string VersionColumnName = "Version";
        public const string ClientIdColumnName = "ClientId";
        public const string InterestRateColumnName = "InterestRate";
        public const string BalanceColumnName = "Balance";
        public const string IsFrozenColumnName = "IsFrozen";

        public void Configure(EntityTypeBuilder<ActiveAccount> builder)
        {
            builder
                .ToTable(TableName)
                .HasKey(o => o.Id);

            builder
                .Property(o => o.Id)
                .HasColumnName(IdColumnName)
                .IsRequired();

            builder
                .Property(o => o.Version)
                .HasColumnName(VersionColumnName)
                .IsRequired();

            builder
                .Property(o => o.ClientId)
                .HasColumnName(ClientIdColumnName)
                .IsRequired();

            builder
                .Property(o => o.InterestRate)
                .HasColumnName(InterestRateColumnName)
                .IsRequired();

            builder
                .Property(o => o.Balance)
                .HasColumnName(BalanceColumnName)
                .IsRequired();

            builder
                .Property(o => o.IsFrozen)
                .HasColumnName(IsFrozenColumnName)
                .IsRequired();
        }
    }
}