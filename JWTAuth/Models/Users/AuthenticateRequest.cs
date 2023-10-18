using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.Users
{
    public class AuthenticateRequest
    {
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
