using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class UserWallet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }       
        public double? available_balance { get; set; }
        public double? total_balance { get; set; }

        public int? UserId { get; set; }
        public string UserName { get; set; }
        
        public virtual ICollection<WalletTxn> WalletTrans { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("update_ts")]
        public DateTime UpdateDate { get; set; }
    }
}
