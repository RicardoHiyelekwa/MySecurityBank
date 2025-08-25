using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models;

namespace MySecurityBank.Services
{
    public class AuditingService : IAuditingService
    {
        private readonly ApplicationDbContext _db;

        public AuditingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(string action, int? entityId = null)
        {
            _db.AuditLogs.Add(new Models.MySecureBank.Models.AuditLog { Timestamp = DateTime.UtcNow, Action = action, EntityId = entityId });
            await _db.SaveChangesAsync();
        }
    }
}