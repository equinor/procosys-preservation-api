using Microsoft.EntityFrameworkCore.Migrations;

namespace Equinor.Procosys.Preservation.Infrastructure.Migrations
{
    public partial class OhmSignAsUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update Fields set Unit = NCHAR(937) where FieldType='Number' and Label in ('L1/L2','L2/L3','L1/L3')");
            migrationBuilder.Sql(
                "update Fields set Unit = '> M' + NCHAR(937) where FieldType='Number' and Label like '%/G'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
