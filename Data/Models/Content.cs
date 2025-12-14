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
public class ContentWrapper<T> where T : ContentVersion
{
    private readonly Content _content;

    public ContentWrapper(Content content)
    {
        _content = content;
    }

    public Guid Id => _content.Id;

    public IEnumerable<T> Versions => _content.Versions.OfType<T>();
}