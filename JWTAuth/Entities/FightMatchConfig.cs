using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class FightMatchConfig
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchCurrentNumber { get; set; }
        public int MatchTotalNumber { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
