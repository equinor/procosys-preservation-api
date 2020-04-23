using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameToTagRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_Requirements_RequirementId",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_RequirementId",
                table: "PreservationPeriods");

            migrationBuilder.RenameTable(
                name: "Requirements",
                newName: "TagRequirements");

            migrationBuilder.AddColumn<int>(
                name: "TagRequirementId",
                table: "PreservationPeriods",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"update PreservationPeriods set TagRequirementId = RequirementId;");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "PreservationPeriods");
            
            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_TagRequirementId",
                table: "PreservationPeriods",
                column: "TagRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_CreatedById",
                table: "TagRequirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_ModifiedById",
                table: "TagRequirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_RequirementDefinitionId",
                table: "TagRequirements",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRequirements_TagId",
                table: "TagRequirements",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods",
                column: "TagRequirementId",
                principalTable: "TagRequirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationPeriods_TagRequirements_TagRequirementId",
                table: "PreservationPeriods");

            migrationBuilder.RenameTable(
                name: "TagRequirements",
                newName: "Requirements");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_TagRequirementId",
                table: "PreservationPeriods");

            migrationBuilder.AddColumn<int>(
                name: "RequirementId",
                table: "PreservationPeriods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"update PreservationPeriods set RequirementId = TagRequirementId;");

            migrationBuilder.DropColumn(
                name: "TagRequirementId",
                table: "PreservationPeriods");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_RequirementId",
                table: "PreservationPeriods",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CreatedById",
                table: "Requirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_ModifiedById",
                table: "Requirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_RequirementDefinitionId",
                table: "Requirements",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_TagId",
                table: "Requirements",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationPeriods_Requirements_RequirementId",
                table: "PreservationPeriods",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
