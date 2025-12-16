using ContentVersionsPOC.Data.Enums;
using ContentVersionsPOC.Data.Extensions;
using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContentVersionsPOC.Data.Services
{
    public interface IContentRepository
    {
        T Get<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch) where T : ContentVersion;
        T Create<T>(T initialVersion, Enums.LanguageBranchEnum languageBranch) where T : ContentVersion;
        T Update<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch, T contentVersion) where T : ContentVersion;
        T Update<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch, Dictionary<string, string?> updates) where T : ContentVersion;
        void Delete(Guid contentId);
        IQueryable<T> QueryActiveVersion<T>(Enums.LanguageBranchEnum languageBranch) where T : ContentVersion;
    }

    public class ContentRepository : IContentRepository
    {
        ContentVersionsPOCContext _context;

        public ContentRepository(ContentVersionsPOCContext context)
        {
            _context = context;
        }

        public T Get<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch) where T : ContentVersion 
        {
            return QueryActiveVersion<T>(languageBranch).Single(x => x.ContentId == contentId);
        }

        public T Create<T>(T initialVersion, Enums.LanguageBranchEnum languageBranch) where T : ContentVersion
        {
            using var transaction = _context.Database.BeginTransaction();
            // 1. Create the Identity (Root)
            var root = new ContentRoot
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow
            };

            // 3. Create the LanguageMapping to link Root + Language to this Version
            var mapping = new Models.LanguageBranch
            {
                LanguageBranchEnum = languageBranch,
                ActiveVersion = initialVersion // This version is now the "Active" one
            };

            // 4. Link everything to the Root
            // Because of the hierarchical setup, we add the mapping to the root, 
            // and the version to the mapping.
            mapping.Versions.Add(initialVersion);
            root.LanguageMappings.Add(mapping);

            // 5. Save to Database
            // Adding the 'root' will add the entire graph (Mapping + Version)
            _context.Content.Add(root);

            CommitTransaction(transaction);

            return initialVersion;
        }

        public void Delete(Guid contentId)
        {
            var content = _context.Content.Single(x => x.Id == contentId);
            _context.Remove(content);
            _context.SaveChanges();
        }

        public T Update<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch, T updatedVersion) where T : ContentVersion
        {
            // 1. Fetch the Root with its specific LanguageMapping and its current versions
            var root = _context.Content
                .Include(r => r.LanguageMappings)
                    .ThenInclude(m => m.Versions)
                .FirstOrDefault(r => r.Id == contentId)
                ?? throw new KeyNotFoundException($"ContentRoot with ID {contentId} not found.");

            // 2. Find the mapping for the specific language
            var mapping = root.LanguageMappings
                .FirstOrDefault(m => m.LanguageBranchEnum == languageBranch)
                ?? throw new KeyNotFoundException($"Language mapping for '{languageBranch}' not found.");

            // 5. Update the pointer to make this version "Active"
            mapping.Versions.Add(updatedVersion);
            mapping.ActiveVersion = updatedVersion;

            // 6. Persist changes
            // EF Core tracks the 'root' object graph and will insert the new version 
            // and update the LanguageMapping table in a single transaction.
            _context.SaveChanges();

            return updatedVersion; ;
        }

        public T Update<T>(Guid contentId, Enums.LanguageBranchEnum languageBranch, Dictionary<string, string?> updates) where T : ContentVersion
        {
            var activeVersion = Get<T>(contentId, languageBranch);
            if (activeVersion == null)
                throw new Exception("Havent considered what should happen here yet...");

            var updatedVersion = activeVersion.ApplyUpdates<T>(updates);
            updatedVersion.VersionId = Guid.NewGuid();

            return Update<T>(contentId, languageBranch, updatedVersion);
        }


        public IQueryable<T> QueryActiveVersion<T>(Enums.LanguageBranchEnum languageBranch) where T : ContentVersion
        {
            return _context.LanguageMappings
                .Where(x => x.LanguageBranchEnum == languageBranch)
                .Include(x => x.ActiveVersion)
                .OfType<T>();
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
