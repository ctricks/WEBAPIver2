using System.ComponentModel.DataAnnotations;

namespace WEBAPI.Models.WalletTransactions
{
    public class RegisterRequest
    {
        [Required]
        public string UserTokenID { get; set; }
        
        [Required]
        public string TransactionType { get; set; }

        [Required]        
        public double available_balance { get; set; }

        [Required]
        public double total_balance { get; set; }

    }
}
