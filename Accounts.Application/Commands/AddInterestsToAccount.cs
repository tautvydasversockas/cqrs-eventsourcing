using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class AddInterestsToAccount : Command, IRequest
    {
        public Guid AccountId { get; }

        public AddInterestsToAccount(Guid id, Guid accountId) 
            : base(id)
        {
            AccountId = accountId;
        }

        public sealed class Validator : CommandValidator<AddInterestsToAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}