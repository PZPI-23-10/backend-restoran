using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_restoran.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantModelChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCuisines_Cuisines_CuisineId",
                table: "RestaurantCuisines");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantModerators_Users_UserId",
                table: "RestaurantModerators");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantTags_Tags_TagId",
                table: "RestaurantTags");

            migrationBuilder.DropTable(
                name: "DishTags");

            migrationBuilder.AddColumn<bool>(
                name: "Accessible",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Restaurants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DressCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DressCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantPhoto_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantDressCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    DressCodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantDressCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantDressCodes_DressCodes_DressCodeId",
                        column: x => x.DressCodeId,
                        principalTable: "DressCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestaurantDressCodes_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantDressCodes_DressCodeId",
                table: "RestaurantDressCodes",
                column: "DressCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantDressCodes_RestaurantId",
                table: "RestaurantDressCodes",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPhoto_RestaurantId",
                table: "RestaurantPhoto",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCuisines_Cuisines_CuisineId",
                table: "RestaurantCuisines",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantModerators_Users_UserId",
                table: "RestaurantModerators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantTags_Tags_TagId",
                table: "RestaurantTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantCuisines_Cuisines_CuisineId",
                table: "RestaurantCuisines");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantModerators_Users_UserId",
                table: "RestaurantModerators");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantTags_Tags_TagId",
                table: "RestaurantTags");

            migrationBuilder.DropTable(
                name: "RestaurantDressCodes");

            migrationBuilder.DropTable(
                name: "RestaurantPhoto");

            migrationBuilder.DropTable(
                name: "DressCodes");

            migrationBuilder.DropColumn(
                name: "Accessible",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Restaurants");

            migrationBuilder.CreateTable(
                name: "DishTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DishId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishTags_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishTags_DishId",
                table: "DishTags",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishTags_TagId",
                table: "DishTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantCuisines_Cuisines_CuisineId",
                table: "RestaurantCuisines",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantModerators_Users_UserId",
                table: "RestaurantModerators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_UserId",
                table: "Restaurants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantTags_Tags_TagId",
                table: "RestaurantTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
