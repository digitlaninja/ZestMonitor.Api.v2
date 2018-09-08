using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddBlockchainProposals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockchainProposal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    FeeHash = table.Column<string>(nullable: true),
                    Yeas = table.Column<int>(nullable: false),
                    Nays = table.Column<int>(nullable: false),
                    Abstains = table.Column<int>(nullable: false),
                    IsEstablished = table.Column<bool>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    IsValidReason = table.Column<string>(nullable: true),
                    FValid = table.Column<bool>(nullable: false),
                    Ratio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainProposal", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockchainProposal");
        }
    }
}
