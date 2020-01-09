using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Schema",
                table: "RequirementTypes",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RequirementDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Title = table.Column<string>(maxLength: 64, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    DefaultInterval = table.Column<int>(nullable: false),
                    SortKey = table.Column<int>(nullable: false),
                    RequirementTypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequirementDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequirementDefinitions_RequirementTypes_RequirementTypeId",
                        column: x => x.RequirementTypeId,
                        principalTable: "RequirementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Label = table.Column<string>(maxLength: 255, nullable: false),
                    Unit = table.Column<string>(maxLength: 32, nullable: false),
                    IsVoided = table.Column<bool>(nullable: false),
                    ShowPrevious = table.Column<bool>(nullable: false),
                    SortKey = table.Column<int>(nullable: false),
                    RequirementDefinitionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_RequirementDefinitions_RequirementDefinitionId",
                        column: x => x.RequirementDefinitionId,
                        principalTable: "RequirementDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fields_RequirementDefinitionId",
                table: "Fields",
                column: "RequirementDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RequirementDefinitions_RequirementTypeId",
                table: "RequirementDefinitions",
                column: "RequirementTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "RequirementDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "Schema",
                table: "RequirementTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }
    }
}
