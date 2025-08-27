using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToProductionEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "ProductionEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "ProductionEntries");
        }
    }
}
