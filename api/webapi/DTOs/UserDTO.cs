﻿namespace PPM.Models.DTOs
{
    public class UserDTO
    {
        public int user_id {  get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
        public bool is_admin { get; set; }
    }
}
