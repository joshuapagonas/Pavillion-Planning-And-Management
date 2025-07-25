﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPM.Models
{
    [Table("Users")]
    public class User //: IdentityUser
    {
        [Key]
        public int user_id { get; set; }

        [StringLength(255)]
        public string first_name { get; set; } = string.Empty;

        [StringLength(255)]
        public string last_name { get; set; } = string.Empty;

        [StringLength(255)]
        public string username { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string password_hash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(50)]
        public string phone_number { get; set; } = string.Empty;
        public bool is_admin { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    }
}