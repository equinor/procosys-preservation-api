using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenamePreservedByPersonId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedByPersonId",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_PreservedByPersonId",
                table: "PreservationRecords");

            migrationBuilder.RenameColumn(
                name: "PreservedByPersonId",
                table: "PreservationRecords",
                newName: "PreservedById");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_PreservedById",
                table: "PreservationRecords",
                column: "PreservedById");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedById",
                table: "PreservationRecords",
                column: "PreservedById",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedById",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_PreservedById",
                table: "PreservationRecords");

            migrationBuilder.RenameColumn(
                name: "PreservedById",
                table: "PreservationRecords",
                newName: "PreservedByPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_PreservedByPersonId",
                table: "PreservationRecords",
                column: "PreservedByPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedByPersonId",
                table: "PreservationRecords",
                column: "PreservedByPersonId",
                principalTable: "Persons",
                principalColumn: "Id");
        }
    }
}
