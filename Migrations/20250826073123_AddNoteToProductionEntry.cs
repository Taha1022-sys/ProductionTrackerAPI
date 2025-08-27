using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteToProductionEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProductionEntries",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProductionEntries");
        }
    }
}
