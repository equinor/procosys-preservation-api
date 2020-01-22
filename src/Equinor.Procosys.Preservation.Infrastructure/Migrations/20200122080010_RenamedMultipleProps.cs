using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenamedMultipleProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalloffNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CommPkgNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "McPkcNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Calloff",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommPkgNo",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "McPkgNo",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Tags",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNo",
                table: "Tags",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calloff",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CommPkgNo",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "McPkgNo",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNo",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "CalloffNumber",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommPkgNumber",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "McPkcNumber",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "Tags",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
