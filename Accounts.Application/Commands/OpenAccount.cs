using System;
using Accounts.Application.Common;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class OpenAccount : Command, IRequest<Guid>
    {
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public OpenAccount(Guid id, Guid clientId, decimal interestRate, decimal balance)
            : base(id)
        {
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }

        public sealed class Validator : CommandValidator<OpenAccount>
        {
            public Validator()
            {
                RuleFor(v => v.ClientId).NotEmpty();
                RuleFor(v => v.InterestRate).ValidInterestRate();
                RuleFor(v => v.Balance).GreaterThanOrEqualTo(0);
            }
        }
    }
}