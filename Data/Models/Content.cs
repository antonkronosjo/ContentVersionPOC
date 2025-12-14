namespace ContentVersionsPOC.Data.Models;

public class Content
{
    public Content()
    {
        Created = DateTime.UtcNow;
    }

    public required Guid Id { get; set; }
    public DateTime Created { get; set; }
    public Guid? ActiveVersionId { get; set; }
    public ContentVersion? ActiveVersion { get; set; }
    public ICollection<ContentVersion> Versions { get; set; } = new List<ContentVersion>();

    public void AddVersion(ContentVersion version)
    {
        version.ContentId = this.Id;
        version.Content = this;
        Versions.Add(version);
        ActiveVersionId = version.VersionId;
        ActiveVersion = version;
    }
}