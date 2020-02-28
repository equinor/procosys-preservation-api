using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Step_Journeys_JourneyId",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Modes_ModeId",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Responsibles_ResponsibleId",
                table: "Step");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Step_StepId",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Step",
                table: "Step");

            migrationBuilder.RenameTable(
                name: "Step",
                newName: "Steps");

            migrationBuilder.RenameIndex(
                name: "IX_Step_ResponsibleId",
                table: "Steps",
                newName: "IX_Steps_ResponsibleId");

            migrationBuilder.RenameIndex(
                name: "IX_Step_ModeId",
                table: "Steps",
                newName: "IX_Steps_ModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Step_JourneyId",
                table: "Steps",
                newName: "IX_Steps_JourneyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Steps",
                table: "Steps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Tags_Steps_StepId",
                table: "Tags",
                column: "StepId",
                principalTable: "Steps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Modes_ModeId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Responsibles_ResponsibleId",
                table: "Steps");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Steps",
                table: "Steps");

            migrationBuilder.RenameTable(
                name: "Steps",
                newName: "Step");

            migrationBuilder.RenameIndex(
                name: "IX_Steps_ResponsibleId",
                table: "Step",
                newName: "IX_Step_ResponsibleId");

            migrationBuilder.RenameIndex(
                name: "IX_Steps_ModeId",
                table: "Step",
                newName: "IX_Step_ModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Steps_JourneyId",
                table: "Step",
                newName: "IX_Step_JourneyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Step",
                table: "Step",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Journeys_JourneyId",
                table: "Step",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Modes_ModeId",
                table: "Step",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Responsibles_ResponsibleId",
                table: "Step",
                column: "ResponsibleId",
                principalTable: "Responsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Step_StepId",
                table: "Tags",
                column: "StepId",
                principalTable: "Step",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
