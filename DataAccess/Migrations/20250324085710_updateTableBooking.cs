using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTableBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeStayAddress",
                table: "Booking",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeStayImage",
                table: "Booking",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeStayName",
                table: "Booking",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 24, 15, 57, 9, 208, DateTimeKind.Utc).AddTicks(1118), new DateTime(2025, 3, 24, 15, 57, 9, 208, DateTimeKind.Utc).AddTicks(1125), "$2a$11$2xO0FRB/IHFU8EsSr/AdSePlBP1DuBFY4NwB/td8uw5OdEShhH9/O" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 24, 15, 57, 9, 321, DateTimeKind.Utc).AddTicks(7596), new DateTime(2025, 3, 24, 15, 57, 9, 321, DateTimeKind.Utc).AddTicks(7605), "$2a$11$HK5OB2q3yWht5T/ZkS8duebwPtDdLoVJxJi5POfxa4if1M5isaxra" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 3, 24, 15, 57, 9, 92, DateTimeKind.Utc).AddTicks(3606), new DateTime(2025, 3, 24, 15, 57, 9, 92, DateTimeKind.Utc).AddTicks(3613), "$2a$11$3mOFvu3LMGL2vkWzril6nep94IwZQzY/VOoXFnIuLM4duI8.WyCz." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeStayAddress",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HomeStayImage",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "HomeStayName",
                table: "Booking");

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
    }
}
