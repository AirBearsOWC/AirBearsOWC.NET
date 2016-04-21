using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class LoginPasswordTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangeDate",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "LastLoginDate", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "LastPasswordChangeDate", table: "AspNetUsers");
        }
    }
}
