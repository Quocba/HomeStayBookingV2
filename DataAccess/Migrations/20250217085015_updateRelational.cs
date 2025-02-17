using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateRelational : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeStayFacility_HomeStay_FacilityID",
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

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStayFacility_Facility_FacilityID",
                table: "HomeStayFacility",
                column: "FacilityID",
                principalTable: "Facility",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeStayFacility_Facility_FacilityID",
                table: "HomeStayFacility");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 47, 51, 277, DateTimeKind.Utc).AddTicks(7839), new DateTime(2025, 2, 17, 15, 47, 51, 277, DateTimeKind.Utc).AddTicks(7853), "$2a$11$HtxffAbvGy228evKWGkWaeB34W4Qqy78DOwvOKDviQAf0X.YIi35W" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 47, 51, 396, DateTimeKind.Utc).AddTicks(6888), new DateTime(2025, 2, 17, 15, 47, 51, 396, DateTimeKind.Utc).AddTicks(6899), "$2a$11$s4kKFd77X7Mu8W24n50QheiSjwH9hDa4Ei9VC1z9KIzPKS5mpzYkq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 17, 15, 47, 51, 164, DateTimeKind.Utc).AddTicks(4799), new DateTime(2025, 2, 17, 15, 47, 51, 164, DateTimeKind.Utc).AddTicks(4806), "$2a$11$xU54yHoRsEV3uOngdyNrK.JxafbvJCbOKDJSldeyZhzMmhoSMvnZe" });

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStayFacility_HomeStay_FacilityID",
                table: "HomeStayFacility",
                column: "FacilityID",
                principalTable: "HomeStay",
                principalColumn: "Id");
        }
    }
}
