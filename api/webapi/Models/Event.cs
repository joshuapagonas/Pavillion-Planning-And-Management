﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPM.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int event_id { get; set; }

        [StringLength(255)]
        public string event_name { get; set; } = string.Empty;

        [StringLength(255)]
        public string event_description { get; set; } = string.Empty;
        public DateTime event_start_date { get; set; }
        public DateTime event_end_date { get; set; }
        public DateTime event_start_time { get; set; }
        public DateTime event_end_time { get; set; }
    }
}
