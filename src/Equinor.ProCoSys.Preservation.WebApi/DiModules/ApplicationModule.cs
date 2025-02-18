using System;
using System.Text.Json.Serialization;
using Azure.Core;
using Equinor.ProCoSys.Auth.Authentication;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Auth.Client;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Caches;
using Equinor.ProCoSys.Common.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.Preservation.Command.EventHandlers;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Delete;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Command.Services.ProjectImportService;
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
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Certificate;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.MainApi.Me;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Equinor.ProCoSys.Preservation.Query.UserDelegationProvider;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Excel;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Synchronization;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration, TokenCredential credential)
        {
            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));
            services.Configure<MainApiAuthenticatorOptions>(configuration.GetSection("MainApi"));
            services.Configure<CacheOptions>(configuration.GetSection("CacheOptions"));
            services.Configure<BlobStorageOptions>(configuration.GetSection("BlobStorage"));

            services.Configure<TagOptions>(configuration.GetSection("TagOptions"));
            services.Configure<ApplicationOptions>(configuration.GetSection("Application"));
            services.Configure<SynchronizationOptions>(configuration.GetSection("Synchronization"));

            services.AddDbContext<PreservationContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("PreservationContext");
                options.UseSqlServer(connectionString,
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
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
                    var serviceBusNamespace = configuration.GetConfig<string>("ServiceBus:Namespace");
                    
                    var serviceUri = new Uri($"sb://{serviceBusNamespace}.servicebus.windows.net/");
                    cfg.Host(serviceUri, host =>
                    {
                        host.TokenCredential = credential;
                    });

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
            
            // Singleton - Created the first time they are requested
            services.AddSingleton<IUserDelegationProvider, UserDelegationProvider>();

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container
            services
                .AddTransient<IDomainToIntegrationEventConverter<ChildAddedEvent<Project, Tag>>,
                    ProjectTagAddedEventConverter>();

            services
                .AddTransient<INotificationHandler<ModifiedEvent<Tag>>,
                    IntegrationEventHandler<ModifiedEvent<Tag>, Tag>>();
            services.AddTransient<IPublishEntityEventHelper<Tag>, PublishEntityEventHelper<Tag, TagEvent>>();
            services.AddTransient<ICreateEventHelper<Tag, TagEvent>, CreateTagEventHelper>();
            services.AddTransient<ICreateTagDeleteEventHelper, CreateTagDeleteEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Project, Tag, TagEvent>, CreateProjectTagEventHelper>();

            services
                .AddTransient<INotificationHandler<ModifiedEvent<TagRequirement>>,
                    IntegrationEventHandler<ModifiedEvent<TagRequirement>, TagRequirement>>();
            services.AddTransient<INotificationHandler<TagRequirementDeletedEvent>, DeleteTagRequirementEventHandler>();
            services
                .AddTransient<IPublishEntityEventHelper<TagRequirement>,
                    PublishEntityEventHelper<TagRequirement, TagRequirementEvent>>();
            services
                .AddTransient<ICreateEventHelper<TagRequirement, TagRequirementEvent>,
                    CreateTagRequirementEventHelper>();
            services
                .AddTransient<ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent>,
                    CreateTagTagRequirementEventHelper>();

            services
                .AddTransient<INotificationHandler<ChildAddedEvent<RequirementType, RequirementDefinition>>,
                    ChildEventHandler<RequirementType, RequirementDefinition, RequirementDefinitionEvent>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<RequirementDefinition>>, IntegrationEventHandler<
                    ModifiedEvent<RequirementDefinition>, RequirementDefinition>>();
            services
                .AddTransient<IPublishEntityEventHelper<RequirementDefinition>,
                    PublishEntityEventHelper<RequirementDefinition, RequirementDefinitionEvent>>();
            services
                .AddTransient<ICreateEventHelper<RequirementDefinition, RequirementDefinitionEvent>,
                    CreateRequirementDefinitionEventHelper>();
            services
                .AddTransient<
                    ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>,
                    CreateRequirementTypeRequirementDefinitionEventHelper>();

            services
                .AddTransient<INotificationHandler<ChildAddedEvent<Tag, Action>>,
                    ChildEventHandler<Tag, Action, ActionEvent>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<Action>>,
                    IntegrationEventHandler<ModifiedEvent<Action>, Action>>();
            services.AddTransient<IPublishEntityEventHelper<Action>, PublishEntityEventHelper<Action, ActionEvent>>();
            services.AddTransient<ICreateEventHelper<Action, ActionEvent>, CreateActionEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Tag, Action, ActionEvent>, CreateTagActionEventHelper>();

            services
                .AddTransient<INotificationHandler<ChildAddedEvent<RequirementDefinition, Field>>,
                    ChildEventHandler<RequirementDefinition, Field, FieldEvent>>();
            services
                .AddTransient<INotificationHandler<ChildModifiedEvent<RequirementDefinition, Field>>,
                    ChildEventHandler<RequirementDefinition, Field, FieldEvent>>();
            services
                .AddTransient<ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent>,
                    CreateRequirementDefinitionFieldEventHelper>();

            services
                .AddTransient<INotificationHandler<CreatedEvent<Journey>>,
                    IntegrationEventHandler<CreatedEvent<Journey>, Journey>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<Journey>>,
                    IntegrationEventHandler<ModifiedEvent<Journey>, Journey>>();
            services
                .AddTransient<IPublishEntityEventHelper<Journey>, PublishEntityEventHelper<Journey, JourneyEvent>>();
            services.AddTransient<ICreateEventHelper<Journey, JourneyEvent>, CreateJourneyEventHelper>();

            services
                .AddTransient<INotificationHandler<CreatedEvent<Mode>>,
                    IntegrationEventHandler<CreatedEvent<Mode>, Mode>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<Mode>>,
                    IntegrationEventHandler<ModifiedEvent<Mode>, Mode>>();
            services
                .AddTransient<INotificationHandler<DeletedEvent<Mode>>,
                    IntegrationDeleteEventHandler<DeletedEvent<Mode>, Mode>>();
            services.AddTransient<IPublishEntityEventHelper<Mode>, PublishEntityEventHelper<Mode, ModeEvent>>();
            services
                .AddTransient<IPublishDeleteEntityEventHelper<Mode>,
                    PublishDeleteEntityEventHelper<Mode, ModeDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Mode, ModeEvent>, CreateModeEventHelper>();
            services.AddTransient<ICreateEventHelper<Mode, ModeDeleteEvent>, CreateModeDeletedEventHelper>();

            services
                .AddTransient<INotificationHandler<ChildAddedEvent<TagRequirement, PreservationPeriod>>,
                    ChildEventHandler<TagRequirement, PreservationPeriod, PreservationPeriodsEvent>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<PreservationPeriod>>,
                    IntegrationEventHandler<ModifiedEvent<PreservationPeriod>, PreservationPeriod>>();
            services
                .AddTransient<IPublishEntityEventHelper<PreservationPeriod>,
                    PublishEntityEventHelper<PreservationPeriod, PreservationPeriodsEvent>>();
            services
                .AddTransient<ICreateChildEventHelper<TagRequirement, PreservationPeriod, PreservationPeriodsEvent>,
                    CreateTagRequirementPreservationPeriodEventHelper>();
            services
                .AddTransient<ICreateEventHelper<PreservationPeriod, PreservationPeriodsEvent>,
                    CreatePreservationPeriodEventHelper>();

            services
                .AddTransient<INotificationHandler<CreatedEvent<RequirementType>>,
                    IntegrationEventHandler<CreatedEvent<RequirementType>, RequirementType>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<RequirementType>>,
                    IntegrationEventHandler<ModifiedEvent<RequirementType>, RequirementType>>();
            services
                .AddTransient<IPublishEntityEventHelper<RequirementType>,
                    PublishEntityEventHelper<RequirementType, RequirementTypeEvent>>();
            services
                .AddTransient<ICreateEventHelper<RequirementType, RequirementTypeEvent>,
                    CreateRequirementTypeEventHelper>();

            services
                .AddTransient<INotificationHandler<CreatedEvent<Responsible>>,
                    IntegrationEventHandler<CreatedEvent<Responsible>, Responsible>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<Responsible>>,
                    IntegrationEventHandler<ModifiedEvent<Responsible>, Responsible>>();
            services
                .AddTransient<INotificationHandler<DeletedEvent<Responsible>>,
                    IntegrationDeleteEventHandler<DeletedEvent<Responsible>, Responsible>>();
            services
                .AddTransient<IPublishEntityEventHelper<Responsible>,
                    PublishEntityEventHelper<Responsible, ResponsibleEvent>>();
            services
                .AddTransient<IPublishDeleteEntityEventHelper<Responsible>,
                    PublishDeleteEntityEventHelper<Responsible, ResponsibleDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Responsible, ResponsibleEvent>, CreateResponsibleEventHelper>();
            services
                .AddTransient<ICreateEventHelper<Responsible, ResponsibleDeleteEvent>,
                    CreateResponsibleDeleteEventHelper>();

            services
                .AddTransient<INotificationHandler<ChildAddedEvent<Journey, Step>>,
                    ChildEventHandler<Journey, Step, StepEvent>>();
            services
                .AddTransient<INotificationHandler<ChildModifiedEvent<Journey, Step>>,
                    ChildEventHandler<Journey, Step, StepEvent>>();
            services
                .AddTransient<INotificationHandler<ModifiedEvent<Step>>,
                    IntegrationEventHandler<ModifiedEvent<Step>, Step>>();
            services.AddTransient<IPublishEntityEventHelper<Step>, PublishEntityEventHelper<Step, StepEvent>>();
            services.AddTransient<ICreateChildEventHelper<Journey, Step, StepEvent>, CreateJourneyStepEventHelper>();
            services.AddTransient<ICreateEventHelper<Step, StepEvent>, CreateStepEventHelper>();

            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();

            // Scoped - Created once per client request (connection)
            services.AddScoped<IExcelConverter, ExcelConverter>();
            services.AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>();
            services.AddScoped<IPersonCache, PersonCache>();
            services.AddScoped<IPermissionCache, PermissionCache>();
            services.AddScoped<IAccessValidator, AccessValidator>();
            services.AddScoped<IProjectChecker, ProjectChecker>();
            services.AddScoped<IProjectAccessChecker, ProjectAccessChecker>();
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

            services.AddScoped<IProjectImportService, ProjectImportService>();

            // Singleton - Created the first time they are requested
            services.AddSingleton<IBusReceiverServiceFactory, ScopedBusReceiverServiceFactory>();
        }
    }
}
