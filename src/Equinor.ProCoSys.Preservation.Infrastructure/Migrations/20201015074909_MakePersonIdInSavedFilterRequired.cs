using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class MakePersonIdInSavedFilterRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFilters_Persons_PersonId",
                table: "SavedFilters");

            migrationBuilder.AlterColumn<int>(
                name: "PersonId",
                table: "SavedFilters",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFilters_Persons_PersonId",
                table: "SavedFilters",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFilters_Persons_PersonId",
                table: "SavedFilters");

            migrationBuilder.AlterColumn<int>(
                name: "PersonId",
                table: "SavedFilters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFilters_Persons_PersonId",
                table: "SavedFilters",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
