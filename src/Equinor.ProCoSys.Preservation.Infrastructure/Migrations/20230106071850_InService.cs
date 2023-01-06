using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class InService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags");

            migrationBuilder.DropCheckConstraint(
                name: "CK_History_constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags",
                sql: "Status in (0,1,2,3)");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed','Rescheduled','UndoPreservationStarted','TagVoidedInSource','TagUnvoidedInSource','TagDeletedInSource','PreservationSetInService')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags");

            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_tag_check_valid_statusenum",
                table: "Tags",
                sql: "Status in (0,1,2)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_History_constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed','Rescheduled','UndoPreservationStarted','TagVoidedInSource','TagUnvoidedInSource','TagDeletedInSource')");
        }
    }
}
