using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class WithdrawFromAccount : IRequest
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public WithdrawFromAccount(
            Guid accountId,
            decimal amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public sealed class Validator : AbstractValidator<WithdrawFromAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}