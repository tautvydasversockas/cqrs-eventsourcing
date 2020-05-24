using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class WithdrawFromAccount : Command, IRequest
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public WithdrawFromAccount(Guid id, Guid accountId, decimal amount)
            : base(id)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public sealed class Validator : CommandValidator<WithdrawFromAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}