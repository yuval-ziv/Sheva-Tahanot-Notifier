using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShevaTahanotNotifier.Migrations
{
    /// <inheritdoc />
    public partial class AddedBridgeStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "NotificationSchedules",
                newName: "Day");

            migrationBuilder.CreateTable(
                name: "BridgeStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsManualRefresh = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BridgeStatuses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BridgeStatuses");

            migrationBuilder.RenameColumn(
                name: "Day",
                table: "NotificationSchedules",
                newName: "DayOfWeek");
        }
    }
}
