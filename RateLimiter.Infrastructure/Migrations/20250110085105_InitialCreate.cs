using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RateLimiter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRateLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BaseLimit = table.Column<int>(type: "int", nullable: false),
                    BonusRequests = table.Column<int>(type: "int", nullable: false),
                    RolloverRequests = table.Column<int>(type: "int", nullable: false),
                    LastResetTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRateLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRateLimits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Name", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Admin User", "admin123", 2, "admin" },
                    { 2, "Regular User", "user123", 1, "user" },
                    { 3, "Guest User", "guest123", 0, "guest" }
                });

            migrationBuilder.InsertData(
                table: "UserRateLimits",
                columns: new[] { "Id", "BaseLimit", "BonusRequests", "LastResetTime", "RolloverRequests", "UserId" },
                values: new object[,]
                {
                    { 1, 1000, 0, new DateTime(2025, 1, 10, 8, 51, 4, 625, DateTimeKind.Utc).AddTicks(870), 0, 1 },
                    { 2, 100, 0, new DateTime(2025, 1, 10, 8, 51, 4, 625, DateTimeKind.Utc).AddTicks(872), 0, 2 },
                    { 3, 10, 0, new DateTime(2025, 1, 10, 8, 51, 4, 625, DateTimeKind.Utc).AddTicks(873), 0, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestLogs_UserId",
                table: "RequestLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRateLimits_UserId",
                table: "UserRateLimits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestLogs");

            migrationBuilder.DropTable(
                name: "UserRateLimits");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
