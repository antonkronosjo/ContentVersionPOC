namespace ContentVersionsPOC.Data.Models;

public abstract class ContentVersion
{
    protected ContentVersion()
    {
        Created = DateTime.UtcNow;
    }

    public required Guid VersionId { get; set; }
    public Guid ContentId { get; set; }
    public Content Content { get; set; }
    public DateTime Created { get; set; }
    public DateTime? StartPublish { get; set; }
    public DateTime? StopPublish { get; set; }
}