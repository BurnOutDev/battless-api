using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}