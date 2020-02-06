using System;
using FluentValidation;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class FreezeAccount : IRequest
    {
        public Guid AccountId { get; }

        public FreezeAccount(
            Guid accountId)
        {
            AccountId = accountId;
        }

        public sealed class Validator : AbstractValidator<FreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}