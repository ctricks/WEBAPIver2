namespace WEBAPI.Models.Users
{
    public class AdminAuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpireToken { get; set; }
    }
}
