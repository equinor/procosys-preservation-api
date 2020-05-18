using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class FieldValueAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobId",
                table: "FieldValues");

            migrationBuilder.AddColumn<int>(
                name: "FieldValueAttachmentId",
                table: "FieldValues",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldValues_FieldValueAttachmentId",
                table: "FieldValues",
                column: "FieldValueAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldValues_Attachments_FieldValueAttachmentId",
                table: "FieldValues",
                column: "FieldValueAttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldValues_Attachments_FieldValueAttachmentId",
                table: "FieldValues");

            migrationBuilder.DropIndex(
                name: "IX_FieldValues_FieldValueAttachmentId",
                table: "FieldValues");

            migrationBuilder.DropColumn(
                name: "FieldValueAttachmentId",
                table: "FieldValues");

            migrationBuilder.AddColumn<string>(
                name: "BlobId",
                table: "FieldValues",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
