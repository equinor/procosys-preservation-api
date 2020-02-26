using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class PreservationPeriodStatusUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
            => migrationBuilder.Sql(@"update PreservationPeriods 
                        set Status='ReadyToBePreserved'
                        where id in (  
                            select pp.id from PreservationPeriods pp 
                            join Requirements r on pp.RequirementId=r.Id
                            where pp.Status = 'NeedsUserInput' and r.InitialPreservationPeriodStatus='ReadyToBePreserved'
                        )");

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
