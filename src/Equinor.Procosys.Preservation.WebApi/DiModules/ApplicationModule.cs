using Equinor.Procosys.Preservation.Command.EventHandlers;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Domain.Time;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Infrastructure.Caching;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.MainApi;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Equinor.Procosys.Preservation.MainApi.TagFunction;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            TimeService.SetProvider(new SystemTimeProvider());

            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));
            services.Configure<TagOptions>(configuration.GetSection("ApiOptions"));
            services.Configure<PermissionOptions>(configuration.GetSection("PermissionOptions"));

            services.AddDbContext<PreservationContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PreservationContext"));
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped<IAccessValidator, AccessValidator>();
            services.AddScoped<IPlantAccessChecker, PlantAccessChecker>();
            services.AddScoped<IProjectAccessChecker, ProjectAccessChecker>();
            services.AddScoped<IContentRestrictionsChecker, ContentRestrictionsChecker>();
            services.AddScoped<ITagHelper, TagHelper>();
            services.AddScoped<IPlantProvider, PlantProvider>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<PreservationContext>());
            services.AddScoped<IReadOnlyContext, PreservationContext>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IModeRepository, ModeRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
            services.AddScoped<IRequirementTypeRepository, RequirementTypeRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ITagFunctionRepository, TagFunctionRepository>();

            services.AddScoped<IBearerTokenProvider, RequestBearerTokenProvider>();
            services.AddScoped<IBearerTokenApiClient, BearerTokenApiClient>();
            services.AddScoped<ITagApiService, MainApiTagService>();
            services.AddScoped<IPlantApiService, MainApiPlantService>();
            services.AddScoped<IProjectApiService, MainApiProjectService>();
            services.AddScoped<IAreaApiService, MainApiAreaService>();
            services.AddScoped<IDisciplineApiService, MainApiDisciplineService>();
            services.AddScoped<ITagFunctionApiService, MainApiTagFunctionService>();
            services.AddScoped<IPermissionApiService, MainApiPermissionService>();
            
            services.AddScoped<IRequirementDefinitionValidator, RequirementDefinitionValidator>();
            services.AddScoped<ITagValidator, TagValidator>();
            services.AddScoped<IProjectValidator, ProjectValidator>();
            services.AddScoped<IStepValidator, StepValidator>();
            services.AddScoped<IJourneyValidator, JourneyValidator>();
            services.AddScoped<IModeValidator, ModeValidator>();
            services.AddScoped<IResponsibleValidator, ResponsibleValidator>();
            services.AddScoped<IFieldValidator, FieldValidator>();
            services.AddScoped<IActionValidator, ActionValidator>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<ICacheManager, CacheManager>();
        }
    }
}
