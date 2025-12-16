using ContentVersionsPOC.Data.Models;

namespace ContentVersionsPOC.Data.Summaries
{
    public class ContentSummary
    {
        public Guid ContentId { get; set; }
        public Guid VersionId { get; set; }
        public ContentSummary(Content content)
        {
            ContentId = content.ContentId;
            VersionId = content.VersionId;
        }
    }
}
