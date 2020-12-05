using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Common;
using Accounts.Domain;
using Accounts.Domain.Common;

namespace Accounts.Application.Handlers
{
    public sealed class AccountCommandHandlers :
        Handler<Account>,
        IHandler<OpenAccount>,
        IHandler<DepositToAccount>,
        IHandler<WithdrawFromAccount>,
        IHandler<AddInterestsToAccount>,
        IHandler<FreezeAccount>,
        IHandler<UnfreezeAccount>
    {
        public AccountCommandHandlers(IEventSourcedRepository<Account> repository)
            : base(repository) { }

        public Task HandleAsync(OpenAccount command, CancellationToken token = default)
        {
            var account = Account.Open(command.AccountId, command.ClientId, new(command.InterestRate), command.Balance);
            return CreateAsync(account, token);
        }

        public Task HandleAsync(DepositToAccount command, CancellationToken token = default)
        {
            return UpdateAsync(command.AccountId, account => account.Deposit(command.Amount), token);
        }

        public Task HandleAsync(WithdrawFromAccount command, CancellationToken token = default)
        {
            return UpdateAsync(command.AccountId, account => account.Withdraw(command.Amount), token);
        }

        public Task HandleAsync(AddInterestsToAccount command, CancellationToken token = default)
        {
            return UpdateAsync(command.AccountId, account => account.AddInterests(), token);
        }

        public Task HandleAsync(FreezeAccount command, CancellationToken token = default)
        {
            return UpdateAsync(command.AccountId, account => account.Freeze(), token);
        }

        public Task HandleAsync(UnfreezeAccount command, CancellationToken token = default)
        {
            return UpdateAsync(command.AccountId, account => account.Unfreeze(), token);
        }
    }
}