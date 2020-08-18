using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Command.EventHandlers;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.AttachmentValidators;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagFunctionValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
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
using Equinor.Procosys.Preservation.MainApi.Certificate;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Responsible;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Equinor.Procosys.Preservation.MainApi.TagFunction;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Equinor.Procosys.Preservation.WebApi.Synchronization;
using Equinor.Procosys.Preservation.WebApi.Authentication;
using Equinor.Procosys.Preservation.WebApi.Excel;
using Equinor.Procosys.Preservation.WebApi.Telemetry;

namespace Equinor.Procosys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            TimeService.SetProvider(new SystemTimeProvider());

            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));
            services.Configure<TagOptions>(configuration.GetSection("ApiOptions"));
            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));
            services.Configure<AttachmentOptions>(configuration.GetSection("AttachmentOptions"));
            services.Configure<SynchronizationOptions>(configuration.GetSection("Synchronization"));
            services.Configure<AuthenticatorOptions>(configuration.GetSection("Authenticator"));

            services.AddDbContext<PreservationContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("PreservationContext"));
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Hosted services
            services.AddHostedService<TimedSynchronization>();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<IExcelConverter, ExcelConverter>();
            services.AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>();
            services.AddScoped<IPlantCache, PlantCache>();
            services.AddScoped<IPermissionCache, PermissionCache>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            services.AddScoped<IClaimsProvider, ClaimsProvider>();
            services.AddScoped<CurrentUserProvider>();
            services.AddScoped<ICurrentUserProvider>(x => x.GetRequiredService<CurrentUserProvider>());
            services.AddScoped<ICurrentUserSetter>(x => x.GetRequiredService<CurrentUserProvider>());
            services.AddScoped<PlantProvider>();
            services.AddScoped<IPlantProvider>(x => x.GetRequiredService<PlantProvider>());
            services.AddScoped<IPlantSetter>(x => x.GetRequiredService<PlantProvider>());
            services.AddScoped<IAccessValidator, AccessValidator>();
            services.AddScoped<IProjectAccessChecker, ProjectAccessChecker>();
            services.AddScoped<IContentRestrictionsChecker, ContentRestrictionsChecker>();
            services.AddScoped<ITagHelper, TagHelper>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<PreservationContext>());
            services.AddScoped<IReadOnlyContext, PreservationContext>();
            services.AddScoped<ISynchronizationService, SynchronizationService>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IModeRepository, ModeRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IResponsibleRepository, ResponsibleRepository>();
            services.AddScoped<IRequirementTypeRepository, RequirementTypeRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<ITagFunctionRepository, TagFunctionRepository>();
            services.AddScoped<IHistoryRepository, HistoryRepository>();

            services.AddScoped<Authenticator>();
            services.AddScoped<IBearerTokenProvider>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IBearerTokenSetter>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IApplicationAuthenticator>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IBearerTokenApiClient, BearerTokenApiClient>();
            services.AddScoped<ITagApiService, MainApiTagService>();
            services.AddScoped<IPlantApiService, MainApiPlantService>();
            services.AddScoped<IProjectApiService, MainApiProjectService>();
            services.AddScoped<IAreaApiService, MainApiAreaService>();
            services.AddScoped<IDisciplineApiService, MainApiDisciplineService>();
            services.AddScoped<IResponsibleApiService, MainApiResponsibleService>();
            services.AddScoped<ITagFunctionApiService, MainApiTagFunctionService>();
            services.AddScoped<IPermissionApiService, MainApiPermissionService>();
            services.AddScoped<ICertificateApiService, MainApiCertificateService>();
            services.AddScoped<IBlobStorage, AzureBlobService>();

            services.AddScoped<IRequirementDefinitionValidator, RequirementDefinitionValidator>();
            services.AddScoped<ITagValidator, TagValidator>();
            services.AddScoped<IProjectValidator, ProjectValidator>();
            services.AddScoped<IStepValidator, StepValidator>();
            services.AddScoped<IJourneyValidator, JourneyValidator>();
            services.AddScoped<IModeValidator, ModeValidator>();
            services.AddScoped<IResponsibleValidator, ResponsibleValidator>();
            services.AddScoped<IFieldValidator, FieldValidator>();
            services.AddScoped<IActionValidator, ActionValidator>();
            services.AddScoped<IAttachmentValidator, AttachmentValidator>();
            services.AddScoped<IRequirementTypeValidator, RequirementTypeValidator>();
            services.AddScoped<ITagFunctionValidator, TagFunctionValidator>();
            services.AddScoped<IRowVersionValidator, RowVersionValidator>();
            services.AddScoped<ISavedFilterValidator, SavedFilterValidator>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<ICacheManager, CacheManager>();
        }
    }
}
