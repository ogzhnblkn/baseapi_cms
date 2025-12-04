using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseApi.Migrations
{
    /// <inheritdoc />
    public partial class token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TokenBlacklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BlacklistedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenBlacklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenBlacklists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklists_ExpiresAt",
                table: "TokenBlacklists",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklists_Token",
                table: "TokenBlacklists",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklists_Token_ExpiresAt",
                table: "TokenBlacklists",
                columns: new[] { "Token", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TokenBlacklists_UserId",
                table: "TokenBlacklists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenBlacklists");
        }
    }
}
