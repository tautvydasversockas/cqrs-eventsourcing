using System;
using FluentValidation;

namespace Accounts.Api.Dto
{
    public sealed class OpenAccountDto
    {
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public OpenAccountDto(Guid clientId, decimal interestRate, decimal balance)
        {
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }

        public sealed class Validator : AbstractValidator<OpenAccountDto>
        {
            public Validator()
            {
                RuleFor(v => v.ClientId).NotEmpty();
                RuleFor(v => v.InterestRate).InclusiveBetween(0, 1);
                RuleFor(v => v.Balance).GreaterThanOrEqualTo(0);
            }
        }
    }
}