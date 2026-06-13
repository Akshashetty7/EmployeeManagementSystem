using EmsApi.Models;

namespace EmsApi.DTOs;

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
}

public class CreateEmployeeDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public decimal BaseSalary { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? EmergencyContact { get; set; }
    public int? ReportsToId { get; set; }
}

public class UpdateEmployeeDto : CreateEmployeeDto
{
    public EmploymentStatus Status { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
