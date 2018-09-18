using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddTimeToBlockchainProposals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "blockchainproposals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "blockchainproposals");
        }
    }
}
