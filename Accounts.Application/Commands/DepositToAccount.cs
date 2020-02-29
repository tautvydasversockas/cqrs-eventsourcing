using System;
using FluentValidation;
using Infrastructure.Messaging;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class DepositToAccount : ICommand, IRequest
    {
        public Guid Id { get; }
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public DepositToAccount(
            Guid id,
            Guid accountId,
            decimal amount)
        {
            Id = id;
            AccountId = accountId;
            Amount = amount;
        }

        public sealed class Validator : AbstractValidator<DepositToAccount>
        {
            public Validator()
            {
                RuleFor(v => v.Id).NotEmpty();
                RuleFor(v => v.AccountId).NotEmpty();
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}