using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemoveHistoryPreservationRecordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History");

            migrationBuilder.DropIndex(
                name: "IX_History_PreservationRecordId",
                table: "History");

            migrationBuilder.DropColumn(
                name: "PreservationRecordId",
                table: "History");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreservationRecordId",
                table: "History",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_History_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_History_PreservationRecords_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId",
                principalTable: "PreservationRecords",
                principalColumn: "Id");
        }
    }
}
