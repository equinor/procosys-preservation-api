using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class RemoveTagStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags");

            migrationBuilder.DropCheckConstraint(
                name: "constraint_tag_check_valid_status",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tags");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Calloff_ASC",
                table: "Tags",
                column: "Calloff")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "StatusEnum", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CommPkgNo_ASC",
                table: "Tags",
                column: "CommPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "StatusEnum", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_McPkgNo_ASC",
                table: "Tags",
                column: "McPkgNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "Description", "CommPkgNo", "CreatedAtUtc", "DisciplineCode", "IsVoided", "NextDueTimeUtc", "PurchaseOrderNo", "StatusEnum", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags",
                column: "PurchaseOrderNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "StatusEnum", "StorageArea", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags",
                column: "StorageArea")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "CreatedAtUtc", "Description", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "StatusEnum", "TagFunctionCode", "TagNo", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags",
                column: "TagNo")
                .Annotation("SqlServer:Include", new[] { "AreaCode", "Calloff", "CommPkgNo", "Description", "CreatedAtUtc", "DisciplineCode", "IsVoided", "McPkgNo", "NextDueTimeUtc", "PurchaseOrderNo", "StatusEnum", "StorageArea", "TagFunctionCode", "TagType" });
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
                name: "IX_Tags_PurchaseOrderNo_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_StorageArea_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TagNo_ASC",
                table: "Tags");

            migrationBuilder.CreateCheckConstraint(
                name: "constraint_tag_check_valid_status",
                table: "Tags",
                sql: "Status in ('NotStarted','Active','Completed')");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NotStarted");

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
        }
    }
}
