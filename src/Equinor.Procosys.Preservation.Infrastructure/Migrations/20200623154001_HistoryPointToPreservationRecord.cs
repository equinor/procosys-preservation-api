using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class HistoryPointToPreservationRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ObjectGuid",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<int>(
                name: "DueWeeks",
                table: "History",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreservationRecordGuid",
                table: "History",
                nullable: true,
                defaultValue: null);

            migrationBuilder.Sql("Update PreservationRecords Set ObjectGuid = NEWID() where ObjectGuid = '00000000-0000-0000-0000-000000000000'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectGuid",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "DueWeeks",
                table: "History");

            migrationBuilder.DropColumn(
                name: "PreservationRecordGuid",
                table: "History");
        }
    }
}
