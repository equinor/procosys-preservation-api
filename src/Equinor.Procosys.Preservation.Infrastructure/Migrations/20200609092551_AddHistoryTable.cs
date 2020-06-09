using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 1048, nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    PreservationRecordId = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryEntries", x => x.Id);
                    table.CheckConstraint("constraint_history_check_valid_event_type", "EventType in ('AddRequirement','DeleteRequirement','VoidRequirement','UnvoidRequirement','VoidTag','UnvoidTag','StartPreservation','CompletePreservation','ChangeInterval','ManualTransfer','AutomaticTransfer')");
                    table.ForeignKey(
                        name: "FK_HistoryEntries_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoryEntries_PreservationRecords_PreservationRecordId",
                        column: x => x.PreservationRecordId,
                        principalTable: "PreservationRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoryEntries_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEntries_CreatedById",
                table: "HistoryEntries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEntries_PreservationRecordId",
                table: "HistoryEntries",
                column: "PreservationRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryEntries_TagId",
                table: "HistoryEntries",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryEntries");
        }
    }
}
