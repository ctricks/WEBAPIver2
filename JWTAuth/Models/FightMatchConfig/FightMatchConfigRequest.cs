namespace WEBAPI.Models.FightMatchConfig
{
    public class FightMatchConfigRequest
    {
        public DateTime FightMatchDate { get; set; }
        public int MatchNumber { get; set; }
        public int? MatchStatusId { get; set; }

        public int? MatchResultId { get; set; } = 0;
    }
}
