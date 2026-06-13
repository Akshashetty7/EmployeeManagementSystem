using System.Text.Json;
using EmsApi.Data;
using EmsApi.Models;

namespace EmsApi.Services;

public interface IAuditService
{
    Task LogAsync(string entityType, int entityId, string action,
        object? oldValues, object? newValues,
        string userId, string userName, string? ipAddress = null, int? employeeId = null);
}

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context) => _context = context;

    public async Task LogAsync(string entityType, int entityId, string action,
        object? oldValues, object? newValues,
        string userId, string userName, string? ipAddress = null, int? employeeId = null)
    {
        _context.AuditLogs.Add(new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues),
            PerformedBy = userId,
            PerformedByName = userName,
            IPAddress = ipAddress,
            EmployeeId = employeeId,
            Timestamp = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }
}
