using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AirBears.Web.Migrations
{
    public partial class AddedPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    DatePublished = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    FeaturedImageUrl = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        { 
            migrationBuilder.DropTable("Post");         
        }
    }
}
