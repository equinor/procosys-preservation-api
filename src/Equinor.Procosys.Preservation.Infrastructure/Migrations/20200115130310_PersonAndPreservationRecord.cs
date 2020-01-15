using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class PersonAndPreservationRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Oid = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 64, nullable: false),
                    LastName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreservationRecords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    RequirementId = table.Column<int>(nullable: false),
                    NextDueTimeUtc = table.Column<DateTime>(nullable: false),
                    BulkPreserved = table.Column<bool>(nullable: true),
                    PreservedAtUtc = table.Column<DateTime>(nullable: true),
                    PreservedBy = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreservationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreservationRecords_Persons_PreservedBy",
                        column: x => x.PreservedBy,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PreservationRecords_Requirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "Requirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_PreservedBy",
                table: "PreservationRecords",
                column: "PreservedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_RequirementId",
                table: "PreservationRecords",
                column: "RequirementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreservationRecords");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
