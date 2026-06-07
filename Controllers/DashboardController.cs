using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var employees = await _context.Employees.Include(e => e.Department).ToListAsync();
            var leaves = await _context.LeaveRequests.Include(e => e.Employee).ThenInclude(e => e!.Department).ToListAsync();
            var audits = await _context.AuditLogs.OrderByDescending(a => a.Timestamp).Take(10).ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalEmployees = employees.Count,
                ActiveEmployees = employees.Count(e => e.Status == EmploymentStatus.Active),
                OnLeaveEmployees = employees.Count(e => e.Status == EmploymentStatus.OnLeave),
                TotalDepartments = await _context.Departments.CountAsync(d => d.IsActive),
                PendingLeaveRequests = leaves.Count(l => l.Status == LeaveStatus.Pending || l.Status == LeaveStatus.ApprovedByManager),
                NewHiresThisMonth = employees.Count(e => e.JoinDate >= monthStart),
                TotalPayroll = employees.Where(e => e.Status == EmploymentStatus.Active).Sum(e => e.BaseSalary),

                DepartmentStats = employees
                    .GroupBy(e => e.Department?.Name ?? "Unknown")
                    .Select(g => new DepartmentStat
                    {
                        Department = g.Key,
                        Count = g.Count(),
                        AverageSalary = g.Any() ? g.Average(e => e.BaseSalary) : 0
                    }).ToList(),

                PendingLeaves = leaves
                    .Where(l => l.Status == LeaveStatus.Pending)
                    .Take(5).ToList(),

                RecentEmployees = employees
                    .OrderByDescending(e => e.JoinDate)
                    .Take(5).ToList(),

                LeaveTypeDistribution = leaves
                    .GroupBy(l => l.LeaveType.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),

                RecentActivities = audits.Select(a => new RecentActivity
                {
                    Icon = GetActionIcon(a.Action),
                    Description = $"{a.Action} on {a.EntityType} #{a.EntityId}",
                    PerformedBy = a.PerformedByName,
                    Timestamp = a.Timestamp,
                    BadgeClass = GetActionBadge(a.Action)
                }).ToList()
            };

            // Monthly headcount for last 6 months
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var label = month.ToString("MMM yy");
                vm.MonthlyHeadcount[label] = employees.Count(e => e.JoinDate <= month);
            }

            return View(vm);
        }

        private static string GetActionIcon(string action) => action switch
        {
            "Create" => "bi-person-plus",
            "Update" => "bi-pencil",
            "Delete" => "bi-trash",
            "Login"  => "bi-box-arrow-in-right",
            _        => "bi-activity"
        };

        private static string GetActionBadge(string action) => action switch
        {
            "Create" => "bg-success",
            "Update" => "bg-primary",
            "Delete" => "bg-danger",
            _        => "bg-secondary"
        };
    }
}
