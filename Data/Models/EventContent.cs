namespace ContentVersionsPOC.Data.Models
{
    public class EventContent : ContentVersion
    {
        public required string Heading { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
