using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 10, 8, 23, 16, 403, DateTimeKind.Utc).AddTicks(6813));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 10, 8, 23, 16, 403, DateTimeKind.Utc).AddTicks(7067));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 9, 34, 35, 811, DateTimeKind.Utc).AddTicks(5907));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 9, 9, 34, 35, 811, DateTimeKind.Utc).AddTicks(6234));
        }
    }
}
