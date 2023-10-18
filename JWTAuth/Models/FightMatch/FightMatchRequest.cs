namespace WEBAPI.Models.FightMatch
{
    public class FightMatchRequest
    {
        public DateTime FightDate { get; set; }
        public int MatchNumber { get; set; }
        public int? MatchStatusId { get; set; }
        public int? MatchResultId { get; set; }
    }
}
