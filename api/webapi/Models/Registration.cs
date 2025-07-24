﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPM.Models
{
    [Table("Registration")]
    public class Registration
    {
        [Key]
        public int registration_id { get; set; }

        [ForeignKey("User")]
        public int user_id { get; set; }

        [ForeignKey("Park")]
        public int park_id { get; set; }
        [StringLength(50)]
        public string? requested_park {  get; set; }
        public int pavillion { get; set; }
        public DateTime registration_date { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public bool is_approved { get; set; }
        public virtual User? User { get; set; }
        public virtual Park? Park { get; set; }
    }
}