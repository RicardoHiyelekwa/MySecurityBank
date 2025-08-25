using Microsoft.AspNetCore.Http;

namespace MySecurityBank.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(IFormFile file);
    }

    public interface ISwiftValidationService
    {
        Task<bool> IsValidAsync(string swiftCode);
    }

    public interface IAuditingService
    {
        Task LogAsync(string action, int? entityId = null);
    }
}