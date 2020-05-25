using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application.Commands;
using Accounts.Application.Handlers;
using Accounts.Application.Tests.Common;
using Accounts.Domain;
using AutoFixture.Xunit2;
using Infrastructure.Domain;
using Moq;
using Xunit;

namespace Accounts.Application.Tests
{
    public sealed class AccountsCommandHandlerTests
    {
        [Theory, AutoMoqData]
        public async Task Handle_OpenAccount(
            [Frozen] Mock<IEventSourcedRepository<Account>> repository,
            AccountsCommandHandler handler)
        {
            var command = new OpenAccount(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: 0.1m,
                balance: 0);

            await handler.Handle(command, CancellationToken.None);

            repository.Verify(x => x.SaveAsync(It.IsAny<Account>(), command.Id), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Handle_DepositToAccount(
            Account account,
            [Frozen] Mock<IEventSourcedRepository<Account>> repository,
            AccountsCommandHandler handler)
        {
            repository
                .Setup(x => x.GetAsync(account.Id))
                .ReturnsAsync(account);

            var command = new DepositToAccount(
                id: Guid.NewGuid(),
                accountId: account.Id,
                amount: 100);

            await handler.Handle(command, CancellationToken.None);

            repository.Verify(x => x.GetAsync(account.Id), Times.Once);
            repository.Verify(x => x.SaveAsync(account, command.Id), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Handle_WithdrawFromAccount(
            Account account,
            [Frozen] Mock<IEventSourcedRepository<Account>> repository,
            AccountsCommandHandler handler)
        {
            account.Deposit(100);

            repository
                .Setup(x => x.GetAsync(account.Id))
                .ReturnsAsync(account);

            var command = new WithdrawFromAccount(
                id: Guid.NewGuid(),
                accountId: account.Id,
                amount: 100);

            await handler.Handle(command, CancellationToken.None);

            repository.Verify(x => x.GetAsync(account.Id), Times.Once);
            repository.Verify(x => x.SaveAsync(account, command.Id), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Handle_FreezeAccount(
            Account account,
            [Frozen] Mock<IEventSourcedRepository<Account>> repository,
            AccountsCommandHandler handler)
        {
            repository
                .Setup(x => x.GetAsync(account.Id))
                .ReturnsAsync(account);

            var command = new FreezeAccount(
                id: Guid.NewGuid(),
                accountId: account.Id);

            await handler.Handle(command, CancellationToken.None);

            repository.Verify(x => x.GetAsync(account.Id), Times.Once);
            repository.Verify(x => x.SaveAsync(account, command.Id), Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Handle_UnFreezeAccount(
            Account account,
            [Frozen] Mock<IEventSourcedRepository<Account>> repository,
            AccountsCommandHandler handler)
        {
            account.Freeze();

            repository
                .Setup(x => x.GetAsync(account.Id))
                .ReturnsAsync(account);

            var command = new UnFreezeAccount(
                id: Guid.NewGuid(),
                accountId: account.Id);

            await handler.Handle(command, CancellationToken.None);

            repository.Verify(x => x.GetAsync(account.Id), Times.Once);
            repository.Verify(x => x.SaveAsync(account, command.Id), Times.Once);
        }
    }
}
