using Microsoft.EntityFrameworkCore.Migrations;

namespace ZestMonitor.Api.Migrations
{
    public partial class AddExtraBlockchainData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockEnd",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlockStart",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPayment",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RemainingPaymentCount",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayment",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalPaymentCount",
                table: "BlockchainProposal",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockEnd",
                table: "BlockchainProposal");

            migrationBuilder.DropColumn(
                name: "BlockStart",
                table: "BlockchainProposal");

            migrationBuilder.DropColumn(
                name: "MonthlyPayment",
                table: "BlockchainProposal");

            migrationBuilder.DropColumn(
                name: "RemainingPaymentCount",
                table: "BlockchainProposal");

            migrationBuilder.DropColumn(
                name: "TotalPayment",
                table: "BlockchainProposal");

            migrationBuilder.DropColumn(
                name: "TotalPaymentCount",
                table: "BlockchainProposal");
        }
    }
}
