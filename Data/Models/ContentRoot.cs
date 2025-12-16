namespace ContentVersionsPOC.Data.Models;

public class ContentRoot
{
    public ContentRoot()
    {
        Created = DateTime.UtcNow;
    }

    public required Guid Id { get; set; }
    public DateTime Created { get; set; }

    // Each language entry acts as a folder for that language's versions
    public virtual ICollection<LanguageBranch> LanguageMappings { get; set; } = new List<LanguageBranch>();

    //public void AddVersion(ContentVersion version)
    //{
    //    version.ContentId = this.Id;
    //    version.Content = this;
    //    Versions.Add(version);
    //    //ActiveVersionId = version.VersionId;
    //    //ActiveVersion = version;
    //}
}
public class ContentWrapper<T> where T : ContentVersion
{
    private readonly ContentRoot _content;

    public ContentWrapper(ContentRoot content)
    {
        _content = content;
    }

    public Guid Id => _content.Id;
    public DateTime Created => _content.Created;
}