﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrMarko.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityToCartEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartEntry",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartEntry");
        }
    }
}
