using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_restoran.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDishDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Dishes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Dishes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
