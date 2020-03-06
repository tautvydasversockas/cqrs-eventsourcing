using System;
using Accounts.Application.Common;
using FluentValidation;
using Infrastructure.Messaging;
using MediatR;

namespace Accounts.Application.Commands
{
    public sealed class OpenAccount : ICommand, IRequest<Guid>
    {
        public Guid Id { get; }
        public Guid ClientId { get; }
        public decimal InterestRate { get; }
        public decimal Balance { get; }

        public OpenAccount(
            Guid id,
            Guid clientId,
            decimal interestRate,
            decimal balance)
        {
            Id = id;
            ClientId = clientId;
            InterestRate = interestRate;
            Balance = balance;
        }

        public sealed class Validator : AbstractValidator<OpenAccount>
        {
            public Validator()
            {
                RuleFor(v => v.Id).NotEmpty();
                RuleFor(v => v.ClientId).NotEmpty();
                RuleFor(v => v.InterestRate).ValidInterestRate();
                RuleFor(v => v.Balance).ValidMoney();
            }
        }
    }
}