using ContentVersionsPOC.Data.Enums;
using System.Text.Json.Serialization;

namespace ContentVersionsPOC.Data.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "contentType")]
[JsonDerivedType(typeof(NewsContent), nameof(NewsContent))]
[JsonDerivedType(typeof(EventContent), nameof(EventContent))]
public abstract class Content
{
    public Content()
    {
            
    }
    protected Content(Guid versionId, Language language)
    {
        VersionId = versionId;
        Created = DateTime.UtcNow;
    }
    public Guid VersionId { get; set; }
    public Guid ContentId { get; set; }
    public ContentRoot ContentRoot { get; set; }
    public ContentType ContentType { get; private set; }
    public Language Language { get; set; }

    [JsonIgnore]
    public virtual LanguageBranch LanguageBranch { get; set; }
    public DateTime Created { get; set; }
}