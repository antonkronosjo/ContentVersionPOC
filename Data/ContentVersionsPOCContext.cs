using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentVersionsPOC.Data
{
    public class ContentVersionsPOCContext : DbContext
    {
        public ContentVersionsPOCContext(DbContextOptions<ContentVersionsPOCContext> options) : base(options)
        {
            
        }

        public DbSet<ContentRoot> ContentRoots => Set<ContentRoot>();
        public DbSet<Content> Content => Set<Content>();
        public DbSet<LanguageBranch> LanguageBranches => Set<LanguageBranch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>().ToTable("ContentVersions");
            modelBuilder.Entity<NewsContent>().ToTable("News");
            modelBuilder.Entity<EventContent>().ToTable("Events");

            modelBuilder.Entity<ContentRoot>().HasKey(x => x.ContentId);

            modelBuilder.Entity<Content>().HasKey(x => x.VersionId);
            modelBuilder.Entity<Content>()
                .HasOne(v => v.LanguageBranch)
                .WithMany(m => m.Versions)
                .HasForeignKey(v => new { v.ContentId, v.Language });

            modelBuilder.Entity<LanguageBranch>().HasKey(m => new { m.ContentId, m.Language });
            modelBuilder.Entity<LanguageBranch>()
                .HasOne(m => m.ActiveVersion)
                .WithMany()
                .HasForeignKey(m => m.ActiveVersionId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
