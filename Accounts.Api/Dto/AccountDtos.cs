namespace Accounts.Api.Dto;

public sealed record OpenAccountDto(
    Guid ClientId,
    decimal InterestRate,
    decimal Balance)
{
    public sealed class Validator : AbstractValidator<OpenAccountDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.ClientId).NotEmpty();
            RuleFor(dto => dto.InterestRate).InclusiveBetween(0, 1);
            RuleFor(dto => dto.Balance).GreaterThanOrEqualTo(0);
        }
    }
}

public sealed record DepositToAccountDto(
    decimal Amount)
{
    public sealed class Validator : AbstractValidator<DepositToAccountDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.Amount).GreaterThan(0);
        }
    }
}

public sealed record WithdrawFromAccountDto(
    decimal Amount)
{
    public sealed class Validator : AbstractValidator<WithdrawFromAccountDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.Amount).GreaterThan(0);
        }
    }
}
