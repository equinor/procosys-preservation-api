using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class CascadeModeDeleteOnAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Actions_ActionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tags_TagId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Actions_ActionId",
                table: "Attachments",
                column: "ActionId",
                principalTable: "Actions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tags_TagId",
                table: "Attachments",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Actions_ActionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tags_TagId",
                table: "Attachments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Actions_ActionId",
                table: "Attachments",
                column: "ActionId",
                principalTable: "Actions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tags_TagId",
                table: "Attachments",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
