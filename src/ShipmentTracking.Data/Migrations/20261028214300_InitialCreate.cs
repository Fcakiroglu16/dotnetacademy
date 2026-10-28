using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracking.Data.Migrations;

/// <inheritdoc />
public partial class _20261028214300_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Shipments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TrackingNumber = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                OriginCountry = table.Column<string>(type: "text", nullable: true, computedColumnSql: "(\"Plan\" #>> '{Stops,0,Address,CountryCode}')", stored: true),
                Tracking_Status = table.Column<string>(type: "text", nullable: false),
                Tracking_Carrier_Code = table.Column<string>(type: "text", nullable: false),
                Tracking_Carrier_ServiceLevel = table.Column<string>(type: "text", nullable: false),
                Tracking_LastScan_HubCode = table.Column<string>(type: "text", nullable: false),
                Tracking_LastScan_ScannedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Plan = table.Column<string>(type: "jsonb", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Shipments", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Shipments_OriginCountry",
            table: "Shipments",
            column: "OriginCountry");

        migrationBuilder.CreateIndex(
            name: "IX_Shipments_Tracking_Carrier_Code_Tracking_LastScan_ScannedAt",
            table: "Shipments",
            columns: new[] { "Tracking_Carrier_Code", "Tracking_LastScan_ScannedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Shipments_Tracking_LastScan_HubCode",
            table: "Shipments",
            column: "Tracking_LastScan_HubCode");

        migrationBuilder.CreateIndex(
            name: "IX_Shipments_TrackingNumber",
            table: "Shipments",
            column: "TrackingNumber",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Shipments");
    }
}
