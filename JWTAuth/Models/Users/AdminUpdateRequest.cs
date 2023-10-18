namespace WEBAPI.Models.Users
{
    public class AdminUpdateRequest
    {
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
