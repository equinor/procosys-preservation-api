using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddPreservationRecordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History");

            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','PreserveRequirement','VoidTag','UnvoidTag','CreateTag','DeleteTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer','AddAction','CloseAction')");

            migrationBuilder.AddForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId",
                principalTable: "PreservationRecords",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History");

            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','PreserveRequirement','VoidTag','UnvoidTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer')");

            migrationBuilder.AddForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId",
                principalTable: "PreservationRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
