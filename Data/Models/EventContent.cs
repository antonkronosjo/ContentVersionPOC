using ContentVersionsPOC.Data.Enums;

namespace ContentVersionsPOC.Data.Models
{
    public class EventContent : Content
    {
        public EventContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }
        
        public required string Heading { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
