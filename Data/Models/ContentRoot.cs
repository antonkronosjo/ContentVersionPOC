using ContentVersionsPOC.Data.Enums;

namespace ContentVersionsPOC.Data.Models;

public class ContentRoot
{
    public ContentRoot()
    {
        
    }
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

    public LanguageBranch AddVersion<T>(T content, Language language) where T : Content
    {
        var languageBranch = GetExistingOrAddBranch(language);
        languageBranch.AddVersion(content);
        return languageBranch;
    }

    public LanguageBranch GetExistingOrAddBranch(Language language) {
        var languageBranch = LanguageBranches.SingleOrDefault(x => x.Language == language);
        if (languageBranch != null)
            return languageBranch;

        languageBranch = new LanguageBranch(this.ContentId, language);
        LanguageBranches.Add(languageBranch);
        return languageBranch;
    }

    public LanguageBranch CreateLanguageBranch(Language language) {
        var languageBranch = new LanguageBranch(this.ContentId, language);
        LanguageBranches.Add(languageBranch);
        return languageBranch;
    }
}