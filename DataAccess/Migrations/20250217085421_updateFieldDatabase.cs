using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateFieldDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "HomeStayFacility",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 54, 20, 439, DateTimeKind.Utc).AddTicks(8365), new DateTime(2025, 2, 17, 15, 54, 20, 439, DateTimeKind.Utc).AddTicks(8375), "$2a$11$zWvQWgoc5nd59bdetNY8Iuo6NUQudCyZ/.5iiOGijgIdLkLPulsju" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 54, 20, 554, DateTimeKind.Utc).AddTicks(7826), new DateTime(2025, 2, 17, 15, 54, 20, 554, DateTimeKind.Utc).AddTicks(7836), "$2a$11$BtxCSiriaSg1nj7KjTxdcuWFaFJU7HV1l13vlSvmqBf4p4D6CSkj6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 54, 20, 323, DateTimeKind.Utc).AddTicks(7613), new DateTime(2025, 2, 17, 15, 54, 20, 323, DateTimeKind.Utc).AddTicks(7621), "$2a$11$XN31FF0PxZRZc.TgbZ72qOmgYWCE.hyX/YETFrJaxS8sQ3R/FPHGe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "HomeStayFacility");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 50, 15, 31, DateTimeKind.Utc).AddTicks(4954), new DateTime(2025, 2, 17, 15, 50, 15, 31, DateTimeKind.Utc).AddTicks(4972), "$2a$11$n5ggCdWy.Qd775BHGAugUuuquPuReDi7MhHFi2esMuI9g/lB2OBSu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 50, 15, 149, DateTimeKind.Utc).AddTicks(4769), new DateTime(2025, 2, 17, 15, 50, 15, 149, DateTimeKind.Utc).AddTicks(4779), "$2a$11$pPsfs8tJzXF9kh0M0BsGVOu2uFAk1xhbDbOkzeq3fzuU3Nt.nVelu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 50, 14, 915, DateTimeKind.Utc).AddTicks(4658), new DateTime(2025, 2, 17, 15, 50, 14, 915, DateTimeKind.Utc).AddTicks(4665), "$2a$11$J8mgSOKEKH2zno5RnKOR4ujF/1fC9bf26z3bWzTZxlayNq1FxARPK" });
        }
    }
}
