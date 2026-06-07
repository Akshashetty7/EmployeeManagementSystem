using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _audit;
        private readonly INotificationService _notifications;
        private readonly IExportService _export;

        public LeaveController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IAuditService audit, INotificationService notifications, IExportService export)
        {
            _context = context;
            _userManager = userManager;
            _audit = audit;
            _notifications = notifications;
            _export = export;
        }

        public async Task<IActionResult> Index(string? status, int? empId)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdminOrHR = User.IsInRole("Admin") || User.IsInRole("HR");
            var isManager = User.IsInRole("Manager");

            var query = _context.LeaveRequests
                .Include(l => l.Employee).ThenInclude(e => e!.Department)
                .AsQueryable();

            if (!isAdminOrHR && !isManager)
                query = query.Where(l => l.Employee!.Email == user!.Email);
            else if (isManager && !isAdminOrHR && user?.EmployeeId != null)
            {
                var myTeam = await _context.Employees
                    .Where(e => e.ReportsToId == user.EmployeeId)
                    .Select(e => e.Id).ToListAsync();
                query = query.Where(l => myTeam.Contains(l.EmployeeId) || l.EmployeeId == user.EmployeeId);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<LeaveStatus>(status, out var s))
                query = query.Where(l => l.Status == s);
            if (empId.HasValue)
                query = query.Where(l => l.EmployeeId == empId.Value);

            var leaves = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
            ViewBag.StatusFilter = status;
            ViewBag.IsAdminOrHR = isAdminOrHR;
            ViewBag.IsManager = isManager;
            return View(leaves);
        }

        public async Task<IActionResult> Apply()
        {
            var user = await _userManager.GetUserAsync(User);
            var emp = user?.EmployeeId.HasValue == true
                ? await _context.Employees.FindAsync(user.EmployeeId)
                : null;

            ViewBag.Employee = emp;
            ViewBag.Employees = await _context.Employees
                .Where(e => e.Status == EmploymentStatus.Active)
                .OrderBy(e => e.FirstName).ToListAsync();
            ViewBag.IsHRorAdmin = User.IsInRole("Admin") || User.IsInRole("HR");
            return View(new LeaveRequest { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(LeaveRequest model)
        {
            ModelState.Remove("Employee");
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.Employees.Where(e => e.Status == EmploymentStatus.Active).ToListAsync();
                ViewBag.IsHRorAdmin = User.IsInRole("Admin") || User.IsInRole("HR");
                return View(model);
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
                ViewBag.Employees = await _context.Employees.Where(e => e.Status == EmploymentStatus.Active).ToListAsync();
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            model.Status = LeaveStatus.Pending;
            _context.LeaveRequests.Add(model);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            await _audit.LogAsync("LeaveRequest", model.Id, "Apply", null,
                new { model.LeaveType, model.StartDate, model.EndDate, model.Status },
                user?.Id ?? "system", user?.FullName ?? "System");

            TempData["Success"] = "Leave request submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Review(int id)
        {
            var leave = await _context.LeaveRequests
                .Include(l => l.Employee).ThenInclude(e => e!.Department)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave == null) return NotFound();
            return View(leave);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Admin,HR,Manager")]
        public async Task<IActionResult> Review(int id, string action, string remarks)
        {
            var leave = await _context.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.Id == id);
            if (leave == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var oldStatus = leave.Status;

            if (User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("HR"))
            {
                leave.Status = action == "approve" ? LeaveStatus.ApprovedByManager : LeaveStatus.RejectedByManager;
                leave.ManagerRemarks = remarks;
                leave.ReviewedByManagerId = user?.Id;
                leave.ManagerReviewDate = DateTime.UtcNow;
            }
            else
            {
                leave.Status = action == "approve" ? LeaveStatus.ApprovedByHR : LeaveStatus.RejectedByHR;
                leave.HRRemarks = remarks;
                leave.ReviewedByHRId = user?.Id;
                leave.HRReviewDate = DateTime.UtcNow;

                // Update employee status if fully approved
                if (leave.Status == LeaveStatus.ApprovedByHR && leave.Employee != null)
                {
                    var emp = await _context.Employees.FindAsync(leave.EmployeeId);
                    if (emp != null && leave.StartDate <= DateTime.Today && leave.EndDate >= DateTime.Today)
                    {
                        emp.Status = EmploymentStatus.OnLeave;
                        emp.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            leave.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _audit.LogAsync("LeaveRequest", leave.Id, "Review",
                new { Status = oldStatus.ToString() }, new { Status = leave.Status.ToString(), Remarks = remarks },
                user?.Id ?? "system", user?.FullName ?? "System");

            if (leave.Employee != null)
                await _notifications.SendLeaveStatusUpdateAsync(
                    leave.Employee.Email, leave.Employee.FullName,
                    leave.LeaveType.ToString(), leave.Status.ToString(), remarks);

            TempData["Success"] = $"Leave request {action}d successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null) return NotFound();

            if (leave.Status != LeaveStatus.Pending)
            {
                TempData["Error"] = "Only pending requests can be cancelled.";
                return RedirectToAction(nameof(Index));
            }

            leave.Status = LeaveStatus.Cancelled;
            leave.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Leave request cancelled.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ExportExcel()
        {
            var leaves = await _context.LeaveRequests
                .Include(l => l.Employee).ThenInclude(e => e!.Department)
                .OrderByDescending(l => l.CreatedAt).ToListAsync();

            var bytes = _export.ExportLeaveReportToExcel(leaves);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"NexaCorp_LeaveReport_{DateTime.Today:yyyyMMdd}.xlsx");
        }
    }
}
