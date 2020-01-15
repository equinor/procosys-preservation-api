using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class TagStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tags",
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_tag_check_valid_status",
                table: "Tags",
                sql: "Status in ('NotStarted','Active','Completed')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_status",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tags");
        }
    }
}
