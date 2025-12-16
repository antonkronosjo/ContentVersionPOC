using ContentVersionsPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace ContentVersionsPOC.Data.Models;

public abstract class ContentVersion
{
    protected ContentVersion()
    {
        Created = DateTime.UtcNow;
    }
    public required Guid VersionId { get; set; }
    public Guid ContentId { get; set; }
    public required LanguageBranchEnum LanguageBranch { get; set; }
    public virtual LanguageBranch LanguageMapping { get; set; }

    //[JsonIgnore]
    //public ContentRoot Content { get; set; }

    public DateTime Created { get; set; }
    public DateTime? StartPublish { get; set; }
    public DateTime? StopPublish { get; set; }
}