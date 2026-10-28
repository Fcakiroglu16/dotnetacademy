using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MigrationBundles.Data.Audit;

// Used by dotnet ef and by the audit bundle instead of the API's startup code.
public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // A bundle passes --connection on to the context afterwards, so the key is optional here.
        var connectionString = configuration["Audit:ConnectionString"];

        var options = new DbContextOptionsBuilder<AuditDbContext>();
        AuditDbContext.Configure(options, connectionString);
        return new AuditDbContext(options.Options);
    }
}
