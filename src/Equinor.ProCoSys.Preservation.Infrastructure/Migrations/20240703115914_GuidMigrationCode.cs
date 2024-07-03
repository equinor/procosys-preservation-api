using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Equinor.ProCoSys.Preservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GuidMigrationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Tags SET ProCoSysGuid = Guid");
            migrationBuilder.Sql("UPDATE PreservationPeriods SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE RequirementTypes SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE RequirementDefinitions SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE TagRequirements SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE PreservationPeriods SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE Fields SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE Actions SET Guid = NEWID()");

            migrationBuilder.Sql(
                @"UPDATE Actions 
                    SET TagGuid = tag.Guid
                    FROM Actions act
                    JOIN Tags tag ON act.TagId = tag.Id
                "
            );

            migrationBuilder.Sql(
                @"UPDATE RequirementDefinitions 
                    SET RequirementTypeGuid = reqType.Guid
                    FROM RequirementDefinitions def
                    JOIN RequirementTypes reqType ON def.RequirementTypeId = reqType.Id
                "
            );

            migrationBuilder.Sql(
                @"UPDATE TagRequirements 
                    SET RequirementDefinitionGuid = def.Guid
                    FROM TagRequirements req
                    JOIN RequirementDefinitions def ON def.id = req.RequirementDefinitionId
                "
            );

            migrationBuilder.Sql(
                @"UPDATE Fields 
                    SET RequirementDefinitionGuid = def.Guid
                    FROM Fields field
                    JOIN RequirementDefinitions def ON def.id = field.RequirementDefinitionId
                "
            );

            migrationBuilder.Sql(
                @"UPDATE TagRequirements 
                    SET TagGuid = tag.Guid
                    FROM TagRequirements req
                    JOIN Tags tag ON req.TagId = tag.Id
                "
            );

            migrationBuilder.Sql(
                @"UPDATE PreservationPeriods 
                    SET TagRequirementGuid = req.Guid
                    FROM PreservationPeriods pp
                    JOIN TagRequirements req ON req.id = pp.TagRequirementId
                "
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
