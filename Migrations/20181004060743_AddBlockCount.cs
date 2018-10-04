using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddBlockCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockCounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Count = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockCounts", x => x.UpdatedAt);
                    table.UniqueConstraint("AK_BlockCounts_CreatedAt", x => x.CreatedAt);
                    table.UniqueConstraint("AK_BlockCounts_Id", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockCounts");
        }
    }
}
