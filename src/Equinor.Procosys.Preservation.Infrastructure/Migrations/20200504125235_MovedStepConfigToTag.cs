using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class MovedStepConfigToTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Steps_StepId",
                table: "Tags");

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
                name: "FK_Tags_Steps_StepId",
                table: "Tags");

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
