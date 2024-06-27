using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess_Layer.Data.Migrations
{
    /// <inheritdoc />
    public partial class x : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Boards");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "TaskCards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "TaskCards");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
