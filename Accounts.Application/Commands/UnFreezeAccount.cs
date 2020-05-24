using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class UnFreezeAccount : Command, IRequest
    {
        public Guid AccountId { get; }

        public UnFreezeAccount(Guid id, Guid accountId)
            : base(id)
        {
            AccountId = accountId;
        }

        public sealed class Validator : CommandValidator<UnFreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}