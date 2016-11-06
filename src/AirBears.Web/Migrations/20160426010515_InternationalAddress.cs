using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class InternationalAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "AddressLine3",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "AddressLine4",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "HasInternationalAddress",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AddressLine1", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AddressLine2", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AddressLine3", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "AddressLine4", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "HasInternationalAddress", table: "AspNetUsers");          
        }
    }
}
