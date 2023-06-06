using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateGuids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update Tags set Guid = ObjectGuid where ProCoSysGuid is null");
            migrationBuilder.Sql("update Tags set Guid = ProCoSysGuid where ProCoSysGuid is not null");

            migrationBuilder.Sql("update History set History.SourceGuid = T.Guid from Tags T join History H on T.ObjectGuid = H.ObjectGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
