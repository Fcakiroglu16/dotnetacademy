using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NestedGameState.Data.Migrations;

/// <summary>
/// Hand-written, Postgres-only: a GIN index over the Board jsonb column for
/// containment (@>) queries. The EF Core model cannot express it.
/// </summary>
public partial class _20261028132140_AddBoardGinIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""CREATE INDEX "IX_SavedGames_Board_Gin" ON "SavedGames" USING GIN ("Board" jsonb_path_ops);""");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""DROP INDEX "IX_SavedGames_Board_Gin";""");
    }
}
