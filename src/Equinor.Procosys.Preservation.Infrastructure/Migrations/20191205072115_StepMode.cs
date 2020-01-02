using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class StepMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Step_ModeId",
                table: "Step",
                column: "ModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Step_Modes_ModeId",
                table: "Step",
                column: "ModeId",
                principalTable: "Modes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Step_Modes_ModeId",
                table: "Step");

            migrationBuilder.DropIndex(
                name: "IX_Step_ModeId",
                table: "Step");
        }
    }
}
