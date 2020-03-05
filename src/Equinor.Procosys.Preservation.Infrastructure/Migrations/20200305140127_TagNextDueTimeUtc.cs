using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class TagNextDueTimeUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueTimeUtc",
                table: "Tags",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextDueTimeUtc",
                table: "Tags");
        }
    }
}
