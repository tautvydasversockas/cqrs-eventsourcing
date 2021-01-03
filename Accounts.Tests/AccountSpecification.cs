using System;
using System.Threading;
using Accounts.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Accounts.Tests
{
    public abstract class AccountSpecification<TCommand> : Specification<Account, AccountId, TCommand>
        where TCommand : ICommand
    {
        protected readonly AccountId AccountId = new(Guid.NewGuid());

        protected override void ConfigureServices(ServiceCollection services)
        {
            var accountRepositoryMock = new Mock<IAccountRepository>();

            accountRepositoryMock
                .Setup(repository => repository.GetAsync(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Aggregate);

            accountRepositoryMock
                .Setup(repository => repository.SaveAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((account, _) => Aggregate = account);

            accountRepositoryMock
                .Setup(repository => repository.GetNextIdentity())
                .Returns(AccountId);

            services.Replace(new(typeof(IAccountRepository), accountRepositoryMock.Object));
            base.ConfigureServices(services);
        }
    }
}