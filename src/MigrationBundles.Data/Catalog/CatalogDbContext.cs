using Microsoft.EntityFrameworkCore;

namespace MigrationBundles.Data.Catalog;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(100);
            category.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.Property(p => p.Name).HasMaxLength(200);
            product.Property(p => p.Price).HasPrecision(10, 2);
            product.Property(p => p.Sku).HasMaxLength(32);
            product.HasIndex(p => p.Sku).IsUnique();
            product.Property(p => p.Stock).HasDefaultValue(0);
        });
    }
}
