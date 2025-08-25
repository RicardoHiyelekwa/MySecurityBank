using System.ComponentModel.DataAnnotations;

namespace MySecurityBank.Models
{
    namespace MySecureBank.Models
    {
        public class Customer
        {
            public int Id { get; set; }
            [Required, StringLength(120)]
            public string FullName { get; set; } = string.Empty;
            [Required, StringLength(30)]
            public string IdNumber { get; set; } = string.Empty;
            [Required, StringLength(30)]
            public string AccountNumber { get; set; } = string.Empty;
            [Required]
            public string PasswordHash { get; set; } = string.Empty;
        }

        public class RegisterViewModel
        {
            [Required, StringLength(120)]
            public string FullName { get; set; } = string.Empty;
            [Required, StringLength(30)]
            public string IdNumber { get; set; } = string.Empty;
            [Required, StringLength(30)]
            public string AccountNumber { get; set; } = string.Empty;
            [Required, MinLength(8), DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [Required, Compare(nameof(Password)), DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class LoginViewModel
        {
            [Required, StringLength(30)]
            public string AccountNumber { get; set; } = string.Empty;
            [Required, DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public class PaymentViewModel
        {
            [Required, Range(1, double.MaxValue)]
            public decimal Amount { get; set; }
            [Required]
            public string Currency { get; set; } = string.Empty;
            [Required]
            public string Provider { get; set; } = string.Empty;
            [Required]
            public string RecipientAccount { get; set; } = string.Empty;
            [Required]
            public string SwiftCode { get; set; } = string.Empty;
            public IFormFile? Attachment { get; set; }
        }

        public class Transaction
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Provider { get; set; } = string.Empty;
            public string RecipientAccount { get; set; } = string.Empty;
            public string SwiftCode { get; set; } = string.Empty;
            public string? AttachmentUrl { get; set; }
            public string Status { get; set; } = "Pending";
        }

        public class AuditLog
        {
            public int Id { get; set; }
            public DateTime Timestamp { get; set; }
            public string Action { get; set; } = string.Empty;
            public int? EntityId { get; set; }
        }
    }
}