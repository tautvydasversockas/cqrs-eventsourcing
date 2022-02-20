namespace Accounts.Tests;

public abstract class AccountSpecification<TCommand> : Specification<Account, AccountId, TCommand>
    where TCommand : ICommand
{
    protected readonly AccountId AccountId = new(Guid.NewGuid());

    protected override void ConfigureServices(ServiceCollection services)
    {
        var repositoryMock = new Mock<IAccountRepository>();

        repositoryMock
            .Setup(repository => repository.GetAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => Aggregate);

        repositoryMock
            .Setup(repository => repository.SaveAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .Callback<Account, CancellationToken>((account, _) => Aggregate = account);

        repositoryMock
            .Setup(repository => repository.GetNextId())
            .Returns(AccountId);

        services.Replace(new ServiceDescriptor(typeof(IAccountRepository), repositoryMock.Object));

        base.ConfigureServices(services);
    }
}
