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
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Delete;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MassTransit;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events.PostSave;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;

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
            services.AddTransient<IDomainToIntegrationEventConverter<ProjectTagAddedEvent>, ProjectTagAddedEventConverter>();
            services.AddTransient<IDomainToIntegrationEventConverter<RequirementTypeRequirementDefinitionAddedEvent>, RequirementTypeRequirementDefinitionAddedEventConverter>();
            
            services.AddTransient<ICreateChildEventHelper<Project, Tag, TagEvent>, CreateTagEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent>, CreateTagRequirementEventHelper>();
            services.AddTransient<INotificationHandler<TagDeletedEvent>, IntegrationDeleteEventHandler<TagDeletedEvent, Tag>>();
            
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Tag>>, IntegrationEventHandler<PlantEntityModifiedEvent<Tag>, Tag>>();
            services.AddTransient<IPublishEntityEventHelper<Tag>, PublishTagEventHelper>();
            services.AddTransient<ICreateEventHelper<Tag, TagDeleteEvent>, CreateTagDeleteEventHelper>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Tag>, PublishDeleteEntityEventHelper<Tag, TagDeleteEvent>>();
            
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<TagRequirement>>, IntegrationEventHandler<PlantEntityModifiedEvent<TagRequirement>, TagRequirement>>();
            services.AddTransient<IPublishEntityEventHelper<TagRequirement>, PublishTagRequirementEventHelper>();
            services.AddTransient<INotificationHandler<TagRequirementDeletedEvent>, DeleteTagRequirementEventHandler>();
            
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<RequirementDefinition>>, IntegrationEventHandler<PlantEntityModifiedEvent<RequirementDefinition>, RequirementDefinition>>();
            services.AddTransient<INotificationHandler<RequirementDefinitionDeletedEvent>, IntegrationDeleteEventHandler<RequirementDefinitionDeletedEvent, RequirementDefinition>>();
            services.AddTransient<ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>, CreateRequirementDefinitionEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementDefinition, RequirementDefinitionDeleteEvent>, CreateRequirementDefinitionDeletedEventHelper>();
            services.AddTransient<IPublishEntityEventHelper<RequirementDefinition>, PublishRequirementDefinitionEventHelper>();
            services.AddTransient<IPublishDeleteEntityEventHelper<RequirementDefinition>, PublishDeleteEntityEventHelper<RequirementDefinition, RequirementDefinitionDeleteEvent>>();

            services.AddTransient<ICreateEventHelper<Action, ActionEvent>, CreateActionEventHelper>();
            services.AddTransient<ICreateEventHelper<Field, FieldEvent>, CreateFieldEventHelper>();
            services.AddTransient<ICreateEventHelper<Field, RequirementFieldDeleteEvent>, CreateRequirementFieldDeleteEventHelper>();
            services.AddTransient<ICreateEventHelper<Journey, JourneyEvent>, CreateJourneyEventHelper>();
            services.AddTransient<ICreateEventHelper<Journey, JourneyDeleteEvent>, CreateJourneyDeletedEventHelper>();
            services.AddTransient<ICreateEventHelper<Mode, ModeEvent>, CreateModeEventHelper>();
            services.AddTransient<ICreateEventHelper<Mode, ModeDeleteEvent>, CreateModeDeletedEventHelper>();
            services.AddTransient<ICreateEventHelper<PreservationPeriod, PreservationPeriodsEvent>, CreatePreservationPeriodEventHelper>();
            services.AddTransient<ICreateEventHelper<PreservationPeriod, PreservationPeriodDeleteEvent>, CreatePreservationPeriodDeletedEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementType, RequirementTypeEvent>, CreateRequirementTypeEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementType, RequirementTypeDeleteEvent>, CreateRequirementTypeDeleteEventHelper>();
            services.AddTransient<ICreateEventHelper<Responsible, ResponsibleEvent>, CreateResponsibleEventHelper>();
            services.AddTransient<ICreateEventHelper<Responsible, ResponsibleDeleteEvent>, CreateResponsibleDeleteEventHelper>();
            services.AddTransient<ICreateEventHelper<Step, StepEvent>, CreateStepEventHelper>();
            services.AddTransient<ICreateEventHelper<Step, StepDeleteEvent>, CreateStepDeleteEventHelper>();
            
            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();
            
            services.AddTransient<IPublishEntityEventHelper<Action>, PublishEntityEventHelper<Action, ActionEvent>>();
            services.AddTransient<IPublishEntityEventHelper<Journey>, PublishEntityEventHelper<Journey, JourneyEvent>>();
            services.AddTransient<IPublishEntityEventHelper<Mode>, PublishEntityEventHelper<Mode, ModeEvent>>();
            services.AddTransient<IPublishEntityEventHelper<Field>, PublishEntityEventHelper<Field, FieldEvent>>();
            services.AddTransient<IPublishEntityEventHelper<PreservationPeriod>, PublishEntityEventHelper<PreservationPeriod, PreservationPeriodsEvent>>();
            services.AddTransient<IPublishEntityEventHelper<RequirementType>, PublishEntityEventHelper<RequirementType, RequirementTypeEvent>>();
            services.AddTransient<IPublishEntityEventHelper<Responsible>, PublishEntityEventHelper<Responsible, ResponsibleEvent>>();
            services.AddTransient<IPublishEntityEventHelper<Step>, PublishEntityEventHelper<Step, StepEvent>>();
            
            services.AddTransient<IPublishDeleteEntityEventHelper<Journey>, PublishDeleteEntityEventHelper<Journey, JourneyDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Mode>, PublishDeleteEntityEventHelper<Mode, ModeDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Field>, PublishDeleteEntityEventHelper<Field, RequirementFieldDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<PreservationPeriod>, PublishDeleteEntityEventHelper<PreservationPeriod, PreservationPeriodDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<RequirementType>, PublishDeleteEntityEventHelper<RequirementType, RequirementTypeDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Responsible>, PublishDeleteEntityEventHelper<Responsible, ResponsibleDeleteEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Step>, PublishDeleteEntityEventHelper<Step, StepDeleteEvent>>();

            services.AddTransient<INotificationHandler<ActionPostSaveEvent>, IntegrationEventHandler<ActionPostSaveEvent, Action>>();
            services.AddTransient<INotificationHandler<FieldPostSaveEvent>, IntegrationEventHandler<FieldPostSaveEvent, Field>>();
            services.AddTransient<INotificationHandler<JourneyPostSaveEvent>, IntegrationEventHandler<JourneyPostSaveEvent, Journey>>();
            services.AddTransient<INotificationHandler<ModePostSaveEvent>, IntegrationEventHandler<ModePostSaveEvent, Mode>>();
            services.AddTransient<INotificationHandler<PreservationPeriodPostSaveEvent>, IntegrationEventHandler<PreservationPeriodPostSaveEvent, PreservationPeriod>>();
            services.AddTransient<INotificationHandler<RequirementTypePostSaveEvent>, IntegrationEventHandler<RequirementTypePostSaveEvent, RequirementType>>();
            services.AddTransient<INotificationHandler<ResponsiblePostSaveEvent>, IntegrationEventHandler<ResponsiblePostSaveEvent, Responsible>>();
            services.AddTransient<INotificationHandler<StepPostSaveEvent>, IntegrationEventHandler<StepPostSaveEvent, Step>>();

            services.AddTransient<INotificationHandler<JourneyDeletedEvent>, IntegrationDeleteEventHandler<JourneyDeletedEvent, Journey>>();
            services.AddTransient<INotificationHandler<RequirementDeletedFieldEvent>, IntegrationDeleteEventHandler<RequirementDeletedFieldEvent, Field>>();
            services.AddTransient<INotificationHandler<ModeDeletedEvent>, IntegrationDeleteEventHandler<ModeDeletedEvent, Mode>>();
            services.AddTransient<INotificationHandler<PreservationPeriodDeletedEvent>, IntegrationDeleteEventHandler<PreservationPeriodDeletedEvent, PreservationPeriod>>();
            services.AddTransient<INotificationHandler<ResponsibleDeletedEvent>, IntegrationDeleteEventHandler<ResponsibleDeletedEvent, Responsible>>();
            services.AddTransient<INotificationHandler<StepDeletedEvent>, IntegrationDeleteEventHandler<StepDeletedEvent, Step>>();

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
