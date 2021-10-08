using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Accounts
{
    public class AuthenticateRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}