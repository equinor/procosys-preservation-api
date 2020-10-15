using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RescheduledEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','TagDeleted','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed','Rescheduled')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','TagDeleted','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed')");
        }
    }
}
