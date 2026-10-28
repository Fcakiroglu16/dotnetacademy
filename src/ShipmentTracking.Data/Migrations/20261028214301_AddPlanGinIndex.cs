using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracking.Data.Migrations;

/// <summary>
/// Hand-written, PostgreSQL-only: a GIN index over the Plan jsonb column for
/// containment (@>) queries. The EF Core model cannot express it.
/// </summary>
public partial class _20261028214301_AddPlanGinIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""CREATE INDEX "IX_Shipments_Plan_Gin" ON "Shipments" USING GIN ("Plan" jsonb_path_ops);""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DROP INDEX "IX_Shipments_Plan_Gin";""");
    }
}
