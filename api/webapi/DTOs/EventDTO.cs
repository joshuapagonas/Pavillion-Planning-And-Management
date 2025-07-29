namespace PPM.Models.DTOs
{
    public class EventDTO
    {
        public int event_id { get; set; }
        public int user_id { get; set; }
        public int park_id { get; set; }
        public int registration_id { get; set; }
        public string event_name { get; set; } = string.Empty;
        public string event_description { get; set; } = string.Empty;
        public DateTime event_start_date { get; set; }
        public DateTime event_end_date { get; set; }
        public DateTime event_start_time { get; set; }
        public DateTime event_end_time { get; set; }
        public UserDTO? User { get; set; }
        public ParkDTO? Park { get; set; }
    }
}
