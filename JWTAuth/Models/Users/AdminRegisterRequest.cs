using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.Users
{
    public class AdminRegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
