using System.Linq;

namespace Accounts.ReadModel
{
    public interface IAccountReadModel
    {
        IQueryable<ActiveAccount> Accounts { get; }
    }
}