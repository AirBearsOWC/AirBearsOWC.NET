using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedOrganizationField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Organization",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Organization", table: "AspNetUsers");          
        }
    }
}
