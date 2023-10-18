using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.Users
{
    public class RegisterRequest
    {        
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
