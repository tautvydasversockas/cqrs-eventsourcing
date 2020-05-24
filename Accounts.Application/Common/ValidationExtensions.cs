using System;
using Accounts.Domain;
using FluentValidation;
using FluentValidation.Validators;

namespace Accounts.Application.Common
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderInitial<T, decimal> ValidInterestRate<T>(this IRuleBuilderInitial<T, decimal> builder) =>
            builder.Custom((value, context) => context.Execute(() => new InterestRate(value)));

        private static void Execute(this CustomContext context, Action action)
        {
            try
            {
                action();
            }
            catch (ArgumentException e)
            {
                context.AddFailure(e.Message);
            }
        }
    }
}