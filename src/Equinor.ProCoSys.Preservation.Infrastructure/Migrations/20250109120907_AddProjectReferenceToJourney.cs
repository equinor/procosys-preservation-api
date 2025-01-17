using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectReferenceToJourney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Journeys",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FieldType",
                table: "FieldValues",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentType",
                table: "Attachments",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Journeys_ProjectId",
                table: "Journeys",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Journeys_Projects_ProjectId",
                table: "Journeys",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Journeys_Projects_ProjectId",
                table: "Journeys");

            migrationBuilder.DropIndex(
                name: "IX_Journeys_ProjectId",
                table: "Journeys");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Journeys");

            migrationBuilder.AlterColumn<string>(
                name: "FieldType",
                table: "FieldValues",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentType",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
