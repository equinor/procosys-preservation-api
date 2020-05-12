using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class AddIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PurchaseOrderNo",
                table: "Tags",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "McPkgNo",
                table: "Tags",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommPkgNo",
                table: "Tags",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Calloff",
                table: "Tags",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Calloff_ASC",
                table: "Tags",
                column: "Calloff")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CommPkgNo_ASC",
                table: "Tags",
                column: "CommPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_McPkgNo_ASC",
                table: "Tags",
                column: "McPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CommPkgNo", "CreatedAtUtc", "DisciplineCode", "IsVoided", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Plant_ASC",
                table: "Tags",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "TagNo" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags",
                column: "PurchaseOrderNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "Status", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags",
                column: "StorageArea")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags",
                column: "TagNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "Status", "StorageArea", "TagFunctionCode", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctions_Plant_ASC",
                table: "TagFunctions",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Code", "RegisterCode" });

            migrationBuilder.CreateIndex(
                name: "IX_TagFunctionRequirements_Plant_ASC",
                table: "TagFunctionRequirements",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Steps_Plant_ASC",
                table: "Steps",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "SortKey", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "RequirementDefinitions",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "IsVoided", "CreatedAtUtc", "ModifiedAtUtc", "SortKey", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name_ASC",
                table: "Projects",
                column: "Name")
                .Annotation("SqlServer:Include", new[] { "Plant" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Plant_ASC",
                table: "Projects",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Name", "IsClosed", "CreatedAtUtc", "ModifiedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "PreservationPeriods",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "Comment", "CreatedAtUtc", "DueTimeUtc", "ModifiedAtUtc", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Modes_Plant_ASC",
                table: "Modes",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_Plant_ASC",
                table: "Journeys",
                column: "Plant")
                .Annotation("SqlServer:Include", new[] { "CreatedAtUtc", "IsVoided", "ModifiedAtUtc", "Title" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Calloff_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CommPkgNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_McPkgNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Plant_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_TagFunctions_Plant_ASC",
                table: "TagFunctions");

            migrationBuilder.DropIndex(
                name: "IX_TagFunctionRequirements_Plant_ASC",
                table: "TagFunctionRequirements");

            migrationBuilder.DropIndex(
                name: "IX_Steps_Plant_ASC",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Responsibles_Plant_ASC",
                table: "Responsibles");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "RequirementDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Name_ASC",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Plant_ASC",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_PreservationPeriods_Plant_ASC",
                table: "PreservationPeriods");

            migrationBuilder.DropIndex(
                name: "IX_Modes_Plant_ASC",
                table: "Modes");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_Plant_ASC",
                table: "Journeys");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseOrderNo",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "McPkgNo",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommPkgNo",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Calloff",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
