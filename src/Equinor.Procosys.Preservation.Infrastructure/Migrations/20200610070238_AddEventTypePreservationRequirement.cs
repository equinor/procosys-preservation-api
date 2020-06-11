using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddEventTypePreservationRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','PreserveRequirement','VoidTag','UnvoidTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','VoidTag','UnvoidTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer')");
        }
    }
}
