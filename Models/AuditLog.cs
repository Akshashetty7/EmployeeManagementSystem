using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string EntityType { get; set; } = string.Empty;

        public int EntityId { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty;

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        [Required]
        public string PerformedBy { get; set; } = string.Empty;

        public string PerformedByName { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? IPAddress { get; set; }

        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
