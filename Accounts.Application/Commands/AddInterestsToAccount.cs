using System;
using FluentValidation;
using Infrastructure.Messaging;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class AddInterestsToAccount : ICommand, IRequest
    {
        public Guid Id { get; }
        public Guid AccountId { get; }

        public AddInterestsToAccount(
            Guid id,
            Guid accountId)
        {
            Id = id;
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