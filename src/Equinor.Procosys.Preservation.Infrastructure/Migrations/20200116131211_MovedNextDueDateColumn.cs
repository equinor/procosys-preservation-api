using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class MovedNextDueDateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedBy",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_PreservedBy",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "NextDueTimeUtc",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "PreservedBy",
                table: "PreservationRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueTimeUtc",
                table: "Requirements",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PreservedAtUtc",
                table: "PreservationRecords",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "BulkPreserved",
                table: "PreservationRecords",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreservedByPersonId",
                table: "PreservationRecords",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedByPersonId",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_PreservedByPersonId",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "NextDueTimeUtc",
                table: "Requirements");

            migrationBuilder.DropColumn(
                name: "PreservedByPersonId",
                table: "PreservationRecords");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PreservedAtUtc",
                table: "PreservationRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<bool>(
                name: "BulkPreserved",
                table: "PreservationRecords",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<DateTime>(
                name: "NextDueTimeUtc",
                table: "PreservationRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PreservedBy",
                table: "PreservationRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_PreservedBy",
                table: "PreservationRecords",
                column: "PreservedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Persons_PreservedBy",
                table: "PreservationRecords",
                column: "PreservedBy",
                principalTable: "Persons",
                principalColumn: "Id");
        }
    }
}
