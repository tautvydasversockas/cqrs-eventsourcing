using System;
using FluentValidation;
using Infrastructure.Messaging;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class UnFreezeAccount : ICommand, IRequest
    {
        public Guid Id { get; }
        public Guid AccountId { get; }

        public UnFreezeAccount(
            Guid id,
            Guid accountId)
        {
            Id = id;
            AccountId = accountId;
        }

        public sealed class Validator : AbstractValidator<UnFreezeAccount>
        {
            public Validator()
            {
                RuleFor(v => v.Id).NotEmpty();
                RuleFor(v => v.AccountId).NotEmpty();
            }
        }
    }
}