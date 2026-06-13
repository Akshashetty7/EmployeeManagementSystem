using ClosedXML.Excel;
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
    private readonly IEncryptionService _encryption;

    public EmployeesController(ApplicationDbContext ctx, IAuditService audit, IEncryptionService encryption)
    {
        _context = ctx;
        _audit = audit;
        _encryption = encryption;
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
            ReportsToName = e.ReportsTo != null ? e.ReportsTo.FirstName + " " + e.ReportsTo.LastName : null,
            NationalId = _encryption.Decrypt(e.NationalId),   // Problem 19: decrypt before sending
            RowVersion = e.RowVersion != null                  // Problem 18: send to client for concurrency
                ? Convert.ToBase64String(e.RowVersion) : null
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

        // Problem 3 + 19: check hash for duplicate, then encrypt the actual value before saving
        if (!string.IsNullOrWhiteSpace(dto.NationalId))
        {
            var hash = _encryption.Hash(dto.NationalId);
            if (await _context.Employees.AnyAsync(e => e.NationalIdHash == hash))
                return Conflict(new { message = "An employee with this National ID already exists." });
        }

        var emp = MapDtoToEmployee(dto, _encryption);
        emp.CreatedBy = User.FindFirstValue(ClaimTypes.Email);

        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();

        await _audit.LogAsync("Employee", emp.Id, "Create", null,
            new { emp.EmployeeCode, emp.FirstName, emp.LastName, emp.Email },
            User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            User.FindFirstValue("fullName") ?? "Unknown",
            HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);

        return CreatedAtAction(nameof(GetById), new { id = emp.Id }, new { emp.Id, emp.EmployeeCode, emp.FullName });
    }

    /// <summary>Update employee (Admin / HR / Manager — managers restricted to their direct reports)</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();

        // Managers may only update employees who report directly to them
        if (User.IsInRole("Manager") && !User.IsInRole("Admin") && !User.IsInRole("HR"))
        {
            var managerEmployeeId = User.FindFirstValue("employeeId");
            if (!int.TryParse(managerEmployeeId, out var mgId) || emp.ReportsToId != mgId)
                return Forbid();
        }

        var oldSnapshot = new { emp.JobTitle, emp.BaseSalary, emp.Status, emp.DepartmentId };

        // Problem 18: if the client sent a RowVersion, wire it into EF Core's change tracker
        // as the expected original value. EF Core adds it to the UPDATE WHERE clause.
        // If another user saved between this GET and PUT, the DB version is different → 0 rows
        // affected → DbUpdateConcurrencyException → we return 409 instead of silent overwrite.
        if (!string.IsNullOrEmpty(dto.RowVersion))
        {
            try
            {
                _context.Entry(emp).Property(x => x.RowVersion).OriginalValue =
                    Convert.FromBase64String(dto.RowVersion);
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid RowVersion format." });
            }
        }

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

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new
            {
                message = "This record was modified by another user after you opened it. " +
                          "Please reload the employee and apply your changes again."
            });
        }

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

    /// <summary>Export all employees to Excel (Admin / HR)</summary>
    [HttpGet("export")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> ExportExcel()
    {
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.ReportsTo)
            .OrderBy(e => e.FirstName)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Employees");

        var headers = new[]
        {
            "Code", "First Name", "Last Name", "Email", "Phone", "Gender",
            "Date of Birth", "Join Date", "Job Title", "Department",
            "Salary", "Status", "City", "Reports To"
        };

        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }

        for (var row = 0; row < employees.Count; row++)
        {
            var e = employees[row];
            var r = row + 2;
            ws.Cell(r, 1).Value = e.EmployeeCode;
            ws.Cell(r, 2).Value = e.FirstName;
            ws.Cell(r, 3).Value = e.LastName;
            ws.Cell(r, 4).Value = e.Email;
            ws.Cell(r, 5).Value = e.Phone ?? "";
            ws.Cell(r, 6).Value = e.Gender.ToString();
            ws.Cell(r, 7).Value = e.DateOfBirth.ToString("yyyy-MM-dd");
            ws.Cell(r, 8).Value = e.JoinDate.ToString("yyyy-MM-dd");
            ws.Cell(r, 9).Value = e.JobTitle;
            ws.Cell(r, 10).Value = e.Department?.Name ?? "";
            ws.Cell(r, 11).Value = e.BaseSalary;
            ws.Cell(r, 12).Value = e.Status.ToString();
            ws.Cell(r, 13).Value = e.City ?? "";
            ws.Cell(r, 14).Value = e.ReportsTo != null ? e.ReportsTo.FullName : "";
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employees_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }

    /// <summary>
    /// Bulk import employees from a CSV file (Admin / HR).
    /// CSV header row required:
    /// EmployeeCode,FirstName,LastName,Email,Phone,Gender,DateOfBirth,JoinDate,JobTitle,DepartmentId,BaseSalary,Address,City,PostalCode,EmergencyContact,ReportsToId
    /// </summary>
    [HttpPost("import")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Only .csv files are accepted." });

        var results = new List<EmployeeImportRow>();
        var newEmployees = new List<Employee>();

        using var reader = new StreamReader(file.OpenReadStream());
        var headerLine = await reader.ReadLineAsync();
        if (headerLine == null)
            return BadRequest(new { message = "CSV file is empty." });

        var rowNumber = 1;
        while (!reader.EndOfStream)
        {
            rowNumber++;
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var cols = ParseCsvLine(line);
            var result = new EmployeeImportRow { RowNumber = rowNumber };

            try
            {
                if (cols.Length < 11)
                    throw new FormatException("Row has fewer columns than expected (minimum 11).");

                result.EmployeeCode = cols[0].Trim();
                result.Name = $"{cols[1].Trim()} {cols[2].Trim()}";

                if (await _context.Employees.AnyAsync(e => e.EmployeeCode == result.EmployeeCode))
                    throw new InvalidOperationException($"Employee code '{result.EmployeeCode}' already exists.");

                var email = cols[3].Trim();
                if (await _context.Employees.AnyAsync(e => e.Email == email))
                    throw new InvalidOperationException($"Email '{email}' already in use.");

                // Problem 3 + 19: hash-based dedup, value will be encrypted before save
                var nationalId = cols.Length > 15 && !string.IsNullOrWhiteSpace(cols[15].Trim())
                    ? cols[15].Trim() : null;
                if (nationalId != null)
                {
                    var hash = _encryption.Hash(nationalId);
                    if (await _context.Employees.AnyAsync(e => e.NationalIdHash == hash))
                        throw new InvalidOperationException($"National ID already exists for another employee.");
                }

                var emp = new Employee
                {
                    EmployeeCode = result.EmployeeCode,
                    FirstName = cols[1].Trim(),
                    LastName = cols[2].Trim(),
                    Email = email,
                    Phone = cols.Length > 4 ? cols[4].Trim() : null,
                    Gender = Enum.TryParse<Gender>(cols.Length > 5 ? cols[5].Trim() : "Male", out var g) ? g : Gender.Male,
                    DateOfBirth = DateTime.Parse(cols[6].Trim()),
                    JoinDate = DateTime.Parse(cols[7].Trim()),
                    JobTitle = cols[8].Trim(),
                    DepartmentId = int.Parse(cols[9].Trim()),
                    BaseSalary = decimal.Parse(cols[10].Trim()),
                    Address = cols.Length > 11 ? cols[11].Trim() : null,
                    City = cols.Length > 12 ? cols[12].Trim() : null,
                    PostalCode = cols.Length > 13 ? cols[13].Trim() : null,
                    EmergencyContact = cols.Length > 14 ? cols[14].Trim() : null,
                    // Problem 19: encrypt NationalId, store hash separately for dedup
                    NationalId = nationalId != null ? _encryption.Encrypt(nationalId) : null,
                    NationalIdHash = nationalId != null ? _encryption.Hash(nationalId) : null,
                    ReportsToId = cols.Length > 16 && int.TryParse(cols[16].Trim(), out var rid) ? rid : null,
                    CreatedBy = User.FindFirstValue(ClaimTypes.Email)
                };

                newEmployees.Add(emp);
                result.Status = "Queued";
            }
            catch (Exception ex)
            {
                result.Status = "Failed";
                result.Error = ex.Message;
            }

            results.Add(result);
        }

        if (newEmployees.Count > 0)
        {
            _context.Employees.AddRange(newEmployees);
            await _context.SaveChangesAsync();

            foreach (var (emp, row) in newEmployees.Zip(results.Where(r => r.Status == "Queued")))
            {
                row.Status = "Success";
                await _audit.LogAsync("Employee", emp.Id, "Create", null,
                    new { emp.EmployeeCode, emp.Email },
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    User.FindFirstValue("fullName") ?? "Unknown",
                    HttpContext.Connection.RemoteIpAddress?.ToString(), emp.Id);
            }
        }

        return Ok(new
        {
            Total = results.Count,
            Imported = results.Count(r => r.Status == "Success"),
            Failed = results.Count(r => r.Status == "Failed"),
            Rows = results
        });
    }

    private static Employee MapDtoToEmployee(CreateEmployeeDto dto, IEncryptionService encryption)
    {
        var rawNationalId = string.IsNullOrWhiteSpace(dto.NationalId) ? null : dto.NationalId.Trim();
        return new Employee
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
            // Problem 19: store encrypted value + hash separately
            NationalId = rawNationalId != null ? encryption.Encrypt(rawNationalId) : null,
            NationalIdHash = rawNationalId != null ? encryption.Hash(rawNationalId) : null,
            ReportsToId = dto.ReportsToId,
        };
    }

    // Handles quoted fields with commas inside
    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var c in line)
        {
            if (c == '"') { inQuotes = !inQuotes; }
            else if (c == ',' && !inQuotes) { fields.Add(current.ToString()); current.Clear(); }
            else { current.Append(c); }
        }
        fields.Add(current.ToString());
        return fields.ToArray();
    }
}
