using AutoFixture.Xunit2;

namespace Accounts.Application.Tests.Common
{
    public sealed class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] objects)
            : base(new AutoMoqDataAttribute(), objects) { }
    }
}