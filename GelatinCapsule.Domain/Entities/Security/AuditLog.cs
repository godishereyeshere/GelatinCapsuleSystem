namespace GelatinCapsule.Domain.Entities.Security;

public class AuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = null!;      // Login, Create, Edit, Delete
    public string? EntityName { get; set; }          // User, Role, Form
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }           // JSON
    public string? NewValues { get; set; }           // JSON
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public int? DurationMs { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? ErrorMessage { get; set; }
}