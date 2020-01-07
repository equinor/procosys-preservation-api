using System;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.EventHandlers;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.MainApi;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, string dbConnectionString, string mainApiAddress)
        {
            services.AddDbContext<PreservationContext>(options =>
            {
                options.UseSqlServer(dbConnectionString);
            });

            // Transient
            services.AddHttpClient<MainApiService>(client =>
            {
                client.BaseAddress = new Uri(mainApiAddress);
            });

            // Transient - Created each time it is requested from the service container
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPlantProvider, PlantProvider>();
            services.AddTransient<ITagApiService>(x => x.GetRequiredService<MainApiService>());
            services.AddTransient<IPlantApiService>(x => x.GetRequiredService<MainApiService>());

            // Scoped - Created once per client request (connection)
            services.AddScoped<IBearerTokenProvider, RequestBearerTokenProvider>();
            services.AddScoped<IReadOnlyContext, PreservationContext>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IModeRepository, ModeRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
            services.AddScoped<IRequirementTypeRepository, RequirementTypeRepository>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<ITimeService, TimeService>();
        }
    }
}
