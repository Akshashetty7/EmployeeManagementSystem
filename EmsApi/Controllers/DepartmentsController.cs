using EmsApi.Data;
using EmsApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmsApi.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext ctx) => _context = ctx;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var depts = await _context.Departments
            .Include(d => d.Manager)
            .Where(d => d.IsActive)
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                d.Location,
                ManagerName = d.Manager != null ? d.Manager.FirstName + " " + d.Manager.LastName : null,
                EmployeeCount = d.Employees.Count(e => e.Status == EmploymentStatus.Active)
            }).ToListAsync();

        return Ok(depts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _context.Departments
            .Include(d => d.Manager)
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dept == null) return NotFound();
        return Ok(dept);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Create([FromBody] Department dept)
    {
        dept.CreatedAt = DateTime.UtcNow;
        _context.Departments.Add(dept);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dept.Id }, dept);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Update(int id, [FromBody] Department updated)
    {
        var dept = await _context.Departments.FindAsync(id);
        if (dept == null) return NotFound();

        dept.Name = updated.Name;
        dept.Description = updated.Description;
        dept.Location = updated.Location;
        dept.ManagerId = updated.ManagerId;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
