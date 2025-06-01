            migrationBuilder.AddColumn<bool>(
                name: "IsGoogleAuth",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGoogleAuth",
                table: "Users");
        }
    }
}
