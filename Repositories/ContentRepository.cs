using ContentVersionsPOC.Data;
using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Extensions;
using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentVersionsPOC.Repositories;

public interface IContentRepository
{
    T? Get<T>(Guid contentId, Language language) where T : Content;
    T Create<T>(T initialVersion, Language language) where T : Content;
    T Update<T>(Guid contentId, Language language, T contentVersion) where T : Content;
    T Update<T>(Guid contentId, Language language, Dictionary<string, string?> updates) where T : Content;
    void Delete(Guid contentId);
    IQueryable<T> QueryActiveVersions<T>(Language languageBranch) where T : Content;
}

public class ContentRepository : IContentRepository
{
    ContentVersionsPOCContext _context;

    public ContentRepository(ContentVersionsPOCContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Returns currently active version of content for language. Returns null if entity not found
    /// </summary>
    public T? Get<T>(Guid contentId, Language language) where T : Content 
    {
        return QueryActiveVersions<T>(language).FirstOrDefault(x => x.ContentId == contentId);
    }

    /// <summary>
    /// Creates new content with underlying root object, language branch and version handeling
    /// </summary>
    public T Create<T>(T initialVersion, Language language) where T : Content
    {
        var contentId = Guid.NewGuid();
        initialVersion.ContentId = contentId;
        var contentRoot = new ContentRoot(contentId);
        contentRoot.AddVersion(initialVersion, language);
        _context.ContentRoots.Add(contentRoot);
        _context.SaveChanges();
        return initialVersion;
    }

    /// <summary>
    /// Delets content and all versions of it
    /// </summary>
    public void Delete(Guid contentId)
    {
        var content = _context.Content.Single(x => x.ContentId == contentId);
        _context.Remove(content);
        _context.SaveChanges();
    }

    /// <summary>
    /// Updates content with new version
    /// </summary>
    public T Update<T>(Guid contentId, Language language, T updatedVersion) where T : Content
    {
        var root = _context.ContentRoots
            .Include(r => r.LanguageBranches)
                .ThenInclude(m => m.Versions)
            .FirstOrDefault(r => r.ContentId == contentId)
            ?? throw new KeyNotFoundException($"ContentRoot with ID {contentId} not found.");

        root.AddVersion(updatedVersion, language);
        
        _context.SaveChanges();
        return updatedVersion; ;
    }

    /// <summary>
    /// Updates content with new version based on key/values 
    /// </summary>
    public T Update<T>(Guid contentId, Language language, Dictionary<string, string?> updates) where T : Content
    {
        var activeVersion = Get<T>(contentId, language);
        if (activeVersion == null)
            throw new InvalidOperationException($"Content with id {contentId} for language {language} does not exist");

        var updatedVersion = activeVersion.ApplyUpdates(updates);
        updatedVersion.VersionId = Guid.NewGuid();

        return Update(contentId, language, updatedVersion);
    }

    /// <summary>
    /// Returns a base query used when querying active versions of content
    /// </summary>
    public IQueryable<T> QueryActiveVersions<T>(Language language) where T : Content
    {
        //return _context.LanguageBranches
        //    .Where(x => x.Language == language)
        //    .Include(x => x.ActiveVersion)
        //    .OfType<T>();
        return _context.Content.OfType<T>()
            .Include(x => x.LanguageBranch);
            //.Where(x => x.LanguageBranch.ActiveVersionId == x.VersionId);
       // return _context.Content.OfType<T>().Where(v => v.LanguageBranch.ActiveVersionId == v.VersionId);

    }
}
