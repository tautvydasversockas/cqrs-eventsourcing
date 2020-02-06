using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class AddInterestsToAccount : IRequest
    {
        public Guid AccountId { get; }

        public AddInterestsToAccount(
            Guid accountId)
        {
            AccountId = accountId;
        }

        public sealed class Validator : AbstractValidator<AddInterestsToAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}