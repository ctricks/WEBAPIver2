
namespace WEBAPI.Models.WalletTransactions
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Message { get; set; }        
        public double? TotalBalance { get; set; }
        public double? AvailableBalance { get; set; }
    }
}
