using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedPrivacyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowsPilotSearch",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: true);
            migrationBuilder.AddColumn<bool>(
                name: "SubscribesToUpdates",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AllowsPilotSearch", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "SubscribesToUpdates", table: "AspNetUsers");
        }
    }
}
