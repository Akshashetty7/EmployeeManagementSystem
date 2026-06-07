using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var depts = await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .ToListAsync();
            return View(depts);
        }

        public IActionResult Create() => View(new Department());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid) return View(dept);
            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Department created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return NotFound();
            ViewBag.Managers = await _context.Employees
                .Where(e => e.Status == EmploymentStatus.Active)
                .OrderBy(e => e.FirstName).ToListAsync();
            return View(dept);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department dept)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Managers = await _context.Employees.Where(e => e.Status == EmploymentStatus.Active).ToListAsync();
                return View(dept);
            }
            _context.Update(dept);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Department updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
