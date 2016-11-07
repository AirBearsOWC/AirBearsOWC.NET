using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedAddressFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Abbr = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "TeeShirtSize",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeeShirtSize", x => x.Id);
                });
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                nullable: false);
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<bool>(
                name: "HasAgreedToTerms",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<Guid>(
                name: "StateId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AddColumn<string>(
                name: "Street1",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<string>(
                name: "Street2",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "TeeShirtSizeId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "AspNetUsers",
                column: "Id");
            migrationBuilder.AddForeignKey(
                name: "FK_User_State_StateId",
                table: "AspNetUsers",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_User_TeeShirtSize_TeeShirtSizeId",
                table: "AspNetUsers",
                column: "TeeShirtSizeId",
                principalTable: "TeeShirtSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_User_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_User_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_User_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_User_State_StateId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_User_TeeShirtSize_TeeShirtSizeId", table: "AspNetUsers");
            //migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            //migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_User_UserId", table: "AspNetUserClaims");
            //migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_User_UserId", table: "AspNetUserLogins");
            //migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            //migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_User_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropPrimaryKey(name: "PK_User", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "City", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "FirstName", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "HasAgreedToTerms", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "LastName", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "StateId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Street1", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Street2", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "TeeShirtSizeId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Zip", table: "AspNetUsers");
            migrationBuilder.DropTable("State");
            migrationBuilder.DropTable("TeeShirtSize");
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                nullable: true);
            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUser",
                table: "AspNetUsers",
                column: "Id");
            //migrationBuilder.AddForeignKey(
            //    name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
            //    table: "AspNetRoleClaims",
            //    column: "RoleId",
            //    principalTable: "AspNetRoles",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
            //migrationBuilder.AddForeignKey(
            //    name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
            //    table: "AspNetUserClaims",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
            //migrationBuilder.AddForeignKey(
            //    name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
            //    table: "AspNetUserLogins",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
            //migrationBuilder.AddForeignKey(
            //    name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
            //    table: "AspNetUserRoles",
            //    column: "RoleId",
            //    principalTable: "AspNetRoles",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
            //migrationBuilder.AddForeignKey(
            //    name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
            //    table: "AspNetUserRoles",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
