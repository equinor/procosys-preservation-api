using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddObjectGuidToTagAndHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_History_ObjectId_ASC",
                table: "History");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "History");

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectGuid",
                table: "Tags",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectGuid",
                table: "History",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_History_ObjectGuid_ASC",
                table: "History",
                column: "ObjectGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_History_ObjectGuid_ASC",
                table: "History");

            migrationBuilder.DropColumn(
                name: "ObjectGuid",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ObjectGuid",
                table: "History");

            migrationBuilder.AddColumn<int>(
                name: "ObjectId",
                table: "History",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_History_ObjectId_ASC",
                table: "History",
                column: "ObjectId");
        }
    }
}
