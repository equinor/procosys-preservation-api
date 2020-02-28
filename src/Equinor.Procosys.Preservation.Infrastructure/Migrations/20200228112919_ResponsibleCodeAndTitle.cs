using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class ResponsibleCodeAndTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Responsibles",
                newName: "Code");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Responsibles",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Responsibles");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Responsibles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
