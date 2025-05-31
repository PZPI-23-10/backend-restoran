using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_restoran.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantPhoto_Restaurants_RestaurantId",
                table: "RestaurantPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantPhoto",
                table: "RestaurantPhoto");

            migrationBuilder.RenameTable(
                name: "RestaurantPhoto",
                newName: "RestaurantPhotos");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantPhoto_RestaurantId",
                table: "RestaurantPhotos",
                newName: "IX_RestaurantPhotos_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantPhotos",
                table: "RestaurantPhotos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RestaurantId",
                table: "Reviews",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantPhotos_Restaurants_RestaurantId",
                table: "RestaurantPhotos",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantPhotos_Restaurants_RestaurantId",
                table: "RestaurantPhotos");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantPhotos",
                table: "RestaurantPhotos");

            migrationBuilder.RenameTable(
                name: "RestaurantPhotos",
                newName: "RestaurantPhoto");

            migrationBuilder.RenameIndex(
                name: "IX_RestaurantPhotos_RestaurantId",
                table: "RestaurantPhoto",
                newName: "IX_RestaurantPhoto_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantPhoto",
                table: "RestaurantPhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantPhoto_Restaurants_RestaurantId",
                table: "RestaurantPhoto",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
