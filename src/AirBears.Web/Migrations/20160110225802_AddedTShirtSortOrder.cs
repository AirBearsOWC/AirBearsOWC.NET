using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedTShirtSortOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_User_State_StateId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_User_TeeShirtSize_TeeShirtSizeId", table: "AspNetUsers");
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TeeShirtSize",
                nullable: false,
                defaultValue: 0);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_User_State_StateId", table: "AspNetUsers");
            migrationBuilder.DropForeignKey(name: "FK_User_TeeShirtSize_TeeShirtSizeId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "SortOrder", table: "TeeShirtSize");
            migrationBuilder.AddForeignKey(
                name: "FK_User_State_StateId",
                table: "AspNetUsers",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_User_TeeShirtSize_TeeShirtSizeId",
                table: "AspNetUsers",
                column: "TeeShirtSizeId",
                principalTable: "TeeShirtSize",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
