using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddColumnTagStatusEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusEnum",
                table: "Tags",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags",
                sql: "StatusEnum in (0,1,2)");

            migrationBuilder.Sql("update Tags set StatusEnum=0 where Status='NotStarted'");
            migrationBuilder.Sql("update Tags set StatusEnum=1 where Status='Active'");
            migrationBuilder.Sql("update Tags set StatusEnum=2 where Status='Completed'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "StatusEnum",
                table: "Tags");
        }
    }
}
