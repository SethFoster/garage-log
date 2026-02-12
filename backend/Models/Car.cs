namespace GarageLog.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Nickname { get; set; }

        // e.g., 1998, 2002
        public string Year { get; set; }
    }
}
