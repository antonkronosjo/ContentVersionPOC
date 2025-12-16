using ContentVersionsPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace ContentVersionsPOC.Data.Models;

public abstract class Content
{
    protected Content(Guid versionId, Language language)
    {
        VersionId = versionId;
        Created = DateTime.UtcNow;
    }
    public Guid VersionId { get; set; }
    public Guid ContentId { get; set; }
    public Language Language { get; set; }
    public virtual LanguageBranch LanguageBranch { get; set; }
    public DateTime Created { get; set; }
}