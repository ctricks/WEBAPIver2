namespace WEBAPI.Models.BetTransaction
{
    public class BetTransactionByColorsResponse
    {
        public string ColorName { get; set; }
        public int FightNumber { get; set; }
        public DateTime FightDate { get; set; }
        public double TotalAmount { get; set; }
    }
}
