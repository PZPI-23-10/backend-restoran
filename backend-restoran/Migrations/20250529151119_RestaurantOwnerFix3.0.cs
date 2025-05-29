using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_restoran.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantOwnerFix30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Restaurants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurants_OwnerId",
                table: "Restaurants",
                newName: "IX_Restaurants_UserId");

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
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_UserId1",
                table: "Restaurants",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_UserId1",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_UserId1",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Restaurants");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Restaurants",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurants_UserId",
                table: "Restaurants",
                newName: "IX_Restaurants_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
