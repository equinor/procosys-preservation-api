using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class IsVoided : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Tags",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Step",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Responsibles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Modes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Journeys",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Step");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Responsibles");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Modes");

            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Journeys");
        }
    }
}
