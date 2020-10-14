using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemovedUnusedSavedFilterIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedFilters_Plant_ProjectId_PersonId",
                table: "SavedFilters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_Plant_ProjectId_PersonId",
                table: "SavedFilters",
                columns: new[] { "Plant", "ProjectId", "PersonId" });
        }
    }
}
