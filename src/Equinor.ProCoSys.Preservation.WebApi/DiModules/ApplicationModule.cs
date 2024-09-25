using System.Text.Json.Serialization;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.BlobStorage;
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
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Certificate;
using Equinor.ProCoSys.Auth.Client;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.MainApi.Me;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Excel;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Equinor.ProCoSys.Auth.Authentication;
using Equinor.ProCoSys.Common.Caches;
using Equinor.ProCoSys.Common.Telemetry;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.WebApi.MassTransit;
using MassTransit;
using MediatR;

namespace Equinor.ProCoSys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));
            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));

            services.Configure<TagOptions>(configuration.GetSection("TagOptions"));
            services.Configure<PreservationAuthenticatorOptions>(configuration.GetSection("Authenticator"));
            services.Configure<SynchronizationOptions>(configuration.GetSection("Synchronization"));

            services.AddDbContext<PreservationContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("PreservationContext");
                options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });

            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<PreservationContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    var connectionString = configuration.GetConnectionString("ServiceBus");
                    cfg.Host(connectionString);

                    cfg.OverrideDefaultBusEndpointQueueName("preservationfamtransferqueue");

                    cfg.MessageTopology.SetEntityNameFormatter(new PreservationNameFormatter());
                    cfg.UseRawJsonSerializer();
                    cfg.ConfigureJsonSerializerOptions(opts =>
                    {
                        opts.Converters.Add(new JsonStringEnumConverter());

                        // Set it to null to use the default .NET naming convention (PascalCase)
                        opts.PropertyNamingPolicy = null;
                        return opts;
                    });

                    cfg.AutoStart = true;
                });
            });

            // Hosted services

            // TimedSynchronization WAS WRITTEN TO RUN A ONETIME TRANSFORMATION WHEN WE INTRODUCED ProCoSysGuid
            // WE KEEP THE CODE ... MAYBE WE WANT TO DO SIMILAR STUFF LATER
            // services.AddHostedService<TimedSynchronization>();

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container
            services.AddTransient<ICreateEventHelper<Action>, CreateActionEventHelper>();
            services.AddTransient<ICreateEventHelper<Field>, CreateFieldEventHelper>();
            services.AddTransient<ICreateEventHelper<Journey>, CreateJourneyEventHelper>();
            services.AddTransient<ICreateEventHelper<Mode>, CreateModeEventHelper>();
            services.AddTransient<ICreateEventHelper<PreservationPeriod>, CreatePreservationPeriodEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementDefinition>, CreateRequirementDefinitionEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementType>, CreateRequirementTypeEventHelper>();
            services.AddTransient<ICreateEventHelper<Responsible>, CreateResponsibleEventHelper>();
            services.AddTransient<ICreateEventHelper<Step>, CreateStepEventHelper>();
            services.AddTransient<ICreateEventHelper<Tag>, CreateTagEventHelper>();
            services.AddTransient<ICreateEventHelper<TagRequirement>, CreateTagRequirementEventEventHelper>();
            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();
            
            services.AddTransient<INotificationHandler<ActionAddedEvent>, IntegrationEventHandler<ActionAddedEvent, Action>>();
            services.AddTransient<INotificationHandler<JourneyAddedEvent>, IntegrationEventHandler<JourneyAddedEvent, Journey>>();
            services.AddTransient<INotificationHandler<ModeAddedEvent>, IntegrationEventHandler<ModeAddedEvent, Mode>>();
            services.AddTransient<INotificationHandler<PreservationPeriodAddedEvent>, IntegrationEventHandler<PreservationPeriodAddedEvent, PreservationPeriod>>();
            services.AddTransient<INotificationHandler<RequirementDefinitionAddedEvent>, IntegrationEventHandler<RequirementDefinitionAddedEvent, RequirementDefinition>>();
            services.AddTransient<INotificationHandler<RequirementAddedFieldEvent>, IntegrationEventHandler<RequirementAddedFieldEvent, Field>>();
            services.AddTransient<INotificationHandler<RequirementTypeAddedEvent>, IntegrationEventHandler<RequirementTypeAddedEvent, RequirementType>>();
            services.AddTransient<INotificationHandler<ResponsibleAddedEvent>, IntegrationEventHandler<ResponsibleAddedEvent, Responsible>>();
            services.AddTransient<INotificationHandler<StepAddedEvent>, IntegrationEventHandler<StepAddedEvent, Step>>();
            services.AddTransient<INotificationHandler<TagCreatedEvent>, IntegrationEventHandler<TagCreatedEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagRequirementAddedEvent>, IntegrationEventHandler<TagRequirementAddedEvent, TagRequirement>>();
            
            services.AddTransient<INotificationHandler<ActionClosedEvent>, IntegrationEventHandler<ActionClosedEvent, Action>>();
            services.AddTransient<INotificationHandler<ActionUpdatedEvent>, IntegrationEventHandler<ActionUpdatedEvent, Action>>();
            services.AddTransient<INotificationHandler<JourneyUpdatedEvent>, IntegrationEventHandler<JourneyUpdatedEvent, Journey>>();
            services.AddTransient<INotificationHandler<ModeUpdatedEvent>, IntegrationEventHandler<ModeUpdatedEvent, Mode>>();
            services.AddTransient<INotificationHandler<PreservationPeriodUpdatedEvent>, IntegrationEventHandler<PreservationPeriodUpdatedEvent, PreservationPeriod>>();
            services.AddTransient<INotificationHandler<RequirementDefinitionUpdatedEvent>, IntegrationEventHandler<RequirementDefinitionUpdatedEvent, RequirementDefinition>>();
            services.AddTransient<INotificationHandler<RequirementTypeUpdatedEvent>, IntegrationEventHandler<RequirementTypeUpdatedEvent, RequirementType>>();
            services.AddTransient<INotificationHandler<RequirementUpdatedFieldEvent>, IntegrationEventHandler<RequirementUpdatedFieldEvent, Field>>();
            services.AddTransient<INotificationHandler<ResponsibleUpdatedEvent>, IntegrationEventHandler<ResponsibleUpdatedEvent, Responsible>>();
            services.AddTransient<INotificationHandler<StepUpdatedEvent>, IntegrationEventHandler<StepUpdatedEvent, Step>>();
            services.AddTransient<INotificationHandler<TagRequirementPreservedEvent>, IntegrationEventHandler<TagRequirementPreservedEvent, TagRequirement>>();
            services.AddTransient<INotificationHandler<TagUnvoidedInSourceEvent>, IntegrationEventHandler<TagUnvoidedInSourceEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagUnvoidedEvent>, IntegrationEventHandler<TagUnvoidedEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagRequirementUnvoidedEvent>, IntegrationEventHandler<TagRequirementUnvoidedEvent, TagRequirement>>();
            services.AddTransient<INotificationHandler<TagRequirementUpdatedEvent>, IntegrationEventHandler<TagRequirementUpdatedEvent, TagRequirement>>();
            services.AddTransient<INotificationHandler<TagUpdatedEvent>, IntegrationEventHandler<TagUpdatedEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagVoidedEvent>, IntegrationEventHandler<TagVoidedEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagVoidedInSourceEvent>, IntegrationEventHandler<TagVoidedInSourceEvent, Tag>>();
            services.AddTransient<INotificationHandler<TagRequirementVoidedEvent>, IntegrationEventHandler<TagRequirementVoidedEvent, TagRequirement>>();

            // Scoped - Created once per client request (connection)
            services.AddScoped<IExcelConverter, ExcelConverter>();
            services.AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>();
            services.AddScoped<IPersonCache, PersonCache>();
            services.AddScoped<IPermissionCache, PermissionCache>();
            services.AddScoped<IAccessValidator, AccessValidator>();
            services.AddScoped<IProjectChecker, ProjectChecker>();
            services.AddScoped<IProjectAccessChecker, ProjectAccessChecker>();
            services.AddScoped<IRestrictionRolesChecker, RestrictionRolesChecker>();
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
            services.AddScoped<ILocalPersonRepository, LocalPersonRepository>();
            services.AddScoped<ITagFunctionRepository, TagFunctionRepository>();
            services.AddScoped<IHistoryRepository, HistoryRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();

            services.AddScoped<IAuthenticatorOptions, AuthenticatorOptions>();
            services.AddScoped<ITagApiService, MainApiTagService>();
            services.AddScoped<IMeApiService, MainApiMeService>();
            services.AddScoped<IProjectApiService, MainApiProjectService>();
            services.AddScoped<IAreaApiService, MainApiAreaService>();
            services.AddScoped<IDisciplineApiService, MainApiDisciplineService>();
            services.AddScoped<IResponsibleApiService, MainApiResponsibleService>();
            services.AddScoped<ITagFunctionApiService, MainApiTagFunctionService>();
            services.AddScoped<ICertificateApiService, MainApiCertificateService>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
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
            services.AddSingleton<IBusReceiverServiceFactory, ScopedBusReceiverServiceFactory>();
        }
    }
}
