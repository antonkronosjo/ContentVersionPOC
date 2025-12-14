namespace ContentVersionsPOC.Data.Models
{
    public class NewsContent : ContentVersion
    {
        public required string Heading { get; set; }
        public string? Lead { get; set; }
        public required string Text { get; set; }
    }
}
