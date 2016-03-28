using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedUserFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightTime",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightTime", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Payload",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payload", x => x.Id);
                });
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "FemaIcsCertified",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<Guid>(
                name: "FlightTimeId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "HamRadioLicensed",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<Guid>(
                name: "PayloadId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_User_FlightTime_FlightTimeId",
                table: "AspNetUsers",
                column: "FlightTimeId",
                principalTable: "FlightTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_User_Payload_PayloadId",
                table: "AspNetUsers",
                column: "PayloadId",
                principalTable: "Payload",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_User_FlightTime_FlightTimeId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_User_Payload_PayloadId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "Bio", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "FemaIcsCertified", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "FlightTimeId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "HamRadioLicensed", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "PayloadId", table: "AspNetUsers");
            migrationBuilder.DropTable("FlightTime");
            migrationBuilder.DropTable("Payload");
        }
    }
}
