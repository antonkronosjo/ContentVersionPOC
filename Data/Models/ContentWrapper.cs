namespace ContentVersionsPOC.Data.Models;

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
