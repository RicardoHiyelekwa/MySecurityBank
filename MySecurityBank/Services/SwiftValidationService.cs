using System.Text.RegularExpressions;

namespace MySecurityBank.Services
{
    public class SwiftValidationService : ISwiftValidationService
    {
        public async Task<bool> IsValidAsync(string swiftCode)
        {
            var regex = new Regex(@"^[A-Z]{4}[A-Z]{2}[A-Z0-9]{2}([A-Z0-9]{3})?$");
            return regex.IsMatch(swiftCode);
        }
    }
}