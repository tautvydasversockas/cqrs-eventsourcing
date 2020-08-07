using System.Linq;
using Accounts.ReadModel.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Accounts.ReadModel
{
    public sealed class AccountDbContext : DbContext, IAccountReadModel
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<AccountDto> Accounts { get; set; }
        IQueryable<AccountDto> IAccountReadModel.Accounts => Accounts;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountConfig).Assembly);
        }
    }
}