namespace ST10026525.PROG7312.MunicipalPOE.Models
{
    public class Report
    {
        public int Id { get; set; } 
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string MediaFileName { get; set; } // Optional image file
        public DateTime SubmittedAt { get; set; } = DateTime.Now; // Timestamp for queue order
    }
}
