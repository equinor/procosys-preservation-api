using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class CreatedAndModifiedConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Step_CreatedById",
                table: "Step",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Step_ModifiedById",
                table: "Step",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_CreatedById",
                table: "Responsibles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_ModifiedById",
                table: "Responsibles",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_CreatedById",
                table: "RequirementTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementTypes_ModifiedById",
                table: "RequirementTypes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CreatedById",
                table: "Requirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ModifiedById",
                table: "Requirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_CreatedById",
                table: "RequirementDefinitions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_ModifiedById",
                table: "RequirementDefinitions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ModifiedById",
                table: "Projects",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_CreatedById",
                table: "PreservationRecords",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_ModifiedById",
                table: "PreservationRecords",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_CreatedById",
                table: "PreservationPeriods",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_ModifiedById",
                table: "PreservationPeriods",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CreatedById",
                table: "Persons",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ModifiedById",
                table: "Persons",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_CreatedById",
                table: "Modes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Modes_ModifiedById",
                table: "Modes",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_CreatedById",
                table: "Journeys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_ModifiedById",
                table: "Journeys",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_CreatedById",
                table: "FieldValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_ModifiedById",
                table: "FieldValues",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_CreatedById",
                table: "Fields",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_ModifiedById",
                table: "Fields",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ModifiedById",
                table: "Actions",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Persons_ModifiedById",
                table: "Actions",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Persons_CreatedById",
                table: "Fields",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Persons_ModifiedById",
                table: "Fields",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_Persons_CreatedById",
                table: "FieldValues",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_Persons_ModifiedById",
                table: "FieldValues",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Persons_CreatedById",
                table: "Journeys",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Persons_ModifiedById",
                table: "Journeys",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_Persons_CreatedById",
                table: "Modes",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Modes_Persons_ModifiedById",
                table: "Modes",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Persons_CreatedById",
                table: "Persons",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Persons_ModifiedById",
                table: "Persons",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_Persons_CreatedById",
                table: "PreservationPeriods",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_Persons_ModifiedById",
                table: "PreservationPeriods",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_CreatedById",
                table: "PreservationRecords",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_ModifiedById",
                table: "PreservationRecords",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Persons_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Persons_ModifiedById",
                table: "Projects",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_Persons_CreatedById",
                table: "RequirementDefinitions",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_Persons_ModifiedById",
                table: "RequirementDefinitions",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Persons_CreatedById",
                table: "Requirements",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_Persons_ModifiedById",
                table: "Requirements",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTypes_Persons_CreatedById",
                table: "RequirementTypes",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementTypes_Persons_ModifiedById",
                table: "RequirementTypes",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibles_Persons_CreatedById",
                table: "Responsibles",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibles_Persons_ModifiedById",
                table: "Responsibles",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Persons_CreatedById",
                table: "Step",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Persons_ModifiedById",
                table: "Step",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Persons_CreatedById",
                table: "Tags",
                column: "CreatedById",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Persons_ModifiedById",
                table: "Tags",
                column: "ModifiedById",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Persons_ModifiedById",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Persons_CreatedById",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Persons_ModifiedById",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_Persons_CreatedById",
                table: "FieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_Persons_ModifiedById",
                table: "FieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Persons_CreatedById",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Persons_ModifiedById",
                table: "Journeys");

            migrationBuilder.DropForeignKey(
                name: "FK_Modes_Persons_CreatedById",
                table: "Modes");

            migrationBuilder.DropForeignKey(
                name: "FK_Modes_Persons_ModifiedById",
                table: "Modes");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Persons_CreatedById",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Persons_ModifiedById",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_Persons_CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_Persons_ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Persons_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Persons_ModifiedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_Persons_CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_Persons_ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Persons_CreatedById",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_Persons_ModifiedById",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTypes_Persons_CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementTypes_Persons_ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsibles_Persons_CreatedById",
                table: "Responsibles");

            migrationBuilder.DropForeignKey(
                name: "FK_Responsibles_Persons_ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Persons_CreatedById",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Persons_ModifiedById",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Persons_CreatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Persons_ModifiedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ModifiedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Step_CreatedById",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Step_ModifiedById",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Responsibles_CreatedById",
                table: "Responsibles");

            migrationBuilder.DropIndex(
                name: "IX_Responsibles_ModifiedById",
                table: "Responsibles");

            migrationBuilder.DropIndex(
                name: "IX_RequirementTypes_CreatedById",
                table: "RequirementTypes");

            migrationBuilder.DropIndex(
                name: "IX_RequirementTypes_ModifiedById",
                table: "RequirementTypes");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_CreatedById",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_ModifiedById",
                table: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_RequirementDefinitions_CreatedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_RequirementDefinitions_ModifiedById",
                table: "RequirementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ModifiedById",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_CreatedById",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_ModifiedById",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_CreatedById",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_ModifiedById",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Persons_CreatedById",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_ModifiedById",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Modes_CreatedById",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Modes_ModifiedById",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_CreatedById",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_ModifiedById",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_FieldValues_CreatedById",
                table: "FieldValues");

            migrationBuilder.DropIndex(
                name: "IX_FieldValues_ModifiedById",
                table: "FieldValues");

            migrationBuilder.DropIndex(
                name: "IX_Fields_CreatedById",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Fields_ModifiedById",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Actions_ModifiedById",
                table: "Actions");
        }
    }
}
