namespace MySecurityBank.Models
{
    using System.ComponentModel.DataAnnotations;

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
    }

}
