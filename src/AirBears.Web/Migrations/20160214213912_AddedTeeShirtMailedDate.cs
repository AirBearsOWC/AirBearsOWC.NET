using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedTeeShirtMailedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TeeShirtMailedDate",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "TeeShirtMailedDate", table: "AspNetUsers");
        }
    }
}
