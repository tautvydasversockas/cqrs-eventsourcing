using FluentValidation;

namespace Accounts.Api.Dto
{
    public sealed class DepositToAccountDto
    {
        public decimal Amount { get; }

        public DepositToAccountDto(decimal amount)
        {
            Amount = amount;
        }

        public sealed class Validator : AbstractValidator<DepositToAccountDto>
        {
            public Validator()
            {
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}