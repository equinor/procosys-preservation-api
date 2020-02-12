using Equinor.Procosys.Preservation.Command.EventHandlers;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Journey;
using Equinor.Procosys.Preservation.Command.Validators.Mode;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Responsible;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.MainApi;
using Equinor.Procosys.Preservation.MainApi.AreaCode;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));

            services.AddDbContext<PreservationContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PreservationContext"));
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped<IPlantProvider, PlantProvider>();
            services.AddScoped<IBearerTokenProvider, RequestBearerTokenProvider>();
            services.AddScoped<IBearerTokenApiClient, BearerTokenApiClient>();
            services.AddScoped<ITagApiService, MainApiTagService>();
            services.AddScoped<IPlantApiService, MainApiPlantService>();
            services.AddScoped<IAreaCodeApiService, MainApiAreaCodeService>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<PreservationContext>());
            services.AddScoped<IReadOnlyContext>(x => x.GetRequiredService<PreservationContext>());

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IModeRepository, ModeRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
            services.AddScoped<IRequirementTypeRepository, RequirementTypeRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            
            services.AddScoped<IRequirementDefinitionValidator, RequirementDefinitionValidator>();
            services.AddScoped<ITagValidator, TagValidator>();
            services.AddScoped<IProjectValidator, ProjectValidator>();
            services.AddScoped<IStepValidator, StepValidator>();
            services.AddScoped<IJourneyValidator, JourneyValidator>();
            services.AddScoped<IModeValidator, ModeValidator>();
            services.AddScoped<IResponsibleValidator, ResponsibleValidator>();
            services.AddScoped<IFieldValidator, FieldValidator>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<ITimeService, TimeService>();
        }
    }
}
