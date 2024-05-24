using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrMarko.Data.Migrations
{
    /// <inheritdoc />
    public partial class SliderAddButtonText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ButtonText",
                table: "Slider",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonText",
                table: "Slider");
        }
    }
}
