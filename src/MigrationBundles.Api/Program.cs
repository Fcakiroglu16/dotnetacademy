using Microsoft.EntityFrameworkCore;
using MigrationBundles.Data.Audit;
using MigrationBundles.Data.Catalog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Catalog"))
    .UseSeeding(CatalogSeed.Seed)
    .UseAsyncSeeding(CatalogSeed.SeedAsync));

builder.Services.AddDbContext<AuditDbContext>(options =>
    AuditDbContext.Configure(options, builder.Configuration["Audit:ConnectionString"]));

var app = builder.Build();

// Hybrid approach: migrate on startup only for local development.
// Every other environment gets its schema from the migration bundle in CI/CD.
if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<CatalogDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<AuditDbContext>().Database.MigrateAsync();
}

app.MapGet("/products", async (CatalogDbContext db) =>
    await db.Products
        .OrderBy(p => p.Id)
        .Select(p => new ProductResponse(p.Id, p.Name, p.Sku, p.Price, p.Stock, p.Category.Name))
        .ToListAsync());

app.MapPost("/products", async (CreateProductRequest request, CatalogDbContext catalog, AuditDbContext audit) =>
{
    if (!await catalog.Categories.AnyAsync(c => c.Id == request.CategoryId))
    {
        return Results.Problem($"Category {request.CategoryId} does not exist.", statusCode: StatusCodes.Status400BadRequest);
    }

    var product = new Product
    {
        Name = request.Name,
        Sku = request.Sku,
        Price = request.Price,
        Stock = request.Stock,
        CategoryId = request.CategoryId,
    };
    catalog.Products.Add(product);
    await catalog.SaveChangesAsync();

    audit.Entries.Add(new AuditEntry { Action = "product.created", Details = $"{product.Id}: {product.Name}" });
    await audit.SaveChangesAsync();

    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/audit", async (AuditDbContext db) =>
    await db.Entries.OrderByDescending(e => e.Id).Take(20).ToListAsync());

// Readiness check: the app refuses traffic until the bundle has brought the schema up to date.
app.MapGet("/health/schema", async (CatalogDbContext catalog, AuditDbContext audit) =>
{
    var pending = new Dictionary<string, IEnumerable<string>>
    {
        ["catalog"] = await catalog.Database.GetPendingMigrationsAsync(),
        ["audit"] = await audit.Database.GetPendingMigrationsAsync(),
    };

    return pending.Values.Any(p => p.Any())
        ? Results.Json(pending, statusCode: StatusCodes.Status503ServiceUnavailable)
        : Results.Ok(pending);
});

app.Run();

record ProductResponse(int Id, string Name, string? Sku, decimal Price, int Stock, string Category);

record CreateProductRequest(string Name, string? Sku, decimal Price, int Stock, int CategoryId);
