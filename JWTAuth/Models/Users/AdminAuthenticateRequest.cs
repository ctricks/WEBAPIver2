using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.Users
{
    public class AdminAuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
