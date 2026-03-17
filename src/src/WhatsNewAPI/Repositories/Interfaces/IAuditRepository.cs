using WhatsNewAPI.Models.Entities;

namespace WhatsNewAPI.Repositories.Interfaces;

public interface IAuditRepository
{
    Task LogAsync(AuditLog auditLog);
}
