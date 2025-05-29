using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_restoran.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantOwnerFix40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_UserId1",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_UserId1",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Restaurants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Restaurants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_UserId1",
                table: "Restaurants",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_UserId1",
                table: "Restaurants",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
