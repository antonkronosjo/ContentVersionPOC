using ContentVersionsPOC.Data.Enums;

namespace ContentVersionsPOC.Data.Models;

public class ContentRoot
{
    public ContentRoot(Guid contentId)
    {
        ContentId = contentId;
        Created = DateTime.UtcNow;
    }

    public Guid ContentId { get; set; }
    public DateTime Created { get; set; }
    public DateTime StartPublish { get; set; }
    public DateTime StopPublish { get; set; }
    public ICollection<LanguageBranch> LanguageBranches { get; set; } = new List<LanguageBranch>();

    public void AddVersion<T>(T content, Language language) where T : Content
    {
        var languageBranch = GetExistingOrCreateNewLanguageBranch(language);
        languageBranch.AddVersion(content);
    }

    private LanguageBranch GetExistingOrCreateNewLanguageBranch(Language language) {
        var languageBranch = LanguageBranches.SingleOrDefault(x => x.Language == language);
        if (languageBranch != null)
            return languageBranch;

        languageBranch = new LanguageBranch(this.ContentId, language);
        LanguageBranches.Add(languageBranch);
        return languageBranch;
    }
}