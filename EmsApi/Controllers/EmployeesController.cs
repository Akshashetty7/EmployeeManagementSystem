using EmsApi.Data;
using EmsApi.DTOs;
using EmsApi.Models;
using EmsApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmsApi.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _audit;

    public EmployeesController(ApplicationDbContext ctx, IAuditService audit)
    {
        _context = ctx;
        _audit = audit;
    }

    /// <summary>Get paginated employee list with optional search and filters</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? departmentId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Employees.Include(e => e.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search) ||
                                     e.Email.Contains(search) || e.EmployeeCode.Contains(search) ||
                                     e.JobTitle.Contains(search));

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EmploymentStatus>(status, out var s))
            query = query.Where(e => e.Status == s);

        var total = await query.CountAsync();
        var items = await query.OrderBy(e => e.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EmployeeListDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                JobTitle = e.JobTitle,
                Department = e.Department!.Name,
                Status = e.Status.ToString(),
                City = e.City ?? "",
                JoinDate = e.JoinDate
            }).ToListAsync();

        return Ok(new PagedResult<EmployeeListDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        });
    }

    /// <summary>Get single employee details</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id)
    {
        var e = await _context.Employees
            .Include(x => x.Department)
            .Include(x => x.ReportsTo)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e == null) return NotFound();

        return Ok(new EmployeeDetailDto
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FullName = e.FullName,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            Gender = e.Gender.ToString(),
            DateOfBirth = e.DateOfBirth,
            Age = e.Age,
            JoinDate = e.JoinDate,
            JobTitle = e.JobTitle,
            Department = e.Department?.Name ?? "",
            DepartmentId = e.DepartmentId,
            BaseSalary = e.BaseSalary,
            Status = e.Status.ToString(),
            Address = e.Address,
            City = e.City ?? "",
            PostalCode = e.PostalCode,
            EmergencyContact = e.EmergencyContact,
            ReportsToName = e.ReportsTo != null ? e.ReportsTo.FirstName + " " + e.ReportsTo.LastName : null
        });
    }

    /// <summary>Create a new employee (Admin / HR only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        if (await _context.Employees.AnyAsync(e => e.EmployeeCode == dto.EmployeeCode))
            return BadRequest(new { message = "Employee code already exists." });

        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
            return BadRequest(new { message = "Email already in use." });

        var emp = new Employee
        {
            EmployeeCode = dto.EmployeeCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            JoinDate = dto.JoinDate,
            JobTitle = dto.JobTitle,
            DepartmentId = dto.DepartmentId,
            BaseSalary = dto.BaseSalary,
            Address = dto.Address,
            City = dto.City,
            PostalCode = dto.PostalCode,
            EmergencyContact = dto.EmergencyContact,
            ReportsToId = dto.ReportsToId,
            CreatedBy = User.FindFirstValue(ClaimTypes.Email)
        };

        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();

        await _audit.LogAsync("Employee", emp.Id, "Create", null, new { emp.EmployeeCode, emp.FirstName, emp.LastName, emp.Email },
            User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            User.FindFirstValue("fullName") ?? "Unknown",
            HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);

        return CreatedAtAction(nameof(GetById), new { id = emp.Id }, new { emp.Id, emp.EmployeeCode, emp.FullName });
    }

    /// <summary>Update employee (Admin / HR / Manager)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();

        var oldSnapshot = new { emp.JobTitle, emp.BaseSalary, emp.Status, emp.DepartmentId };

        emp.FirstName = dto.FirstName;
        emp.LastName = dto.LastName;
        emp.Email = dto.Email;
        emp.Phone = dto.Phone;
        emp.Gender = dto.Gender;
        emp.DateOfBirth = dto.DateOfBirth;
        emp.JoinDate = dto.JoinDate;
        emp.JobTitle = dto.JobTitle;
        emp.DepartmentId = dto.DepartmentId;
        emp.BaseSalary = dto.BaseSalary;
        emp.Status = dto.Status;
        emp.Address = dto.Address;
        emp.City = dto.City;
        emp.PostalCode = dto.PostalCode;
        emp.EmergencyContact = dto.EmergencyContact;
        emp.ReportsToId = dto.ReportsToId;
        emp.UpdatedBy = User.FindFirstValue(ClaimTypes.Email);
        emp.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _audit.LogAsync("Employee", emp.Id, "Update", oldSnapshot,
            new { emp.JobTitle, emp.BaseSalary, emp.Status, emp.DepartmentId },
            User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            User.FindFirstValue("fullName") ?? "Unknown",
            HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);

        return NoContent();
    }

    /// <summary>Terminate employee — soft delete (Admin only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Terminate(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();

        emp.Status = EmploymentStatus.Terminated;
        emp.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _audit.LogAsync("Employee", emp.Id, "Terminate", null, new { Status = "Terminated" },
            User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            User.FindFirstValue("fullName") ?? "Unknown",
            HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);

        return NoContent();
    }
}
