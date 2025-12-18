using ContentVersionsPOC.Attributes;
using ContentVersionsPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace ContentVersionsPOC.Data.Models
{
    [JsonDerivedType(typeof(EventContent), nameof(EventContent))]
    [ContentType(ContentType.Event)]
    public class EventContent : Content
    {
        public EventContent()
        {
            
        }

        public EventContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }
        
        public required string Heading { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}
