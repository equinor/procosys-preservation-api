using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RenameResposibleTitleToDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Responsibles",
                newName: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Description" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Responsibles",
                newName: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });
        }
    }
}
