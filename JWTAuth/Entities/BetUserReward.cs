using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class BetUserReward
    {
        public int Id { get; set; }
        public double RewardAmount { get; set; }

        //CB-10022023 Reference for User Bet
        public virtual ICollection<UserBetTxn> UBetTxn { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
