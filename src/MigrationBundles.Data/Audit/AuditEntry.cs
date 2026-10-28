namespace MigrationBundles.Data.Audit;

public class AuditEntry
{
    public long Id { get; set; }
    public required string Action { get; set; }
    public required string Details { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
