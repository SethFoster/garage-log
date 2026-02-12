namespace GarageLog.Models
{
    public class ModItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Link { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }
        // Optional relation to a Car
        public int? CarId { get; set; }
        public Car? Car { get; set; }

        // Optional file references (store paths or blob ids)
        public string? PhotoPath { get; set; }
        public string? ReceiptPath { get; set; }
    }
}
