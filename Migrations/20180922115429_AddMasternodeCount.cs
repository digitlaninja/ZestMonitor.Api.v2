using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddMasternodeCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_blockchainproposals",
                table: "blockchainproposals");

            migrationBuilder.RenameTable(
                name: "blockchainproposals",
                newName: "BlockchainProposal");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BlockchainProposal",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MasternodeCount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Stable = table.Column<int>(nullable: false),
                    ObfCompat = table.Column<int>(nullable: false),
                    Enabled = table.Column<int>(nullable: false),
                    InQueue = table.Column<int>(nullable: false),
                    IPv4 = table.Column<int>(nullable: false),
                    IPv6 = table.Column<int>(nullable: false),
                    Onion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasternodeCount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasternodeCount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockchainProposal",
                table: "BlockchainProposal");

            migrationBuilder.RenameTable(
                name: "BlockchainProposal",
                newName: "blockchainproposals");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "blockchainproposals",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_blockchainproposals",
                table: "blockchainproposals",
                column: "Id");
        }
    }
}
