using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class AddUniqueConstraintOnTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tags_Plant_TagNo_ProjectId",
                table: "Tags",
                columns: new[] { "Plant", "TagNo", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Plant_TagNo_ProjectId",
                table: "Tags");
        }
    }
}
