using FluentValidation;

namespace Accounts.Api.Dto
{
    public sealed class WithdrawFromAccountDto
    {
        public decimal Amount { get; }

        public WithdrawFromAccountDto(decimal amount)
        {
            Amount = amount;
        }

        public sealed class Validator : AbstractValidator<WithdrawFromAccountDto>
        {
            public Validator()
            {
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}