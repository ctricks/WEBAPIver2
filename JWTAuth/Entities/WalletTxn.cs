using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class WalletTxn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TransactionType { get; set; } 
        public double amount { get; set; }
        public double account_bal { get; set; }

        public int UserIDRef { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
