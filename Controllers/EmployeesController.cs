using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Models.ViewModels;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _audit;
        private readonly IExportService _export;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notifications;

        public EmployeesController(ApplicationDbContext context, IAuditService audit,
            IExportService export, UserManager<ApplicationUser> userManager,
            INotificationService notifications)
        {
            _context = context;
            _audit = audit;
            _export = export;
            _userManager = userManager;
            _notifications = notifications;
        }

        public async Task<IActionResult> Index(string? search, int? deptId, string? status, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Employees.Include(e => e.Department).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search)
                    || e.Email.Contains(search) || e.EmployeeCode.Contains(search) || e.JobTitle.Contains(search));

            if (deptId.HasValue)
                query = query.Where(e => e.DepartmentId == deptId.Value);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EmploymentStatus>(status, out var s))
                query = query.Where(e => e.Status == s);

            int total = await query.CountAsync();
            var employees = await query.OrderBy(e => e.FirstName)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var vm = new EmployeeListViewModel
            {
                Employees = employees,
                SearchTerm = search,
                DepartmentFilter = deptId,
                StatusFilter = status,
                Departments = await _context.Departments.Where(d => d.IsActive).ToListAsync(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                TotalCount = total,
                PageSize = pageSize
            };
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var emp = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.ReportsTo)
                .Include(e => e.Subordinates)
                .Include(e => e.LeaveRequests.OrderByDescending(l => l.CreatedAt).Take(5))
                .Include(e => e.AuditLogs.OrderByDescending(a => a.Timestamp).Take(10))
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emp == null) return NotFound();
            return View(emp);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            var vm = new EmployeeViewModel
            {
                JoinDate = DateTime.Today,
                DateOfBirth = DateTime.Today.AddYears(-25),
                Departments = new SelectList(await _context.Departments.Where(d => d.IsActive).ToListAsync(), "Id", "Name"),
                Managers = new SelectList(await _context.Employees.Where(e => e.Status == EmploymentStatus.Active).ToListAsync(), "Id", "FullName")
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(EmployeeViewModel vm)
        {
            vm.Departments = new SelectList(await _context.Departments.Where(d => d.IsActive).ToListAsync(), "Id", "Name");
            vm.Managers = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");

            if (!ModelState.IsValid) return View(vm);

            if (await _context.Employees.AnyAsync(e => e.EmployeeCode == vm.EmployeeCode))
            {
                ModelState.AddModelError("EmployeeCode", "Employee code already exists.");
                return View(vm);
            }

            if (await _context.Employees.AnyAsync(e => e.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View(vm);
            }

            var employee = new Employee
            {
                EmployeeCode = vm.EmployeeCode,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Phone = vm.Phone,
                Gender = vm.Gender,
                DateOfBirth = vm.DateOfBirth,
                JoinDate = vm.JoinDate,
                JobTitle = vm.JobTitle,
                DepartmentId = vm.DepartmentId,
                BaseSalary = vm.BaseSalary,
                Status = vm.Status,
                Address = vm.Address,
                City = vm.City,
                PostalCode = vm.PostalCode,
                EmergencyContact = vm.EmergencyContact,
                NationalId = vm.NationalId,
                ReportsToId = vm.ReportsToId,
                CreatedBy = User.Identity?.Name,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _audit.LogAsync("Employee", employee.Id, "Create", null, new { employee.EmployeeCode, employee.FullName, employee.Email },
                user?.Id ?? "system", user?.FullName ?? "System",
                HttpContext.Connection.RemoteIpAddress?.ToString(), employee.Id);

            await _notifications.SendWelcomeEmailAsync(employee.Email, employee.FullName, "Welcome@123!");

            TempData["Success"] = $"Employee {employee.FullName} ({employee.EmployeeCode}) added successfully.";
            return RedirectToAction(nameof(Details), new { id = employee.Id });
        }

        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            var vm = new EmployeeViewModel
            {
                Id = emp.Id,
                EmployeeCode = emp.EmployeeCode,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.Email,
                Phone = emp.Phone,
                Gender = emp.Gender,
                DateOfBirth = emp.DateOfBirth,
                JoinDate = emp.JoinDate,
                JobTitle = emp.JobTitle,
                DepartmentId = emp.DepartmentId,
                BaseSalary = emp.BaseSalary,
                Status = emp.Status,
                Address = emp.Address,
                City = emp.City,
                PostalCode = emp.PostalCode,
                EmergencyContact = emp.EmergencyContact,
                NationalId = emp.NationalId,
                ReportsToId = emp.ReportsToId,
                Departments = new SelectList(await _context.Departments.Where(d => d.IsActive).ToListAsync(), "Id", "Name", emp.DepartmentId),
                Managers = new SelectList(await _context.Employees.Where(e => e.Id != id && e.Status == EmploymentStatus.Active).ToListAsync(), "Id", "FullName", emp.ReportsToId)
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel vm)
        {
            vm.Departments = new SelectList(await _context.Departments.Where(d => d.IsActive).ToListAsync(), "Id", "Name");
            vm.Managers = new SelectList(await _context.Employees.Where(e => e.Id != id).ToListAsync(), "Id", "FullName");

            if (!ModelState.IsValid) return View(vm);

            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            var oldSnapshot = new { emp.JobTitle, emp.DepartmentId, emp.BaseSalary, emp.Status, emp.Phone };

            emp.FirstName = vm.FirstName;
            emp.LastName = vm.LastName;
            emp.Phone = vm.Phone;
            emp.Gender = vm.Gender;
            emp.DateOfBirth = vm.DateOfBirth;
            emp.JoinDate = vm.JoinDate;
            emp.JobTitle = vm.JobTitle;
            emp.DepartmentId = vm.DepartmentId;
            emp.BaseSalary = vm.BaseSalary;
            emp.Status = vm.Status;
            emp.Address = vm.Address;
            emp.City = vm.City;
            emp.PostalCode = vm.PostalCode;
            emp.EmergencyContact = vm.EmergencyContact;
            emp.ReportsToId = vm.ReportsToId;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.UpdatedBy = User.Identity?.Name;

            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _audit.LogAsync("Employee", emp.Id, "Update", oldSnapshot,
                new { emp.JobTitle, emp.DepartmentId, emp.BaseSalary, emp.Status, emp.Phone },
                user?.Id ?? "system", user?.FullName ?? "System",
                HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);

            TempData["Success"] = "Employee record updated successfully.";
            return RedirectToAction(nameof(Details), new { id = emp.Id });
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            emp.Status = EmploymentStatus.Terminated;
            emp.UpdatedAt = DateTime.UtcNow;
            emp.UpdatedBy = User.Identity?.Name;
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _audit.LogAsync("Employee", id, "Terminate", new { emp.Status }, new { Status = "Terminated" },
                user?.Id ?? "system", user?.FullName ?? "System");

            TempData["Success"] = $"Employee {emp.FullName} has been terminated.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ExportExcel(string? search, int? deptId, string? status)
        {
            var query = _context.Employees.Include(e => e.Department).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search));
            if (deptId.HasValue)
                query = query.Where(e => e.DepartmentId == deptId.Value);
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EmploymentStatus>(status, out var s))
                query = query.Where(e => e.Status == s);

            var employees = await query.OrderBy(e => e.Department!.Name).ThenBy(e => e.FirstName).ToListAsync();
            var bytes = _export.ExportEmployeesToExcel(employees);

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NexaCorp_Employees_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
