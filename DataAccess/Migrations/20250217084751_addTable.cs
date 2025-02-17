using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeStayFacility",
                columns: table => new
                {
                    HomeStayID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeStayFacility", x => new { x.HomeStayID, x.FacilityID });
                    table.ForeignKey(
                        name: "FK_HomeStayFacility_HomeStay_FacilityID",
                        column: x => x.FacilityID,
                        principalTable: "HomeStay",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeStayFacility_HomeStay_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStay",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_HomeStayFacility_FacilityID",
                table: "HomeStayFacility",
                column: "FacilityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "HomeStayFacility");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 15, 20, 45, 1, 768, DateTimeKind.Utc).AddTicks(2280), new DateTime(2025, 2, 15, 20, 45, 1, 768, DateTimeKind.Utc).AddTicks(2292), "$2a$11$x5/9o50xsIzCe9u3x.S5/uPwTgTCmTc8ZlnvsvtbbY/V9IQgmKlT6" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 15, 20, 45, 1, 887, DateTimeKind.Utc).AddTicks(7596), new DateTime(2025, 2, 15, 20, 45, 1, 887, DateTimeKind.Utc).AddTicks(7609), "$2a$11$/HkbbOhjB3m0z3mymHs1T.yJ2wf5h2nAZnQVoC268lW4ITT.se0Gm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 15, 20, 45, 1, 650, DateTimeKind.Utc).AddTicks(2863), new DateTime(2025, 2, 15, 20, 45, 1, 650, DateTimeKind.Utc).AddTicks(2870), "$2a$11$qxj50p.JIWTQ5A59radti.CW2b6dH42hCod8fewf2WJ.th.LExcTO" });
        }
    }
}
