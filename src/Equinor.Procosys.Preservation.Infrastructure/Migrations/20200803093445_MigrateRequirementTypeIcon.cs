using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class MigrateRequirementTypeIcon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Area' where Code = 'Area'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Battery' where Code = 'Charging'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Bearings' where Code = 'Grease'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Heating' where Code = 'Heating'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Electrical' where Code = 'IR Test'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Nitrogen' where Code = 'Nitrogen'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Pressure' where Code = 'Oil Level'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Power' where Code = 'Powered'");
            migrationBuilder.Sql("update RequirementTypes set Icon = 'Rotate' where Code = 'Rotation'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
