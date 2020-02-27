using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class InitialPreservationPeriodStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "_initialPreservationPeriodStatus",
                table: "Requirements",
                maxLength: 64,
                nullable: false,
                defaultValue: "NeedsUserInput");

            migrationBuilder.Sql(@"update Requirements
                        set _initialPreservationPeriodStatus = 'ReadyToBePreserved'
                        where Requirements.RequirementDefinitionId not in (
                            select rd.Id from Fields f
                            join RequirementDefinitions rd on rd.Id = f.RequirementDefinitionId
                            where f.FieldType in ('CheckBox','Number')
                        )");

            migrationBuilder.Sql(@"update PreservationPeriods 
                        set Status='ReadyToBePreserved'
                        where id in (  
                            select pp.id from PreservationPeriods pp 
                            join Requirements r on pp.RequirementId=r.Id
                            where pp.Status = 'NeedsUserInput' and r._initialPreservationPeriodStatus='ReadyToBePreserved'
                        )");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements",
                sql: "_initialPreservationPeriodStatus in ('NeedsUserInput','ReadyToBePreserved')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_requirement_check_valid_initial_status",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "_initialPreservationPeriodStatus",
                table: "Requirements");
        }
    }
}
