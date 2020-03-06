using System;
using Accounts.Domain;
using FluentValidation;
using FluentValidation.Validators;

namespace Accounts.Application.Common
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderInitial<T, decimal> ValidMoney<T>(this IRuleBuilderInitial<T, decimal> builder) =>
            builder.Custom((value, ctx) => ctx.Execute(() => new Money(value)));

        public static IRuleBuilderInitial<T, decimal> ValidInterestRate<T>(this IRuleBuilderInitial<T, decimal> builder) =>
            builder.Custom((value, ctx) => ctx.Execute(() => new InterestRate(value)));

        private static void Execute(this CustomContext ctx, Action action)
        {
            try
            {
                action();
            }
            catch (ArgumentException e)
            {
                ctx.AddFailure($"{e.Message}.");
            }
        }
    }
}