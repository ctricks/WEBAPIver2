using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.WalletTransactions
{
    public class AuthenticateRequest
    {
        [Required]
        public string TokenID { get; set; }
        
    }
}
