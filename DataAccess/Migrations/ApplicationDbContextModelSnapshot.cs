﻿// <auto-generated />
using System;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BusinessObject.Entities.Amenity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Amenity");
                });

            modelBuilder.Entity("BusinessObject.Entities.Booking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOutDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReasonCancel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPrice")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("UnitPrice")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserID");

                    b.ToTable("Booking");
                });

            modelBuilder.Entity("BusinessObject.Entities.Calendar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("BookingID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("HomeStayID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("BookingID");

                    b.HasIndex("HomeStayID");

                    b.ToTable("Calendar");
                });

            modelBuilder.Entity("BusinessObject.Entities.CommentPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CommontDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ParrentID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ParrentID");

                    b.HasIndex("PostID");

                    b.HasIndex("UserID");

                    b.ToTable("CommentPost");
                });

            modelBuilder.Entity("BusinessObject.Entities.EmailConfirmationToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailConfirmationTokens");
                });

            modelBuilder.Entity("BusinessObject.Entities.FeedBack", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HomeStayID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("HomeStayID");

                    b.HasIndex("UserID");

                    b.ToTable("FeedBack");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomeStay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CheckInTime")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CheckOutTime")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("MainImage")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OpenIn")
                        .HasColumnType("int");

                    b.Property<int>("Standar")
                        .HasColumnType("int");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isBooked")
                        .HasColumnType("bit");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserID");

                    b.ToTable("HomeStay");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomeStayImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("HomeStayID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("HomeStayID");

                    b.ToTable("HomeStayImage");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomestayAmenity", b =>
                {
                    b.Property<Guid>("HomeStayID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AmenityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("HomeStayID", "AmenityId");

                    b.HasIndex("AmenityId");

                    b.ToTable("HomestayAmenity");
                });

            modelBuilder.Entity("BusinessObject.Entities.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReasonReject")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserID");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("BusinessObject.Entities.PostImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("PostID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("PostID");

                    b.ToTable("PostImage");
                });

            modelBuilder.Entity("BusinessObject.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("BusinessObject.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Manager"
                        },
                        new
                        {
                            Id = 3,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("BusinessObject.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Avatar")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("BirhDay")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CitizenID")
                        .HasMaxLength(100)
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Phone")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d87b4b72-609b-4979-b758-7771481da883"),
                            Address = "Ninh Kiều, Cần Thơ",
                            CreatedAt = new DateTime(2025, 2, 15, 20, 45, 1, 650, DateTimeKind.Utc).AddTicks(2863),
                            Email = "admin@gmail.com",
                            FullName = "admin",
                            IsDeleted = false,
                            IsEmailConfirmed = true,
                            LastModifiedAt = new DateTime(2025, 2, 15, 20, 45, 1, 650, DateTimeKind.Utc).AddTicks(2870),
                            PasswordHash = "$2a$11$qxj50p.JIWTQ5A59radti.CW2b6dH42hCod8fewf2WJ.th.LExcTO",
                            Phone = "0987654321",
                            RoleId = 1
                        },
                        new
                        {
                            Id = new Guid("4b7b0200-70f9-416a-9a3f-29ccab0deec4"),
                            Address = "Bình Thủy, Cần Thơ",
                            CreatedAt = new DateTime(2025, 2, 15, 20, 45, 1, 768, DateTimeKind.Utc).AddTicks(2280),
                            Email = "staff@gmail.com",
                            FullName = "staff",
                            IsDeleted = false,
                            IsEmailConfirmed = true,
                            LastModifiedAt = new DateTime(2025, 2, 15, 20, 45, 1, 768, DateTimeKind.Utc).AddTicks(2292),
                            PasswordHash = "$2a$11$x5/9o50xsIzCe9u3x.S5/uPwTgTCmTc8ZlnvsvtbbY/V9IQgmKlT6",
                            Phone = "0987654123",
                            RoleId = 2
                        },
                        new
                        {
                            Id = new Guid("a85f272f-353e-4ff6-be2b-a15f1e7c0c47"),
                            Address = "Phong Điền, Cần Thơ",
                            CreatedAt = new DateTime(2025, 2, 15, 20, 45, 1, 887, DateTimeKind.Utc).AddTicks(7596),
                            Email = "user@gmail.com",
                            FullName = "user",
                            IsDeleted = false,
                            IsEmailConfirmed = true,
                            LastModifiedAt = new DateTime(2025, 2, 15, 20, 45, 1, 887, DateTimeKind.Utc).AddTicks(7609),
                            PasswordHash = "$2a$11$/HkbbOhjB3m0z3mymHs1T.yJ2wf5h2nAZnQVoC268lW4ITT.se0Gm",
                            Phone = "0987654312",
                            RoleId = 3
                        });
                });

            modelBuilder.Entity("BusinessObject.Entities.UserVoucher", b =>
                {
                    b.Property<Guid>("UserID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VoucherID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isUsed")
                        .HasColumnType("bit");

                    b.HasKey("UserID", "VoucherID");

                    b.HasIndex("VoucherID");

                    b.ToTable("UserVoucher");
                });

            modelBuilder.Entity("BusinessObject.Entities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<double>("Discount")
                        .HasColumnType("float");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("QuantityUsed")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Voucher");
                });

            modelBuilder.Entity("BusinessObject.Entities.Booking", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.Calendar", b =>
                {
                    b.HasOne("BusinessObject.Entities.Booking", "Booking")
                        .WithMany("Calendars")
                        .HasForeignKey("BookingID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("BusinessObject.Entities.HomeStay", "HomeStay")
                        .WithMany("Calendars")
                        .HasForeignKey("HomeStayID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Booking");

                    b.Navigation("HomeStay");
                });

            modelBuilder.Entity("BusinessObject.Entities.CommentPost", b =>
                {
                    b.HasOne("BusinessObject.Entities.CommentPost", "ReplyToUser")
                        .WithMany("ChildComments")
                        .HasForeignKey("ParrentID")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("BusinessObject.Entities.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("CommentPosts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("ReplyToUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.EmailConfirmationToken", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.FeedBack", b =>
                {
                    b.HasOne("BusinessObject.Entities.HomeStay", "HomeStay")
                        .WithMany()
                        .HasForeignKey("HomeStayID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("FeedBacks")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("HomeStay");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomeStay", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("HomeStays")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomeStayImage", b =>
                {
                    b.HasOne("BusinessObject.Entities.HomeStay", "HomeStay")
                        .WithMany("HomestayImages")
                        .HasForeignKey("HomeStayID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("HomeStay");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomestayAmenity", b =>
                {
                    b.HasOne("BusinessObject.Entities.Amenity", "Amenity")
                        .WithMany("HomeStayAmenities")
                        .HasForeignKey("AmenityId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.HomeStay", "HomeStay")
                        .WithMany("HomestayAmenities")
                        .HasForeignKey("HomeStayID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Amenity");

                    b.Navigation("HomeStay");
                });

            modelBuilder.Entity("BusinessObject.Entities.Post", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.PostImage", b =>
                {
                    b.HasOne("BusinessObject.Entities.Post", "Post")
                        .WithMany("PostImages")
                        .HasForeignKey("PostID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("BusinessObject.Entities.RefreshToken", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObject.Entities.User", b =>
                {
                    b.HasOne("BusinessObject.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BusinessObject.Entities.UserVoucher", b =>
                {
                    b.HasOne("BusinessObject.Entities.User", "user")
                        .WithMany("UserVouchers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BusinessObject.Entities.Voucher", "voucher")
                        .WithMany("UserVouchers")
                        .HasForeignKey("VoucherID")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("user");

                    b.Navigation("voucher");
                });

            modelBuilder.Entity("BusinessObject.Entities.Amenity", b =>
                {
                    b.Navigation("HomeStayAmenities");
                });

            modelBuilder.Entity("BusinessObject.Entities.Booking", b =>
                {
                    b.Navigation("Calendars");
                });

            modelBuilder.Entity("BusinessObject.Entities.CommentPost", b =>
                {
                    b.Navigation("ChildComments");
                });

            modelBuilder.Entity("BusinessObject.Entities.HomeStay", b =>
                {
                    b.Navigation("Calendars");

                    b.Navigation("HomestayAmenities");

                    b.Navigation("HomestayImages");
                });

            modelBuilder.Entity("BusinessObject.Entities.Post", b =>
                {
                    b.Navigation("PostImages");
                });

            modelBuilder.Entity("BusinessObject.Entities.User", b =>
                {
                    b.Navigation("Bookings");

                    b.Navigation("CommentPosts");

                    b.Navigation("FeedBacks");

                    b.Navigation("HomeStays");

                    b.Navigation("Posts");

                    b.Navigation("RefreshTokens");

                    b.Navigation("UserVouchers");
                });

            modelBuilder.Entity("BusinessObject.Entities.Voucher", b =>
                {
                    b.Navigation("UserVouchers");
                });
#pragma warning restore 612, 618
        }
    }
}
