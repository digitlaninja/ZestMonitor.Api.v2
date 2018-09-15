using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class RenameBlockchainProposals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal");

            migrationBuilder.RenameTable(
                name: "BlockchainProposal",
                newName: "blockchainproposals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_blockchainproposals",
                table: "blockchainproposals",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_blockchainproposals",
                table: "blockchainproposals");

            migrationBuilder.RenameTable(
                name: "blockchainproposals",
                newName: "BlockchainProposal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal",
                column: "Id");
        }
    }
}
