using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System.Text.Json;

namespace EmployeeManagementSystem.Services
{
    public interface IAuditService
    {
        Task LogAsync(string entityType, int entityId, string action, object? oldValues, object? newValues, string userId, string userName, string? ipAddress = null, int? employeeId = null);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context) => _context = context;

        public async Task LogAsync(string entityType, int entityId, string action,
            object? oldValues, object? newValues, string userId, string userName,
            string? ipAddress = null, int? employeeId = null)
        {
            var log = new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                PerformedBy = userId,
                PerformedByName = userName,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
