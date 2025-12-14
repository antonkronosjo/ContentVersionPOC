using ContentVersionsPOC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ContentVersionsPOC.Data
{
    public class ContentVersionsPOCContext : DbContext
    {
        public ContentVersionsPOCContext(DbContextOptions<ContentVersionsPOCContext> options) : base(options)
        {
            
        }

        public DbSet<Content> Content => Set<Content>();
        public DbSet<ContentVersion> ContentVersions => Set<ContentVersion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentVersion>().ToTable("ContentVersions");
            modelBuilder.Entity<NewsContent>().ToTable("News");
            modelBuilder.Entity<EventContent>().ToTable("Events");

            modelBuilder.Entity<ContentVersion>().HasKey(x => x.VersionId);
            modelBuilder.Entity<ContentVersion>()
                .HasOne(v => v.Content)
                .WithMany(c => c.Versions)
                .HasForeignKey(v => v.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Content>()
                .HasOne(c => c.ActiveVersion)
                .WithMany()
                .HasForeignKey(c => c.ActiveVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
