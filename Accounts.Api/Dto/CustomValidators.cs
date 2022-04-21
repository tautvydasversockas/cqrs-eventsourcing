namespace Accounts.Api.Dto;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, Guid> ValidClientId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder.Custom((element, context) =>
            context.CheckValueObject(() => new ClientId(element)));
    }

    public static IRuleBuilderOptionsConditions<T, decimal> ValidMoney<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.Custom((element, context) =>
            context.CheckValueObject(() => new Money(element)));
    }

    public static IRuleBuilderOptionsConditions<T, decimal> ValidInterestRate<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.Custom((element, context) =>
            context.CheckValueObject(() => new InterestRate(element)));
    }

    private static void CheckValueObject<T, TValueObject>(this ValidationContext<T> context, Func<TValueObject> create)
        where TValueObject : ValueObject
    {
        try
        {
            create();
        }
        catch (ArgumentException e)
        {
            context.AddFailure(e);
        }
    }

    private static void AddFailure<T>(this ValidationContext<T> context, Exception e)
    {
        var failure = new ValidationFailure(context.PropertyName, e.Message);
        context.AddFailure(failure);
    }
}
