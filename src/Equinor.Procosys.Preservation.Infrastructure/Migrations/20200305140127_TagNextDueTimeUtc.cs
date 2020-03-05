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

            migrationBuilder.Sql(@"update Tags set NextDueTimeUtc=
                   (select top 1 r.NextDueTimeUtc 
	                from Requirements r 
	                where r.TagId=Tags.Id 
	                order by r.NextDueTimeUtc)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextDueTimeUtc",
                table: "Tags");
        }
    }
}
