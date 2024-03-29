﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class removeTagDeletedEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.AddCheckConstraint(
                // keep new contraint name created by EF 6.0: https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/breaking-changes#unique-check-constraints
                name: "CK_History_constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed','Rescheduled','UndoPreservationStarted')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_History_constraint_history_check_valid_event_type",
                table: "History");

            migrationBuilder.AddCheckConstraint(
                name: "constraint_history_check_valid_event_type",
                table: "History",
                sql: "EventType in ('RequirementAdded','RequirementDeleted','RequirementVoided','RequirementUnvoided','RequirementPreserved','TagVoided','TagUnvoided','TagCreated','TagDeleted','PreservationStarted','PreservationCompleted','IntervalChanged','JourneyChanged','StepChanged','TransferredManually','TransferredAutomatically','ActionAdded','ActionClosed','Rescheduled','UndoPreservationStarted')");
        }
    }
}
