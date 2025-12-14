using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContentVersionsPOC.Data
{
    public interface IContentRepository
    {
        ContentWrapper<T> Create<T>(T contentVersion) where T : ContentVersion;
        void Delete(Guid contentId);
        void Update<T>(T contentVersion) where T : ContentVersion;
        IQueryable<T> QueryActiveVersion<T>(LanguageBranch languageBranch) where T : ContentVersion;
    }

    public class ContentRepository : IContentRepository
    {
        ContentVersionsPOCContext _context;

        public ContentRepository(ContentVersionsPOCContext context)
        {
            _context = context;
        }

        public ContentWrapper<T> Create<T>(T contentVersion) where T : ContentVersion
        {
            using var transaction = _context.Database.BeginTransaction();

            var content = new Content() { Id = contentVersion.ContentId };

            _context.Add(content);
            _context.SaveChanges();

            content.AddVersion(contentVersion);
            _context.Add(contentVersion);

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

        public void Update<T>(T updatedVersion) where T : ContentVersion
        {
            var content = _context.Content
                .Where(x => x.Id == updatedVersion.ContentId)
                .Include(x => x.Versions)
                .Single();

            content.AddVersion(updatedVersion);
            _context.Add(updatedVersion);
            _context.Update(content);

            _context.SaveChanges();
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
