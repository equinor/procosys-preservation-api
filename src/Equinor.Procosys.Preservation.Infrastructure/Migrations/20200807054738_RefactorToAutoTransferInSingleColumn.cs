using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RefactorToAutoTransferInSingleColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferOnRfccSign",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "TransferOnRfocSign",
                table: "Steps");

            migrationBuilder.AddColumn<string>(
                name: "AutoTransferMethod",
                table: "Steps",
                nullable: false,
                defaultValue: "None");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_step_check_valid_auto_transfer",
                table: "Steps",
                sql: "AutoTransferMethod in ('None','OnRfccSign','OnRfocSign')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "constraint_step_check_valid_auto_transfer",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "AutoTransferMethod",
                table: "Steps");

            migrationBuilder.AddColumn<bool>(
                name: "TransferOnRfccSign",
                table: "Steps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TransferOnRfocSign",
                table: "Steps",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
