namespace Accounts.Api.Dto;

public sealed record OpenAccountDto(
    Guid ClientId,
    decimal InterestRate)
{
    public sealed class Validator : AbstractValidator<OpenAccountDto>
    {
        public Validator()
        {
            RuleFor(dto => dto.ClientId).ValidClientId();
            RuleFor(dto => dto.InterestRate).ValidInterestRate();
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
            RuleFor(dto => dto.Amount).ValidMoney();
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
            RuleFor(dto => dto.Amount).ValidMoney();
        }
    }
}
