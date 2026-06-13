using System.ComponentModel.DataAnnotations;

namespace EmsApi.Validation;

// Problem 8a: JoinDate must not be more than 6 months in the future
public class NotFarFutureDateAttribute : ValidationAttribute
{
    private readonly int _maxMonthsAhead;
    public NotFarFutureDateAttribute(int maxMonthsAhead = 6) => _maxMonthsAhead = maxMonthsAhead;

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is DateTime date && date.Date > DateTime.Today.AddMonths(_maxMonthsAhead))
            return new ValidationResult(
                ErrorMessage ?? $"Date cannot be more than {_maxMonthsAhead} months in the future.");
        return ValidationResult.Success;
    }
}

// Problem 8b: Employee must be at least N years old (DateOfBirth validation)
public class MinAgeAttribute : ValidationAttribute
{
    private readonly int _minAge;
    public MinAgeAttribute(int minAge = 18) => _minAge = minAge;

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is DateTime dob && DateTime.Today < dob.AddYears(_minAge))
            return new ValidationResult(
                ErrorMessage ?? $"Employee must be at least {_minAge} years old.");
        return ValidationResult.Success;
    }
}

// Problem 8c: DateOfBirth must be in the past (cannot be today or future)
public class PastDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is DateTime date && date.Date >= DateTime.Today)
            return new ValidationResult(ErrorMessage ?? "Date of birth must be in the past.");
        return ValidationResult.Success;
    }
}
