using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentVersionsPOC.Data
{
    public class ContentVersionsPOCContext : DbContext
    {
        public ContentVersionsPOCContext(DbContextOptions<ContentVersionsPOCContext> options) : base(options)
        {
            
        }

        public DbSet<ContentRoot> Content => Set<ContentRoot>();
        public DbSet<ContentVersion> ContentVersions => Set<ContentVersion>();
        public DbSet<LanguageBranch> LanguageMappings => Set<LanguageBranch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentVersion>().ToTable("ContentVersions");
            modelBuilder.Entity<NewsContent>().ToTable("News");
            modelBuilder.Entity<EventContent>().ToTable("Events");

            // 1. Composite Key for Mapping
            modelBuilder.Entity<LanguageBranch>()
                .HasKey(m => new { m.ContentId, m.LanguageBranchEnum });

            // 2. Link Version to its specific LanguageMapping
            modelBuilder.Entity<ContentVersion>()
                .HasOne(v => v.LanguageMapping)
                .WithMany(m => m.Versions)
                .HasForeignKey(v => new { v.ContentId, v.LanguageBranch }); // Composite FK

            // 3. Keep the ActiveVersion pointer
            modelBuilder.Entity<LanguageBranch>()
                .HasOne(m => m.ActiveVersion)
                .WithMany()
                .HasForeignKey(m => m.ActiveVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<ContentVersion>().HasKey(x => x.VersionId);
            //modelBuilder.Entity<ContentVersion>()
            //    .HasOne(v => v.Content)
            //    .WithMany(c => c.Versions)
            //    .HasForeignKey(v => v.ContentId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Content>()
            //    .HasOne(c => c.ActiveVersion)
            //    .WithMany()
            //    .HasForeignKey(c => c.ActiveVersionId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
