using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class ActionCreatedInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Actions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Actions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Actions_CreatedById",
                table: "Actions",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Persons_CreatedById",
                table: "Actions",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Persons_CreatedById",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_CreatedById",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Actions");
        }
    }
}
