using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrMarko.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSaveConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CartEntry",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "CartEntry",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CartEntry");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "CartEntry");
        }
    }
}
