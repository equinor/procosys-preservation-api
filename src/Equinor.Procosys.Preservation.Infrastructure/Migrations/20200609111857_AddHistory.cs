using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    ObjectType = table.Column<int>(nullable: false),
                    PreservationRecordId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.CheckConstraint("constraint_history_check_valid_event_type", "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','VoidTag','UnvoidTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer')");
                    table.ForeignKey(
                        name: "FK_History_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_History_PreservationRecords_PreservationRecordId",
                        column: x => x.PreservationRecordId,
                        principalTable: "PreservationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_History_CreatedById",
                table: "History",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_History_PreservationRecordId",
                table: "History",
                column: "PreservationRecordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "History");
        }
    }
}
