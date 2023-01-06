using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class NegativeInService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags",
                sql: "Status in (0,1,2,-1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags",
                sql: "Status in (0,1,2,3)");
        }
    }
}
