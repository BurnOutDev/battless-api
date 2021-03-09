using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Accounts
{
    public class AuthenticateRequest
    {
        public string Username
        {
            set
            {
                Email = value;
            }
        }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}