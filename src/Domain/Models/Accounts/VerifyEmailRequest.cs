using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}