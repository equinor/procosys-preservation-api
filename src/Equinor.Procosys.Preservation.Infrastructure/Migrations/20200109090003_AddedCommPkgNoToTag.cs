using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddedCommPkgNoToTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectNo",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagNo",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "AreaCode",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalloffNumber",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommPkgNumber",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisciplineCode",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "McPkcNumber",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueTime",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "Tags",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagFunctionCode",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagNumber",
                table: "Tags",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaCode",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CalloffNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CommPkgNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DisciplineCode",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "McPkcNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "NextDueTime",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagFunctionCode",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagNumber",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "ProjectNo",
                table: "Tags",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagNo",
                table: "Tags",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
