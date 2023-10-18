using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class FightMatch
    {
        public int Id { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchNumber { get; set; }

        public int MatchStatusId { get; set; } = -1;
        public int MatchResultId { get; set; } = -1;

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
