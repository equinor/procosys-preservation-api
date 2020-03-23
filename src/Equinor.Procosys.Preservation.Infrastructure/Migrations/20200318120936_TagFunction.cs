using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class TagFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TagFunctionCode",
                table: "Tags",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TagFunctions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Code = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    RegisterCode = table.Column<string>(maxLength: 255, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagFunctions_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctions_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TagFunctionRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    IntervalWeeks = table.Column<int>(nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    RequirementDefinitionId = table.Column<int>(nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    TagFunctionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFunctionRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_Persons_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_Persons_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_RequirementDefinitions_RequirementDefinitionId",
                        column: x => x.RequirementDefinitionId,
                        principalTable: "RequirementDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagFunctionRequirements_TagFunctions_TagFunctionId",
                        column: x => x.TagFunctionId,
                        principalTable: "TagFunctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_CreatedById",
                table: "TagFunctionRequirements",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_ModifiedById",
                table: "TagFunctionRequirements",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_RequirementDefinitionId",
                table: "TagFunctionRequirements",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_TagFunctionId",
                table: "TagFunctionRequirements",
                column: "TagFunctionId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_CreatedById",
                table: "TagFunctions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_ModifiedById",
                table: "TagFunctions",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagFunctionRequirements");

            migrationBuilder.DropTable(
                name: "TagFunctions");

            migrationBuilder.AlterColumn<string>(
                name: "TagFunctionCode",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
