using EmsApi.Data;
using EmsApi.DTOs;
using EmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmsApi.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext ctx) => _context = ctx;

    /// <summary>Get dashboard KPI statistics</summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var employees = await _context.Employees.Include(e => e.Department).ToListAsync();
        var leaves = await _context.LeaveRequests.ToListAsync();

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var deptStats = employees
            .Where(e => e.Status == EmploymentStatus.Active && e.Department != null)
            .GroupBy(e => e.Department!.Name)
            .Select(g => new DeptStatDto(g.Key, g.Count(), g.Sum(e => e.BaseSalary)))
            .OrderByDescending(x => x.Count)
            .ToList();

        var leaveTypeStats = leaves
            .GroupBy(l => l.LeaveType.ToString())
            .Select(g => new LeaveTypeStatDto(g.Key, g.Count()))
            .ToList();

        var recentHires = employees
            .OrderByDescending(e => e.JoinDate)
            .Take(5)
            .Select(e => new RecentHireDto(e.Id, e.FullName, e.JobTitle, e.Department?.Name ?? "", e.JoinDate))
            .ToList();

        var monthlyHeadcount = Enumerable.Range(0, 6)
            .Select(i =>
            {
                var month = now.AddMonths(-i);
                var count = employees.Count(e => e.JoinDate <= new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)));
                return new MonthlyCountDto(month.ToString("MMM yyyy"), count);
            })
            .Reverse()
            .ToList();

        return Ok(new DashboardDto
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employees.Count(e => e.Status == EmploymentStatus.Active),
            OnLeave = employees.Count(e => e.Status == EmploymentStatus.OnLeave),
            TotalDepartments = await _context.Departments.CountAsync(d => d.IsActive),
            PendingLeaves = leaves.Count(l => l.Status == LeaveStatus.Pending),
            NewHiresThisMonth = employees.Count(e => e.JoinDate >= monthStart),
            TotalPayroll = employees.Where(e => e.Status == EmploymentStatus.Active).Sum(e => e.BaseSalary),
            DepartmentStats = deptStats,
            LeaveTypeStats = leaveTypeStats,
            RecentHires = recentHires,
            MonthlyHeadcount = monthlyHeadcount
        });
    }
}
