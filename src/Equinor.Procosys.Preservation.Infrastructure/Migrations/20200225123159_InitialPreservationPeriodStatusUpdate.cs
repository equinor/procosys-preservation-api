using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class InitialPreservationPeriodStatusUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.Sql(@"update Requirements
                        set InitialPreservationPeriodStatus = 'ReadyToBePreserved'
                        where Requirements.RequirementDefinitionId not in (
                            select rd.Id from Fields f
                            join RequirementDefinitions rd on rd.Id = f.RequirementDefinitionId
                            where f.FieldType in ('CheckBox','Number')
                        )");

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
