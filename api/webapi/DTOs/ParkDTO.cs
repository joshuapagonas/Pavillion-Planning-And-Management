namespace PPM.Models.DTOs
{
    public class ParkDTO
    {
        public int park_id { get; set; }
        public required string park_name { get; set; }
        public string? geolocation { get; set; }
        public string street_address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public int number_of_pavillions { get; set; }
        public int acres { get; set; }
        public int play_structures { get; set; }
        public int trails { get; set; }
        public int baseball_fields { get; set; }
        public int disc_golf_courses { get; set; }
        public int volleyball_courts { get; set; }
        public int fishing_spots { get; set; }
        public int soccer_fields { get; set; }
    }
}
