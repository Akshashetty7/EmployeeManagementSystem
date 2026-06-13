using System.ComponentModel.DataAnnotations;
using EmsApi.Models;
using EmsApi.Validation;

namespace EmsApi.DTOs;

// Problem 2 + 8: Full validation on every inbound employee payload.
// [ApiController] automatically returns HTTP 400 with error details when ModelState is invalid —
// no manual ModelState.IsValid check needed in controllers.
public class CreateEmployeeDto : IValidatableObject
{
    [Required(ErrorMessage = "Employee code is required.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Employee code must be 3–20 characters.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Employee code may only contain uppercase letters, digits, and hyphens.")]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be 2–100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be 2–100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public Gender Gender { get; set; }

    // Problem 8: DateOfBirth must be in the past and employee must be >= 18
    [Required(ErrorMessage = "Date of birth is required.")]
    [PastDate(ErrorMessage = "Date of birth must be in the past.")]
    [MinAge(18, ErrorMessage = "Employee must be at least 18 years old.")]
    public DateTime DateOfBirth { get; set; }

    // Problem 8: JoinDate cannot be absurdly far in the future
    [Required(ErrorMessage = "Join date is required.")]
    [NotFarFutureDate(6, ErrorMessage = "Join date cannot be more than 6 months in the future.")]
    public DateTime JoinDate { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Job title must be 2–150 characters.")]
    public string JobTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "A valid department must be selected.")]
    public int DepartmentId { get; set; }

    // Problem 2: Salary must be a positive, realistic value
    [Required(ErrorMessage = "Base salary is required.")]
    [Range(1000, 10_000_000, ErrorMessage = "Salary must be between 1,000 and 10,000,000.")]
    public decimal BaseSalary { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(200)]
    public string? EmergencyContact { get; set; }

    // Problem 3: NationalId exposed in DTO so the controller can deduplicate before insert
    [StringLength(50)]
    public string? NationalId { get; set; }

    public int? ReportsToId { get; set; }

    // Problem 8: Cross-field validation — JoinDate must be after DateOfBirth
    // IValidatableObject runs after all individual [Attribute] checks pass
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (JoinDate.Date <= DateOfBirth.Date)
            yield return new ValidationResult(
                "Join date must be after date of birth.",
                new[] { nameof(JoinDate) });

        if (JoinDate.Date < DateOfBirth.AddYears(15).Date)
            yield return new ValidationResult(
                "Join date implies the employee was under 15 at the time of joining.",
                new[] { nameof(JoinDate) });
    }
}

public class UpdateEmployeeDto : CreateEmployeeDto
{
    public EmploymentStatus Status { get; set; }
    // Problem 18: client must echo back the RowVersion it received on GET.
    // If another user saved between the GET and this PUT, versions differ → 409 Conflict.
    public string? RowVersion { get; set; }
}

public class EmployeeListDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}

public class EmployeeDetailDto : EmployeeListDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public decimal BaseSalary { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContact { get; set; }
    public string? ReportsToName { get; set; }
    public int DepartmentId { get; set; }
    public string? NationalId { get; set; }
    // Problem 18: sent to client so they can echo it back on PUT for concurrency check
    public string? RowVersion { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
