using DemoWUITGallery.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoWUITGallery.Database
{
    public class PhotoGalleryContext : DbContext
    {
        public PhotoGalleryContext(DbContextOptions<PhotoGalleryContext> options) : base(options)
        {
        }

        public DbSet<PhotoGallery> PhotoGalleries { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
