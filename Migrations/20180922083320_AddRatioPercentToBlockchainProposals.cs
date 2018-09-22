using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddRatioPercentToBlockchainProposals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RatioPercent",
                table: "blockchainproposals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatioPercent",
                table: "blockchainproposals");
        }
    }
}
