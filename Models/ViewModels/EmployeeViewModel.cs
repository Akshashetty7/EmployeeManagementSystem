using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagementSystem.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(10)]
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [Display(Name = "Work Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Join Date")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Base Salary (INR)")]
        [Range(0, 10000000)]
        public decimal BaseSalary { get; set; }

        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; } = string.Empty;

        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        [Display(Name = "Reports To")]
        public int? ReportsToId { get; set; }

        // Dropdowns
        public SelectList? Departments { get; set; }
        public SelectList? Managers { get; set; }
    }

    public class EmployeeListViewModel
    {
        public List<Employee> Employees { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? DepartmentFilter { get; set; }
        public string? StatusFilter { get; set; }
        public List<Department> Departments { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
    }
}
