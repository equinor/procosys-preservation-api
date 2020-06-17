using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class MigrateTagObjectGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Update Tags Set ObjectGuid = NEWID() where ObjectGuid = 'D445579D-E8CA-49F4-80D7-59452EBD8D47'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
