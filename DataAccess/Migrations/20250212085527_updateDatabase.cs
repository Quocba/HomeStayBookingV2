using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "HomeStay",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 12, 15, 55, 26, 422, DateTimeKind.Utc).AddTicks(5665), new DateTime(2025, 2, 12, 15, 55, 26, 422, DateTimeKind.Utc).AddTicks(5676), "$2a$11$5v0r2rLIqq/OiTPQtMDR2umgq6ZlKvfc93ioa9Dh9uWXWyvWqydIa" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 12, 15, 55, 26, 545, DateTimeKind.Utc).AddTicks(8859), new DateTime(2025, 2, 12, 15, 55, 26, 545, DateTimeKind.Utc).AddTicks(8871), "$2a$11$hae7ZvpaMDmzelCGsqngzO5zSHFR845eXrVdDlaW.7vYJXdlGhYr." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 12, 15, 55, 26, 302, DateTimeKind.Utc).AddTicks(276), new DateTime(2025, 2, 12, 15, 55, 26, 302, DateTimeKind.Utc).AddTicks(282), "$2a$11$UgrmZAFeUwttAc9lxENVweaERf5yH/8bk2zSpar.IGmZl0qwhPVFa" });

            migrationBuilder.CreateIndex(
                name: "IX_HomeStay_UserID",
                table: "HomeStay",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay");

            migrationBuilder.DropIndex(
                name: "IX_HomeStay_UserID",
                table: "HomeStay");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "HomeStay");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 11, 23, 28, 16, 278, DateTimeKind.Utc).AddTicks(2464), new DateTime(2025, 2, 11, 23, 28, 16, 278, DateTimeKind.Utc).AddTicks(2477), "$2a$11$Oecs54ZTQsw1oEFC0ALr3.Pcca0cve8q.k5NPnJmwgEgm6k/qCZre" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 11, 23, 28, 16, 393, DateTimeKind.Utc).AddTicks(4150), new DateTime(2025, 2, 11, 23, 28, 16, 393, DateTimeKind.Utc).AddTicks(4167), "$2a$11$dq3w1V.pzmIMGvNUfOqndOA0SP8FRXn6MNSvl6JqAxv4VKEW8jfmG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 11, 23, 28, 16, 163, DateTimeKind.Utc).AddTicks(9432), new DateTime(2025, 2, 11, 23, 28, 16, 163, DateTimeKind.Utc).AddTicks(9440), "$2a$11$nihmuFapYmJUW9MsBzyKfO9/wAhLmEmVsk872fXPJt9WvBaQb4ik2" });
        }
    }
}
