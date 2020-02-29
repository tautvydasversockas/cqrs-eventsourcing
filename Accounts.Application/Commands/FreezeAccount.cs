using System;
using FluentValidation;
using Infrastructure.Messaging;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class FreezeAccount : ICommand, IRequest
    {
        public Guid Id { get; }
        public Guid AccountId { get; }

        public FreezeAccount(
            Guid id,
            Guid accountId)
        {
            Id = id;
            AccountId = accountId;
        }

        public sealed class Validator : AbstractValidator<FreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.Id).NotEmpty();
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}