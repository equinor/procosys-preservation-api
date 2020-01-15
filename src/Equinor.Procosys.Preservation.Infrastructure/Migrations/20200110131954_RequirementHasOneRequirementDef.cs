using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RequirementHasOneRequirementDef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Step_ResponsibleId",
                table: "Step",
                column: "ResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_RequirementDefinitionId",
                table: "Requirements",
                column: "RequirementDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requirements_RequirementDefinitions_RequirementDefinitionId",
                table: "Requirements",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Responsibles_ResponsibleId",
                table: "Step",
                column: "ResponsibleId",
                principalTable: "Responsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requirements_RequirementDefinitions_RequirementDefinitionId",
                table: "Requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Step_Responsibles_ResponsibleId",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Step_ResponsibleId",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Requirements_RequirementDefinitionId",
                table: "Requirements");
        }
    }
}
