using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dbCalendarUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBooked",
                table: "HomeStay");

            migrationBuilder.AddColumn<bool>(
                name: "isBooked",
                table: "Calendar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 22, 56, 31, 421, DateTimeKind.Utc).AddTicks(2158), new DateTime(2025, 3, 7, 22, 56, 31, 421, DateTimeKind.Utc).AddTicks(2168), "$2a$11$Kh59Mm4Ah5cw3VvqflytqOWepNXcNzwotLxnrU52dIOLPwUOpELYC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 22, 56, 31, 537, DateTimeKind.Utc).AddTicks(6228), new DateTime(2025, 3, 7, 22, 56, 31, 537, DateTimeKind.Utc).AddTicks(6238), "$2a$11$K1/zw.KR91vOXLDJDSsJIuZDpyZNED/A8069.3kzA9ISwcWE1uQde" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 22, 56, 31, 305, DateTimeKind.Utc).AddTicks(6499), new DateTime(2025, 3, 7, 22, 56, 31, 305, DateTimeKind.Utc).AddTicks(6508), "$2a$11$5NUTUeOOeoGnOsUDF8dLqOaeX5JxorObA0yTGhR00JlhZG9SD0JwG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBooked",
                table: "Calendar");

            migrationBuilder.AddColumn<bool>(
                name: "isBooked",
                table: "HomeStay",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 21, 4, 20, 237, DateTimeKind.Utc).AddTicks(7118), new DateTime(2025, 3, 7, 21, 4, 20, 237, DateTimeKind.Utc).AddTicks(7125), "$2a$11$jeKY1HKLW2ZVRtMOANcP6uCEX5Bza2Y2wlsSwHVv5f21BdI7KAIZS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 21, 4, 20, 350, DateTimeKind.Utc).AddTicks(8034), new DateTime(2025, 3, 7, 21, 4, 20, 350, DateTimeKind.Utc).AddTicks(8043), "$2a$11$9v5xA/Ze0zXJEjrjbHPB9O4OpNW4Zw5G//3XDKZ9yVy5ICXi/Y5la" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 7, 21, 4, 20, 124, DateTimeKind.Utc).AddTicks(5920), new DateTime(2025, 3, 7, 21, 4, 20, 124, DateTimeKind.Utc).AddTicks(5926), "$2a$11$e7mSJ6m3ZUHnWZlocIRwOuTP0mF7VTRzWHy2tq8LyvlXHob2BF8im" });
        }
    }
}
