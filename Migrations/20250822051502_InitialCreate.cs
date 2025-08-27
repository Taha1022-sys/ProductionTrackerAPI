using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MkCycleSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Shift = table.Column<int>(type: "int", nullable: false),
                    MoldNo = table.Column<int>(type: "int", nullable: false),
                    Steam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormCount = table.Column<int>(type: "int", nullable: false),
                    MatchingPersonnelCount = table.Column<int>(type: "int", nullable: false),
                    TablePersonnelCount = table.Column<int>(type: "int", nullable: false),
                    ModelNo = table.Column<int>(type: "int", nullable: false),
                    SizeNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ItemsPerPackage = table.Column<int>(type: "int", nullable: false),
                    PackagesPerBag = table.Column<int>(type: "int", nullable: true),
                    BagsPerBox = table.Column<int>(type: "int", nullable: true),
                    TableTotalPackage = table.Column<int>(type: "int", nullable: false),
                    MeasurementError = table.Column<int>(type: "int", nullable: false),
                    KnittingError = table.Column<int>(type: "int", nullable: false),
                    ToeDefect = table.Column<int>(type: "int", nullable: false),
                    OtherDefect = table.Column<int>(type: "int", nullable: false),
                    TotalDefects = table.Column<int>(type: "int", nullable: false),
                    RemainingOnTableCount = table.Column<int>(type: "int", nullable: true),
                    CountTakenFromTable = table.Column<int>(type: "int", nullable: false),
                    MeasurementErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KnittingErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToeDefectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDefectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GeneralErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalTableCount = table.Column<int>(type: "int", nullable: false),
                    TotalTableCountDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalErrorCount = table.Column<int>(type: "int", nullable: false),
                    TotalErrorCountDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeasurementErrorCount = table.Column<int>(type: "int", nullable: false),
                    MeasurementErrorDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeasurementErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KnittingErrorCount = table.Column<int>(type: "int", nullable: false),
                    KnittingErrorDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KnittingErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToeDefectCount = table.Column<int>(type: "int", nullable: false),
                    ToeDefectDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ToeDefectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDefectCount = table.Column<int>(type: "int", nullable: false),
                    OtherDefectDozen = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherDefectRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverallErrorRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionSummaries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionEntries");

            migrationBuilder.DropTable(
                name: "ProductionSummaries");
        }
    }
}
