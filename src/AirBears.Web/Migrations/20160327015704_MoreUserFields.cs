using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class MoreUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NightVisionCapable",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<bool>(
                name: "ThermalVisionCapable",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NightVisionCapable", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "ThermalVisionCapable", table: "AspNetUsers");
        }
    }
}
