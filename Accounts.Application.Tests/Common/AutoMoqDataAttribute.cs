using AutoFixture;
using AutoFixture.Xunit2;

namespace Accounts.Application.Tests.Common
{
    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture().Customize(new AutoMoqIntegrationTestsCustomization())) { }
    }
}