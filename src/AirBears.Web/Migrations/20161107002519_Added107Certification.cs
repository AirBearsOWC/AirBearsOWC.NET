using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class Added107Certification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {          
            migrationBuilder.AddColumn<bool>(
                name: "FaaPart107Certified",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaaPart107Certified",
                table: "AspNetUsers");
        }
    }
}
