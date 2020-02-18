using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class Action : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 4096, nullable: false),
                    DueTimeUtc = table.Column<DateTime>(nullable: true),
                    ClearedAtUtc = table.Column<DateTime>(nullable: true),
                    ClearedById = table.Column<int>(nullable: true),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_Persons_ClearedById",
                        column: x => x.ClearedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actions_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ClearedById",
                table: "Actions",
                column: "ClearedById");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_TagId",
                table: "Actions",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");
        }
    }
}
