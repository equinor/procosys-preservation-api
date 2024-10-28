using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTagHistoryAddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_History_Plant_ASC",
                table: "History",
                column: "Plant");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Guid_ASC",
                table: "Tags",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_PreservationRecords_Plant_ASC",
                table: "PreservationRecords",
                column: "Plant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PreservationRecords_Plant_ASC",
                table: "PreservationRecords");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Guid_ASC",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_History_Plant_ASC",
                table: "History");
        }
    }
}
