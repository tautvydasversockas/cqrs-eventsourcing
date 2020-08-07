using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.ReadModel.Configurations
{
    public sealed class AccountConfig : IEntityTypeConfiguration<AccountDto>
    {
        public void Configure(EntityTypeBuilder<AccountDto> builder)
        {
            builder.ToTable("Accounts").HasKey(o => o.Id);
        }
    }
}