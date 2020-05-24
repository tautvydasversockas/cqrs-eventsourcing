﻿using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class DepositToAccount : Command, IRequest
    {
        public Guid AccountId { get; }
        public decimal Amount { get; }

        public DepositToAccount(Guid id, Guid accountId, decimal amount)
            : base(id)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public sealed class Validator : CommandValidator<DepositToAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
                RuleFor(v => v.Amount).GreaterThan(0);
            }
        }
    }
}