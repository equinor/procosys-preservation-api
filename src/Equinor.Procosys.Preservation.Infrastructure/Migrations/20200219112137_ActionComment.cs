using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class ActionComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionComment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Schema = table.Column<string>(maxLength: 255, nullable: false),
                    Comment = table.Column<string>(maxLength: 4096, nullable: false),
                    CommentedAtUtc = table.Column<DateTime>(nullable: false),
                    CommentedById = table.Column<int>(nullable: false),
                    ActionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionComment_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActionComment_Persons_CommentedById",
                        column: x => x.CommentedById,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionComment_ActionId",
                table: "ActionComment",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionComment_CommentedById",
                table: "ActionComment",
                column: "CommentedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionComment");
        }
    }
}
