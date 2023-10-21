namespace WEBAPI.Models.Users
{
    public class TokenInfoResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? ExpireDate { get; set; }
        public int? UserId { get; set; }
        public string? UserRole { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }

        public bool? isExpired { get; set; }

    }
}
