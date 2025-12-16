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
            modelBuilder.Entity<NewsContent>().HasBaseType<Content>().ToTable("News");
            modelBuilder.Entity<EventContent>().HasBaseType<Content>().ToTable("Events");

            modelBuilder.Entity<ContentRoot>().HasKey(x => x.ContentId);

            modelBuilder.Entity<Content>().HasKey(x => x.VersionId);
            modelBuilder.Entity<Content>()
                .HasOne(x => x.LanguageBranch)
                .WithMany(x => x.Versions)
                .HasForeignKey(x => new { x.ContentId, x.Language });
            modelBuilder.Entity<Content>()
                .HasOne(v => v.ContentRoot)
                .WithMany()
                .HasForeignKey(x => x.ContentId);

            modelBuilder.Entity<LanguageBranch>().HasKey(x => new { x.ContentId, x.Language });
            modelBuilder.Entity<LanguageBranch>()
                .HasOne(x => x.ActiveVersion)
                .WithMany()
                .HasForeignKey(x => x.ActiveVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
