using Microsoft.EntityFrameworkCore;

namespace simple_file_system.API.Data;

public class FileSystemDbContext : DbContext
{
    public FileSystemDbContext(DbContextOptions<FileSystemDbContext> options) : base(options)
    {
    }

    public DbSet<Models.Node> Nodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.Node>()
            .HasMany(n => n.Children)
            .WithOne(n => n.Parent)
            .HasForeignKey(n => n.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Node>()
            .HasIndex(n => new { n.Name, n.ParentId })
            .IsUnique()
            .HasFilter("[ParentId] IS NOT NULL");

        modelBuilder.Entity<Models.Node>()
            .HasIndex(n => n.Name)
            .IsUnique()
            .HasFilter("[ParentId] IS NULL");
    }
}
