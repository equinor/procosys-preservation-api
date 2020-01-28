using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class PreservationPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationRecords_Requirements_RequirementId",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_RequirementId",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "PreservationRecords");

            migrationBuilder.DropColumn(
                name: "RequirementId",
                table: "PreservationRecords");

            migrationBuilder.CreateTable(
                name: "PreservationPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Status = table.Column<string>(nullable: false, defaultValue: "NeedUserInput"),
                    DueTimeUtc = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    PreservationRecordId = table.Column<int>(nullable: true),
                    RequirementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationPeriods", x => x.Id);
                    table.CheckConstraint("constraint_period_check_valid_status", "Status in ('NeedUserInput','ReadyToBePreserved','Preserved','Stopped')");
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_PreservationRecords_PreservationRecordId",
                        column: x => x.PreservationRecordId,
                        principalTable: "PreservationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreservationPeriods_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_PreservationRecordId",
                table: "PreservationPeriods",
                column: "PreservationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_RequirementId",
                table: "PreservationPeriods",
                column: "RequirementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreservationPeriods");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "PreservationRecords",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequirementId",
                table: "PreservationRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_RequirementId",
                table: "PreservationRecords",
                column: "RequirementId");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationRecords_Requirements_RequirementId",
                table: "PreservationRecords",
                column: "RequirementId",
                principalTable: "Requirements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
