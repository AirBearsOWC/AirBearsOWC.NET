using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AirBears.Web.Models;

namespace AirBears.Web.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20161107002519_Added107Certification")]
    partial class Added107Certification
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AirBears.Web.Models.FlightTime", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("Id");

                    b.ToTable("FlightTime");
                });

            modelBuilder.Entity("AirBears.Web.Models.Payload", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("Id");

                    b.ToTable("Payload");
                });

            modelBuilder.Entity("AirBears.Web.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime?>("DatePublished");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("FeaturedImageUrl");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("AirBears.Web.Models.State", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Abbr");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("State");
                });

            modelBuilder.Entity("AirBears.Web.Models.TeeShirtSize", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("SortOrder");

                    b.HasKey("Id");

                    b.ToTable("TeeShirtSize");
                });

            modelBuilder.Entity("AirBears.Web.Models.User", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AddressLine1")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("AddressLine2")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("AddressLine3")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("AddressLine4")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<bool>("AllowsPilotSearch");

                    b.Property<string>("Bio")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("City")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("DateRegistered");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("FaaPart107Certified");

                    b.Property<bool>("FemaIcsCertified");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<Guid?>("FlightTimeId");

                    b.Property<string>("GeocodeAddress")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<bool>("HamRadioLicensed");

                    b.Property<bool>("HasAgreedToTerms");

                    b.Property<bool>("HasInternationalAddress");

                    b.Property<bool>("IsAuthorityAccount");

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 100);

                    b.Property<DateTime?>("LastPasswordChangeDate");

                    b.Property<double?>("Latitude");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<double?>("Longitude");

                    b.Property<bool>("NightVisionCapable");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Organization")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("PasswordHash");

                    b.Property<Guid?>("PayloadId");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<Guid?>("StateId");

                    b.Property<string>("Street1")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("Street2")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<bool>("SubscribesToUpdates");

                    b.Property<DateTime?>("TeeShirtMailedDate");

                    b.Property<Guid?>("TeeShirtSizeId");

                    b.Property<bool>("ThermalVisionCapable");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Zip")
                        .HasAnnotation("MaxLength", 10);

                    b.HasKey("Id");

                    b.HasIndex("FlightTimeId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("PayloadId");

                    b.HasIndex("StateId");

                    b.HasIndex("TeeShirtSizeId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("AirBears.Web.Models.User", b =>
                {
                    b.HasOne("AirBears.Web.Models.FlightTime", "FlightTime")
                        .WithMany()
                        .HasForeignKey("FlightTimeId");

                    b.HasOne("AirBears.Web.Models.Payload", "Payload")
                        .WithMany()
                        .HasForeignKey("PayloadId");

                    b.HasOne("AirBears.Web.Models.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId");

                    b.HasOne("AirBears.Web.Models.TeeShirtSize", "TeeShirtSize")
                        .WithMany()
                        .HasForeignKey("TeeShirtSizeId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AirBears.Web.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AirBears.Web.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AirBears.Web.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
