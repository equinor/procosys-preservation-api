using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SavedFiltersMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update JSON data in the Criteria column by replacing "preservationStatus":null with an empty array[]
            migrationBuilder.Sql("UPDATE SavedFilters " +
                                 "SET Criteria = REPLACE(Criteria, '\"preservationStatus\":null', '\"preservationStatus\":[]') " +
                                "WHERE Criteria LIKE '%\"preservationStatus\":null%'");

            // Update JSON data in the Criteria column by replacing "actionStatus":null with an empty array[]
            migrationBuilder.Sql("UPDATE SavedFilters " +
                                 "SET Criteria = REPLACE(Criteria, '\"actionStatus\":null', '\"actionStatus\":[]') " +
                                 "WHERE Criteria LIKE '%\"actionStatus\":null%'");

            // Update JSON data in the Criteria column by wrapping the value of "preservationStatus" in an array if it is not null
            migrationBuilder.Sql("UPDATE SavedFilters " +
                                 "SET Criteria = JSON_MODIFY(Criteria, '$.preservationStatus', JSON_QUERY('[\"' + JSON_VALUE(Criteria, '$.preservationStatus') + '\"]')) " +
                                 "WHERE JSON_VALUE(Criteria, '$.preservationStatus') IS NOT NULL");

            // Update JSON data in the Criteria column by wrapping the value of "actionStatus" in an array if it is not null
            migrationBuilder.Sql("UPDATE SavedFilters " +
                                 "SET Criteria = JSON_MODIFY(Criteria, '$.actionStatus', JSON_QUERY('[\"' + JSON_VALUE(Criteria, '$.actionStatus') + '\"]')) " +
                                 "WHERE JSON_VALUE(Criteria, '$.actionStatus') IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
