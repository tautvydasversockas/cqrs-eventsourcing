using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class DepositToAccount : IRequest
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public DepositToAccount(
            Guid accountId,
            decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public sealed class Validator : AbstractValidator<DepositToAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}