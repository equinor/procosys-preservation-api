using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Command.EventHandlers;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.SavedFilterValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Infrastructure.Caching;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.MainApi;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Certificate;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.MainApi.Me;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Caches;
using Equinor.ProCoSys.Preservation.WebApi.Excel;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            TimeService.SetProvider(new SystemTimeProvider());

            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));
            services.Configure<TagOptions>(configuration.GetSection("TagOptions"));
            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));
            services.Configure<AuthenticatorOptions>(configuration.GetSection("Authenticator"));
            services.Configure<SynchronizationOptions>(configuration.GetSection("Synchronization"));

            services.AddDbContext<PreservationContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("PreservationContext");
                options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });

            // Hosted services
            services.AddHostedService<TimedSynchronization>();

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<IExcelConverter, ExcelConverter>();
            services.AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>();
            services.AddScoped<IPersonCache, PersonCache>();
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
            services.AddScoped<IProjectChecker, ProjectChecker>();
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
            services.AddScoped<ISettingRepository, SettingRepository>();

            services.AddScoped<Authenticator>();
            services.AddScoped<IBearerTokenProvider>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IBearerTokenSetter>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IAuthenticator>(x => x.GetRequiredService<Authenticator>());
            services.AddScoped<IBearerTokenApiClient, BearerTokenApiClient>();
            services.AddScoped<ITagApiService, MainApiTagService>();
            services.AddScoped<IMeApiService, MainApiMeService>();
            services.AddScoped<IPlantApiService, MainApiPlantService>();
            services.AddScoped<IPersonApiService, MainApiPersonService>();
            services.AddScoped<IProjectApiService, MainApiProjectService>();
            services.AddScoped<IAreaApiService, MainApiAreaService>();
            services.AddScoped<IDisciplineApiService, MainApiDisciplineService>();
            services.AddScoped<IResponsibleApiService, MainApiResponsibleService>();
            services.AddScoped<ITagFunctionApiService, MainApiTagFunctionService>();
            services.AddScoped<IPermissionApiService, MainApiPermissionService>();
            services.AddScoped<ICertificateApiService, MainApiCertificateService>();
            services.AddScoped<IBlobStorage, AzureBlobService>();
            services.AddScoped<IBusReceiverService, BusReceiverService>();
            services.AddScoped<ICertificateEventProcessorService, CertificateEventProcessorService>();

            services.AddScoped<IRequirementDefinitionValidator, RequirementDefinitionValidator>();
            services.AddScoped<ITagValidator, TagValidator>();
            services.AddScoped<IProjectValidator, ProjectValidator>();
            services.AddScoped<IStepValidator, StepValidator>();
            services.AddScoped<IJourneyValidator, JourneyValidator>();
            services.AddScoped<IModeValidator, ModeValidator>();
            services.AddScoped<IResponsibleValidator, ResponsibleValidator>();
            services.AddScoped<IFieldValidator, FieldValidator>();
            services.AddScoped<IActionValidator, ActionValidator>();
            services.AddScoped<IRequirementTypeValidator, RequirementTypeValidator>();
            services.AddScoped<ITagFunctionValidator, TagFunctionValidator>();
            services.AddScoped<IRowVersionValidator, RowVersionValidator>();
            services.AddScoped<ISavedFilterValidator, SavedFilterValidator>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<IBusReceiverServiceFactory, ScopedBusReceiverServiceFactory>();
        }
    }
}
