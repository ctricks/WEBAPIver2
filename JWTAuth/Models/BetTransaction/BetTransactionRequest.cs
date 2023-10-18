namespace WEBAPI.Models.BetTransaction
{
    public class BetTransactionRequest
    {
        public int UserId { get; set; }
        public int FightMatchNumber { get; set; }
        public string BetColorName { get; set; }
        public DateTime FightDate { get; set; }        
        public double BetAmount { get; set; }
    }
}
