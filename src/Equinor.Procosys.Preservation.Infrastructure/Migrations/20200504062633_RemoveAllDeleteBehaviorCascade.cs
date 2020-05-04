using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemoveAllDeleteBehaviorCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Tags_TagId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_PreservationPeriods_PreservationPeriodId",
                table: "FieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Modes_ModeId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Responsibles_ResponsibleId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagFunctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                table: "TagFunctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRequirements_Tags_TagId",
                table: "TagRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Tags_TagId",
                table: "Actions",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_PreservationPeriods_PreservationPeriodId",
                table: "FieldValues",
                column: "PreservationPeriodId",
                principalTable: "PreservationPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods",
                column: "TagRequirementId",
                principalTable: "TagRequirements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId",
                principalTable: "RequirementTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Modes_ModeId",
                table: "Steps",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Responsibles_ResponsibleId",
                table: "Steps",
                column: "ResponsibleId",
                principalTable: "Responsibles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagFunctionRequirements",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                table: "TagFunctionRequirements",
                column: "TagFunctionId",
                principalTable: "TagFunctions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagRequirements",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagRequirements_Tags_TagId",
                table: "TagRequirements",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags",
                column: "StepId",
                principalTable: "Steps",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Tags_TagId",
                table: "Actions");

            migrationBuilder.DropForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_PreservationPeriods_PreservationPeriodId",
                table: "FieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Modes_ModeId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Responsibles_ResponsibleId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagFunctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                table: "TagFunctionRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRequirements_Tags_TagId",
                table: "TagRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Tags_TagId",
                table: "Actions",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_PreservationPeriods_PreservationPeriodId",
                table: "FieldValues",
                column: "PreservationPeriodId",
                principalTable: "PreservationPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods",
                column: "TagRequirementId",
                principalTable: "TagRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId",
                principalTable: "RequirementTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Modes_ModeId",
                table: "Steps",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Responsibles_ResponsibleId",
                table: "Steps",
                column: "ResponsibleId",
                principalTable: "Responsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagFunctionRequirements",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                table: "TagFunctionRequirements",
                column: "TagFunctionId",
                principalTable: "TagFunctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagRequirements_RequirementDefinitions_RequirementDefinitionId",
                table: "TagRequirements",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagRequirements_Tags_TagId",
                table: "TagRequirements",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags",
                column: "StepId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
