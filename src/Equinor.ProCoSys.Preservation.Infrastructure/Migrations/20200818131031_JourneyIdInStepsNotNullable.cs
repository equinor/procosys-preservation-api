using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class JourneyIdInStepsNotNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Steps where JourneyId is NULL");

            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps");

            migrationBuilder.AlterColumn<int>(
                name: "JourneyId",
                table: "Steps",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps");

            migrationBuilder.AlterColumn<int>(
                name: "JourneyId",
                table: "Steps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_Journeys_JourneyId",
                table: "Steps",
                column: "JourneyId",
                principalTable: "Journeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
