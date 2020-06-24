using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameDueInWeeksAndUpdatePresRecGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueWeeks",
                table: "History");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreservationRecordGuid",
                table: "History",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "DueInWeeks",
                table: "History",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueInWeeks",
                table: "History");

            migrationBuilder.AlterColumn<Guid>(
                name: "PreservationRecordGuid",
                table: "History",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DueWeeks",
                table: "History",
                type: "int",
                nullable: true);
        }
    }
}
