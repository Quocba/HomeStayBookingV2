using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_HomeStay_HomeStayID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Users_UserID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Calendar_HomeStay_HomeStayID",
                table: "Calendar");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentPost_Post_PostID",
                table: "CommentPost");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentPost_Users_UserID",
                table: "CommentPost");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmationTokens_Users_UserId",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_HomeStay_HomeStayID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Users_UserID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay");

            migrationBuilder.DropForeignKey(
                name: "FK_HomestayAmenity_Amenity_AmennityId",
                table: "HomestayAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_HomestayAmenity_HomeStay_HomeStayID",
                table: "HomestayAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeStayImage_HomeStay_HomeStayID",
                table: "HomeStayImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_UserID",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostImage_Post_PostID",
                table: "PostImage");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVoucher_Users_UserID",
                table: "UserVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVoucher_Voucher_VoucherID",
                table: "UserVoucher");

            migrationBuilder.DropIndex(
                name: "IX_HomestayAmenity_AmennityId",
                table: "HomestayAmenity");

            migrationBuilder.DropColumn(
                name: "AmennityId",
                table: "HomestayAmenity");

            migrationBuilder.RenameColumn(
                name: "AmenityID",
                table: "HomestayAmenity",
                newName: "AmenityId");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 34, 30, 906, DateTimeKind.Utc).AddTicks(5220), new DateTime(2025, 2, 13, 14, 34, 30, 906, DateTimeKind.Utc).AddTicks(5232), "$2a$11$BR7LgTCctqzomAfFkifbE.5YLIDc/ms/vp.66SjWGQRodmNxsb/um" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 34, 31, 23, DateTimeKind.Utc).AddTicks(3173), new DateTime(2025, 2, 13, 14, 34, 31, 23, DateTimeKind.Utc).AddTicks(3184), "$2a$11$m8ZnniUKw7GmaDHROMMjwOWp/fQODRJlipQ/mHWj0moH2R2cQB2oe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 34, 30, 787, DateTimeKind.Utc).AddTicks(2154), new DateTime(2025, 2, 13, 14, 34, 30, 787, DateTimeKind.Utc).AddTicks(2162), "$2a$11$t/8MMEUeZo8BB2nXzame6.80fPkszHeMTaDXCp2lK.rAtB5P4w4ce" });

            migrationBuilder.CreateIndex(
                name: "IX_HomestayAmenity_AmenityId",
                table: "HomestayAmenity",
                column: "AmenityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_HomeStay_HomeStayID",
                table: "Booking",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Users_UserID",
                table: "Booking",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Calendar_HomeStay_HomeStayID",
                table: "Calendar",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPost_Post_PostID",
                table: "CommentPost",
                column: "PostID",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPost_Users_UserID",
                table: "CommentPost",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmationTokens_Users_UserId",
                table: "EmailConfirmationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_HomeStay_HomeStayID",
                table: "FeedBack",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Users_UserID",
                table: "FeedBack",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HomestayAmenity_Amenity_AmenityId",
                table: "HomestayAmenity",
                column: "AmenityId",
                principalTable: "Amenity",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HomestayAmenity_HomeStay_HomeStayID",
                table: "HomestayAmenity",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStayImage_HomeStay_HomeStayID",
                table: "HomeStayImage",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Users_UserID",
                table: "Post",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PostImage_Post_PostID",
                table: "PostImage",
                column: "PostID",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVoucher_Users_UserID",
                table: "UserVoucher",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVoucher_Voucher_VoucherID",
                table: "UserVoucher",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_HomeStay_HomeStayID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Users_UserID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Calendar_HomeStay_HomeStayID",
                table: "Calendar");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentPost_Post_PostID",
                table: "CommentPost");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentPost_Users_UserID",
                table: "CommentPost");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailConfirmationTokens_Users_UserId",
                table: "EmailConfirmationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_HomeStay_HomeStayID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedBack_Users_UserID",
                table: "FeedBack");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay");

            migrationBuilder.DropForeignKey(
                name: "FK_HomestayAmenity_Amenity_AmenityId",
                table: "HomestayAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_HomestayAmenity_HomeStay_HomeStayID",
                table: "HomestayAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_HomeStayImage_HomeStay_HomeStayID",
                table: "HomeStayImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_UserID",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_PostImage_Post_PostID",
                table: "PostImage");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVoucher_Users_UserID",
                table: "UserVoucher");

            migrationBuilder.DropForeignKey(
                name: "FK_UserVoucher_Voucher_VoucherID",
                table: "UserVoucher");

            migrationBuilder.DropIndex(
                name: "IX_HomestayAmenity_AmenityId",
                table: "HomestayAmenity");

            migrationBuilder.RenameColumn(
                name: "AmenityId",
                table: "HomestayAmenity",
                newName: "AmenityID");

            migrationBuilder.AddColumn<Guid>(
                name: "AmennityId",
                table: "HomestayAmenity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 31, 3, 886, DateTimeKind.Utc).AddTicks(7295), new DateTime(2025, 2, 13, 14, 31, 3, 886, DateTimeKind.Utc).AddTicks(7307), "$2a$11$l4WYMogNJGY6ym0xVcmqrOyQeV7cw6.8TsfnBEQkZrAFkVSHyydDW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 31, 4, 7, DateTimeKind.Utc).AddTicks(8874), new DateTime(2025, 2, 13, 14, 31, 4, 7, DateTimeKind.Utc).AddTicks(8889), "$2a$11$CxPAjnL.HXTnl9XsdDUCSefrlHazT2Hzmlh0utxWiWY8A0BYHAMUq" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                columns: new[] { "CreatedAt", "LastModifiedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 2, 13, 14, 31, 3, 766, DateTimeKind.Utc).AddTicks(7798), new DateTime(2025, 2, 13, 14, 31, 3, 766, DateTimeKind.Utc).AddTicks(7806), "$2a$11$M.T2UDHCvBS1RkqmPyKFSO3D6mJ3Y5FevxiaNZwTozW7o1v58WIty" });

            migrationBuilder.CreateIndex(
                name: "IX_HomestayAmenity_AmennityId",
                table: "HomestayAmenity",
                column: "AmennityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_HomeStay_HomeStayID",
                table: "Booking",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Users_UserID",
                table: "Booking",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Voucher_VoucherID",
                table: "Booking",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendar_HomeStay_HomeStayID",
                table: "Calendar",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPost_Post_PostID",
                table: "CommentPost",
                column: "PostID",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentPost_Users_UserID",
                table: "CommentPost",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailConfirmationTokens_Users_UserId",
                table: "EmailConfirmationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_HomeStay_HomeStayID",
                table: "FeedBack",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedBack_Users_UserID",
                table: "FeedBack",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStay_Users_UserID",
                table: "HomeStay",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomestayAmenity_Amenity_AmennityId",
                table: "HomestayAmenity",
                column: "AmennityId",
                principalTable: "Amenity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomestayAmenity_HomeStay_HomeStayID",
                table: "HomestayAmenity",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeStayImage_HomeStay_HomeStayID",
                table: "HomeStayImage",
                column: "HomeStayID",
                principalTable: "HomeStay",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Users_UserID",
                table: "Post",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostImage_Post_PostID",
                table: "PostImage",
                column: "PostID",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVoucher_Users_UserID",
                table: "UserVoucher",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVoucher_Voucher_VoucherID",
                table: "UserVoucher",
                column: "VoucherID",
                principalTable: "Voucher",
                principalColumn: "Id");
        }
    }
}
