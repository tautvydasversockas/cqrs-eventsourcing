using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class OpenAccount : IRequest<Guid>
    {
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public OpenAccount(
            Guid clientId,
            decimal interestRate,
            decimal balance)
        {
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }

        public sealed class Validator : AbstractValidator<OpenAccount>
        {
            public Validator()
            {
                RuleFor(v => v.ClientId).NotEmpty();
                RuleFor(v => v.InterestRate).GreaterThanOrEqualTo(0);
                RuleFor(v => v.Balance).GreaterThanOrEqualTo(0);
            }
        }
    }
}