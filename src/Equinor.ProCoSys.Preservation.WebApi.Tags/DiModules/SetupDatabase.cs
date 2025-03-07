using Equinor.ProCoSys.Preservation.WebApi.Tags.Middleware;
using Equinor.ProCoSys.Preservation.WebApi.Tags.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.DiModules;

public static class SetupDatabase
{
    public static void ConfigureDatabase(this IHostApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopment())
        {
            return;
        }

        if (builder.Configuration.GetValue<bool>("Application:MigrateDatabase"))
        {
            builder.Services.AddHostedService<DatabaseMigrator>();
        }

        if (builder.Configuration.GetValue<bool>("Application:SeedDummyData"))
        {
            builder.Services.AddHostedService<Seeder>();
        }
    }
}
