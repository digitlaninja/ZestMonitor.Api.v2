using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class PluraliseTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MasternodeCount",
                table: "MasternodeCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal");

            migrationBuilder.RenameTable(
                name: "MasternodeCount",
                newName: "MasternodeCounts");

            migrationBuilder.RenameTable(
                name: "BlockchainProposal",
                newName: "BlockchainProposals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasternodeCounts",
                table: "MasternodeCounts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockchainProposals",
                table: "BlockchainProposals",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MasternodeCounts",
                table: "MasternodeCounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockchainProposals",
                table: "BlockchainProposals");

            migrationBuilder.RenameTable(
                name: "MasternodeCounts",
                newName: "MasternodeCount");

            migrationBuilder.RenameTable(
                name: "BlockchainProposals",
                newName: "BlockchainProposal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasternodeCount",
                table: "MasternodeCount",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal",
                column: "Id");
        }
    }
}
