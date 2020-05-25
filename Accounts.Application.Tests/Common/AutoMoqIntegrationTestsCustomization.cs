using AutoFixture;
using AutoFixture.AutoMoq;

namespace Accounts.Application.Tests.Common
{
    public sealed class AutoMoqIntegrationTestsCustomization : CompositeCustomization
    {
        public AutoMoqIntegrationTestsCustomization()
            : base(new AutoMoqCustomization(), new IntegrationTestsCustomization()) { }
    }
}