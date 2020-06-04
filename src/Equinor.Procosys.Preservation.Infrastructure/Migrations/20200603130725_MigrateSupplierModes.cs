using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class MigrateSupplierModes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update Modes set ForSupplier = 1 where Title = 'SUPPLIER';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
