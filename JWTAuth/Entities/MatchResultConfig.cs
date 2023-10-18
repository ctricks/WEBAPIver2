using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class MatchResultConfig
    {
        public int Id { get; set; }
        public string Result { get; set; }

        //CB-10022023 Reference to FightMatch Table Many to One
        public virtual ICollection<FightMatch> FightMatches { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
