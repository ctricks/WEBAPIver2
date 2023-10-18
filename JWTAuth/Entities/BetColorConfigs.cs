using System.ComponentModel.DataAnnotations.Schema;

namespace WEBAPI.Entities
{
    public class BetColorConfigs
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        
        [Column("create_ts")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
