using System.Linq;
using Accounts.ReadModel.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Accounts.ReadModel
{
    public sealed class AccountDbContext : DbContext, IAccountReadModel
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<ActiveAccount> Accounts { get; set; }
        IQueryable<ActiveAccount> IAccountReadModel.Accounts => Accounts;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfig());
        }
    }
}