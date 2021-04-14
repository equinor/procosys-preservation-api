using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class AddIndexesOnSavedFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SavedFilters_Plant_ProjectId_PersonId",
                table: "SavedFilters",
                columns: new[] { "Plant", "ProjectId", "PersonId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SavedFilters_Plant_ProjectId_PersonId",
                table: "SavedFilters");
        }
    }
}
