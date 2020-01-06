using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RequiredForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions");

            migrationBuilder.AlterColumn<int>(
                name: "RequirementTypeId",
                table: "RequirementDefinitions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequirementDefinitionId",
                table: "Fields",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId",
                principalTable: "RequirementTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields");

            migrationBuilder.DropForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions");

            migrationBuilder.AlterColumn<int>(
                name: "RequirementTypeId",
                table: "RequirementDefinitions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RequirementDefinitionId",
                table: "Fields",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId",
                principalTable: "RequirementDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId",
                principalTable: "RequirementTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
