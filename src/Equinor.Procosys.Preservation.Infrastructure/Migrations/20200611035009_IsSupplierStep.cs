using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class IsSupplierStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSupplierStep",
                table: "Steps",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"update Steps set IsSupplierStep = 1 where ModeId in (select Id from Modes where ForSupplier = 1);");

            migrationBuilder.AddColumn<bool>(
                name: "IsInSupplierStep",
                table: "Tags",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"update Tags set IsInSupplierStep = 1 where StepId in (select Id from Steps where IsSupplierStep = 1);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInSupplierStep",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsSupplierStep",
                table: "Steps");
        }
    }
}
