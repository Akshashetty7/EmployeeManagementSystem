using EmsApi.Data;
using EmsApi.DTOs;
using EmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmsApi.Controllers;

[ApiController]
[Route("api/leave")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LeaveController(ApplicationDbContext ctx, UserManager<ApplicationUser> um)
    {
        _context = ctx;
        _userManager = um;
    }

    /// <summary>Get leave requests (scoped by role)</summary>
    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);

        var query = _context.LeaveRequests
            .Include(l => l.Employee).ThenInclude(e => e!.Department)
            .AsQueryable();

        if (User.IsInRole("Employee") && !User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            if (user?.EmployeeId != null)
                query = query.Where(l => l.EmployeeId == user.EmployeeId);
        }
        else if (User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            if (user?.EmployeeId != null)
            {
                var teamIds = await _context.Employees
                    .Where(e => e.ReportsToId == user.EmployeeId)
                    .Select(e => e.Id).ToListAsync();
                teamIds.Add(user.EmployeeId.Value);
                query = query.Where(l => teamIds.Contains(l.EmployeeId));
            }
        }

        var leaves = await query.OrderByDescending(l => l.AppliedOn)
            .Select(l => new LeaveRequestDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee!.FirstName + " " + l.Employee.LastName,
                Department = l.Employee.Department!.Name,
                LeaveType = l.LeaveType.ToString(),
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                TotalDays = (l.EndDate - l.StartDate).Days + 1,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                ManagerRemarks = l.ManagerRemarks,
                HRRemarks = l.HRRemarks,
                AppliedOn = l.AppliedOn
            }).ToListAsync();

        return Ok(leaves);
    }

    /// <summary>Apply for leave</summary>
    [HttpPost("apply")]
    public async Task<IActionResult> Apply([FromBody] ApplyLeaveDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);

        int employeeId;
        if ((User.IsInRole("Admin") || User.IsInRole("HR")) && dto.EmployeeId.HasValue)
            employeeId = dto.EmployeeId.Value;
        else if (user?.EmployeeId != null)
            employeeId = user.EmployeeId.Value;
        else
            return BadRequest(new { message = "No employee record linked to your account." });

        var leave = new LeaveRequest
        {
            EmployeeId = employeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending,
            AppliedOn = DateTime.UtcNow
        };

        _context.LeaveRequests.Add(leave);
        await _context.SaveChangesAsync();

        return CreatedAtAction(null, new { leave.Id }, new { leave.Id, leave.Status });
    }

    /// <summary>Review (approve/reject) a leave request</summary>
    [HttpPost("{id}/review")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewLeaveDto dto)
    {
        var leave = await _context.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(l => l.Id == id);
        if (leave == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var approve = dto.Action.ToLower() == "approve";

        if (User.IsInRole("Manager") && !User.IsInRole("HR") && !User.IsInRole("Admin"))
        {
            leave.Status = approve ? LeaveStatus.ApprovedByManager : LeaveStatus.RejectedByManager;
            leave.ManagerRemarks = dto.Remarks;
            leave.ReviewedByManagerId = userId;
            leave.ManagerReviewDate = DateTime.UtcNow;
        }
        else
        {
            leave.Status = approve ? LeaveStatus.ApprovedByHR : LeaveStatus.RejectedByHR;
            leave.HRRemarks = dto.Remarks;
            leave.ReviewedByHRId = userId;
            leave.HRReviewDate = DateTime.UtcNow;

            if (approve && leave.Employee != null &&
                leave.StartDate <= DateTime.Today && leave.EndDate >= DateTime.Today)
            {
                leave.Employee.Status = EmploymentStatus.OnLeave;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { leave.Id, Status = leave.Status.ToString() });
    }

    /// <summary>Cancel a pending leave request</summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var leave = await _context.LeaveRequests.FindAsync(id);
        if (leave == null) return NotFound();
        if (leave.Status != LeaveStatus.Pending)
            return BadRequest(new { message = "Only pending requests can be cancelled." });

        leave.Status = LeaveStatus.Cancelled;
        await _context.SaveChangesAsync();
        return Ok(new { leave.Id, Status = "Cancelled" });
    }
}
