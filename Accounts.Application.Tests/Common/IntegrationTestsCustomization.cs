using System;
using Accounts.Domain;
using AutoFixture;

namespace Accounts.Application.Tests.Common
{
    public sealed class IntegrationTestsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => Account.Open(
                id: Guid.NewGuid(),
                clientId: Guid.NewGuid(),
                interestRate: (InterestRate)0.1m,
                balance: 0));
        }
    }
}