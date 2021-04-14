using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class AddProjectForeignKeyToSavedFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "SavedFilters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_ProjectId",
                table: "SavedFilters",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedFilters_Projects_ProjectId",
                table: "SavedFilters",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedFilters_Projects_ProjectId",
                table: "SavedFilters");

            migrationBuilder.DropIndex(
                name: "IX_SavedFilters_ProjectId",
                table: "SavedFilters");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SavedFilters");
        }
    }
}
