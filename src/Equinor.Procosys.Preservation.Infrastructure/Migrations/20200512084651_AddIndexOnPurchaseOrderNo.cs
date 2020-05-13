using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddIndexOnPurchaseOrderNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "RequirementDefinitions",
                newName: "IX_RequirementDefinitions_Plant_ASC");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseOrderNo",
                table: "Tags",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_RequirementDefinitions_Plant_ASC",
                table: "RequirementDefinitions",
                newName: "IX_PreservationPeriods_Plant_ASC");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseOrderNo",
                table: "Tags",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
