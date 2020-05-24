using System;
using FluentValidation;

namespace Accounts.Application.Commands
{
    public abstract class Command
    {
        public Guid Id { get; }

        protected Command(Guid id)
        {
            Id = id;
        }

        public class CommandValidator<T> : AbstractValidator<T> where T : Command
        {
            public CommandValidator()
            {
                RuleFor(v => v.Id).NotEmpty();
            }
        }
    }
}