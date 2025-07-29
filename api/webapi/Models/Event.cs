using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPM.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int event_id { get; set; }

        [ForeignKey("User)")]
        public int user_id { get; set; }

        [ForeignKey("Park")]
        public int park_id { get; set; }

        [StringLength(255)]
        [ForeignKey("Registration")]
        public int registration_id { get; set; }
        [StringLength(255)]
        public string event_name { get; set; } = string.Empty;
        [StringLength(4000)]
        public string event_description { get; set; } = string.Empty;
        public DateTime event_start_date { get; set; }
        public DateTime event_end_date { get; set; }
        public DateTime event_start_time { get; set; }
        public DateTime event_end_time { get; set; }
        public bool is_public_event { get; set; }
        public int num_of_attendees { get; set; }
        public virtual required User? User { get; set; }
        public virtual required Park? Park { get; set; }
    }
}