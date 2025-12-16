using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Extensions;
using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContentVersionsPOC.Data.Services
{
    public interface IContentRepository
    {
        ContentWrapper<T> Get<T>(Guid contentId) where T : ContentVersion;
        ContentWrapper<T> Create<T>(T initialVersion) where T : ContentVersion;
        ContentWrapper<T> Update<T>(Guid contentId, T contentVersion) where T : ContentVersion;
        ContentWrapper<T> Update<T>(Guid contentId, Dictionary<string, string?> updates) where T : ContentVersion;
        void Delete(Guid contentId);
        IQueryable<T> QueryActiveVersion<T>(LanguageBranch languageBranch) where T : ContentVersion;
    }

    public class ContentRepository : IContentRepository
    {
        ContentVersionsPOCContext _context;

        public ContentRepository(ContentVersionsPOCContext context)
        {
            _context = context;
        }

        public ContentWrapper<T> Get<T>(Guid contentId) where T : ContentVersion 
        {
            var content = _context.Content
                .Include(x => x.Versions)
                .Single(x => x.Id == contentId);

            return new ContentWrapper<T>(content);
        }

        public ContentWrapper<T> Create<T>(T initialVersion) where T : ContentVersion
        {
            using var transaction = _context.Database.BeginTransaction();

            var content = new Content() { Id = initialVersion.ContentId };

            _context.Add(content);
            _context.SaveChanges();

            content.AddVersion(initialVersion);
            _context.Add(initialVersion);

            _context.Update(content); ;
            _context.SaveChanges();

            CommitTransaction(transaction);

            return new ContentWrapper<T>(content);
        }

        public void Delete(Guid contentId)
        {
            var content = _context.Content.Single(x => x.Id == contentId);
            _context.Remove(content);
            _context.SaveChanges();
        }

        public ContentWrapper<T> Update<T>(Guid contentId, T updatedVersion) where T : ContentVersion
        {
            var content = _context.Content
                .Where(x => x.Id == contentId)
                .Include(x => x.Versions)
                .Single();
            
            content.AddVersion(updatedVersion);

            _context.Add(updatedVersion);
            _context.Update(content);

            _context.SaveChanges();

            return new ContentWrapper<T>(content);
        }

        public ContentWrapper<T> Update<T>(Guid contentId, Dictionary<string, string?> updates) where T : ContentVersion
        {
            var test = Get<T>(contentId);
            var activeVersion = Get<T>(contentId).ActiveVersion;
            if (activeVersion == null)
                throw new Exception("Havent considered what should happen here yet...");

            var updatedVersion = activeVersion.ApplyUpdates<T>(updates);
            updatedVersion.VersionId = Guid.NewGuid();

            return Update<T>(contentId, updatedVersion);
        }


        public IQueryable<T> QueryActiveVersion<T>(LanguageBranch languageBranch) where T : ContentVersion
        {
            return _context.ContentVersions
                .OfType<T>()
                .Where(x => x.LanguageBranch == languageBranch)
                .Include(x => x.Content)
                .Where(x => x.Content.ActiveVersionId == x.VersionId);
        }

        private void CommitTransaction(IDbContextTransaction transaction)
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
