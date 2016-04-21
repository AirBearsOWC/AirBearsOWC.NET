using System;
using Microsoft.Data.Entity.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class IsAuthorityAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Zip",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AlterColumn<Guid>(
                name: "TeeShirtSizeId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "Street1",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AlterColumn<Guid>(
                name: "StateId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "IsAuthorityAccount",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsAuthorityAccount", table: "AspNetUsers");
            migrationBuilder.AlterColumn<string>(
                name: "Zip",
                table: "AspNetUsers",
                nullable: false);
            migrationBuilder.AlterColumn<Guid>(
                name: "TeeShirtSizeId",
                table: "AspNetUsers",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "Street1",
                table: "AspNetUsers",
                nullable: false);
            migrationBuilder.AlterColumn<Guid>(
                name: "StateId",
                table: "AspNetUsers",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "AspNetUsers",
                nullable: false);
        }
    }
}
