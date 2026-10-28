using Microsoft.EntityFrameworkCore;

namespace MigrationBundles.Data.Catalog;

// Runs from UseSeeding/UseAsyncSeeding after every Migrate call, even when no migration
// was pending, so it must check for existing data before inserting.
public static class CatalogSeed
{
    public static void Seed(DbContext context, bool _)
    {
        var categories = context.Set<Category>();
        if (categories.Any())
        {
            return;
        }

        categories.AddRange(CreateCategories());
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken cancellationToken)
    {
        var categories = context.Set<Category>();
        if (await categories.AnyAsync(cancellationToken))
        {
            return;
        }

        categories.AddRange(CreateCategories());
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Category[] CreateCategories() =>
    [
        new()
        {
            Name = "Books",
            Products =
            [
                new() { Name = "Entity Framework Core in Action", Price = 49.90m },
                new() { Name = "C# 14 and .NET 10", Price = 54.50m },
            ],
        },
        new()
        {
            Name = "Courses",
            Products = [new() { Name = "Kubernetes for .NET Developers", Price = 19.99m }],
        },
    ];
}
