using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    public partial class UniqueContraintInTagFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagFunctions_Plant_ASC",
                table: "TagFunctions");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_Plant_Code_RegisterCode",
                table: "TagFunctions",
                columns: new[] { "Plant", "Code", "RegisterCode" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagFunctions_Plant_Code_RegisterCode",
                table: "TagFunctions");

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_Plant_ASC",
                table: "TagFunctions",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Code", "RegisterCode" });
        }
    }
}
