namespace WhatsNewAPI.Services.Interfaces;

public interface IAuditService
{
    Task LogActionAsync(
        int? userId,
        string action,
        string entityType,
        int? entityId = null,
        string? oldValue = null,
        string? newValue = null,
        string? ipAddress = null);
}
