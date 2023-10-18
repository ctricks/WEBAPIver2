using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class MatchStatusConfig
    {
        public int Id { get; set; }
        public string Status { get; set; }

        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
