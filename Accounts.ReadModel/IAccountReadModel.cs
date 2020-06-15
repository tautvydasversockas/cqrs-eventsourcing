using System.Linq;

namespace Accounts.ReadModel
{
    public interface IAccountReadModel
    {
        IQueryable<AccountDto> Accounts { get; }
    }
}