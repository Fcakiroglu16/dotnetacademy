using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Data.Migrations;

/// <inheritdoc />
public partial class _20261106064048_InitialClinic : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Pets",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Species = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Pets", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Vaccinations",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PetId = table.Column<int>(type: "INTEGER", nullable: false),
                Vaccine = table.Column<string>(type: "TEXT", nullable: false),
                GivenOn = table.Column<DateOnly>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vaccinations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Visits",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                PetId = table.Column<int>(type: "INTEGER", nullable: false),
                StartsAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Visits", x => x.Id);
                table.ForeignKey(
                    name: "FK_Visits_Pets_PetId",
                    column: x => x.PetId,
                    principalTable: "Pets",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Vaccinations_PetId",
            table: "Vaccinations",
            column: "PetId");

        migrationBuilder.CreateIndex(
            name: "IX_Visits_PetId",
            table: "Visits",
            column: "PetId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Vaccinations");

        migrationBuilder.DropTable(
            name: "Visits");

        migrationBuilder.DropTable(
            name: "Pets");
    }
}
