using ContentVersionsPOC.Attributes;
using ContentVersionsPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace ContentVersionsPOC.Data.Models
{
    [JsonDerivedType(typeof(NewsContent), nameof(NewsContent))]
    [ContentType(ContentType.News)]
    public class NewsContent : Content
    {
        public NewsContent(Guid versionId, Language language) : base(versionId, language)
        {
            
        }

        [ContentPropertyMetaData(editable: true)]
        public required string Heading { get; set; }

        [ContentPropertyMetaData(editable: true)]
        public string? Lead { get; set; }

        [ContentPropertyMetaData(editable: true)]
        public required string Text { get; set; }
    }
}
