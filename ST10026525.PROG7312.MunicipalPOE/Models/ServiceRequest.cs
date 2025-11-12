namespace ST10026525.PROG7312.MunicipalPOE.Models
{

    public class ServiceRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; 
        public DateTime DateSubmitted { get; set; } = DateTime.Now;
        public int Priority { get; set; } = 1; 
        public string Location { get; set; } = string.Empty;
        public List<Guid> RelatedRequestIds { get; set; } = new(); 
    }
}
