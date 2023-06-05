using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateTagGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Tags set Guid = ObjectGuid");
            migrationBuilder.Sql("update Tags set Guid = ProCoSysGuid where ProCoSysGuid is not null");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
