using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class UnFreezeAccount : IRequest
    {
        public Guid AccountId { get; }

        public UnFreezeAccount(
            Guid accountId)
        {
            AccountId = accountId;
        }

        public sealed class Validator : AbstractValidator<UnFreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}