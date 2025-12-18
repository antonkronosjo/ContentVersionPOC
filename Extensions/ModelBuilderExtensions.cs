using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace ContentVersionsPOC.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterContentTypes(this ModelBuilder modelBuilder)
        {
            foreach (var type in DiscriminatorBuilderExtensions.GetAllContentTypes())
                modelBuilder.Entity(type);
        }
    }
}
