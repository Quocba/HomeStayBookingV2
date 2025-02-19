using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTableFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateAt",
                table: "Facility",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Facility",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 19, 18, 55, 30, 386, DateTimeKind.Utc).AddTicks(7530), new DateTime(2025, 2, 19, 18, 55, 30, 386, DateTimeKind.Utc).AddTicks(7532), "$2a$11$OXNTN3DX/qiCOsb85xT/j.gD3wmhx/HXQbhxiokqYA.YKOfcgpR5e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 19, 18, 55, 30, 498, DateTimeKind.Utc).AddTicks(2447), new DateTime(2025, 2, 19, 18, 55, 30, 498, DateTimeKind.Utc).AddTicks(2448), "$2a$11$EC6i/JG6s4X3z6S4ZQdiAO2lMDSLJp67YvGMcID9kRLoDXnfITUH6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 19, 18, 55, 30, 275, DateTimeKind.Utc).AddTicks(3676), new DateTime(2025, 2, 19, 18, 55, 30, 275, DateTimeKind.Utc).AddTicks(3683), "$2a$11$QmaoNKpHvSXaH7z.L5JNEu2BqPOqCAA11K3j5fgTSXyZ/DzlH3k9q" });

        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Facility");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateAt",
                table: "Facility",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

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
    }
}
