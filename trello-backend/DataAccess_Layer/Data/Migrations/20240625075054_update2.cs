﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess_Layer.Data.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Workflows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cover",
                table: "TaskCards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "Cover",
                table: "TaskCards");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Boards");
        }
    }
}
