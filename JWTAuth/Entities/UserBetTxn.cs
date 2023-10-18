using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class UserBetTxn
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FightMatchId { get; set; }
        public int BetColorId { get; set; }
        public double BetAmount { get; set; }   
        public DateTime BetDate { get; set; }

        //For reference UserWallet Table
        public virtual ICollection<UserWallet> UWallet { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
