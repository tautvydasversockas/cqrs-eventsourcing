using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class FreezeAccount : Command, IRequest
    {
        public Guid AccountId { get; }

        public FreezeAccount(Guid id, Guid accountId)
            : base(id)
        {
            AccountId = accountId;
        }

        public sealed class Validator : CommandValidator<FreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}