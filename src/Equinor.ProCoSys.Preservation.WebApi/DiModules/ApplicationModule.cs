﻿using System.Text.Json.Serialization;
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
            services.AddTransient<IDomainToIntegrationEventConverter<ChildEntityAddedEvent<Project, Tag>>, ProjectTagAddedEventConverter>();
            
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Tag>>, IntegrationEventHandler<PlantEntityModifiedEvent<Tag>, Tag>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<Tag>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<Tag>, Tag>>();
            services.AddTransient<IPublishEntityEventHelper<Tag>, PublishEntityEventHelper<Tag, TagEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Tag>, PublishDeleteEntityEventHelper<Tag, TagDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Tag, TagEvent>, CreateTagEventHelper>();
            services.AddTransient<ICreateEventHelper<Tag, TagDeleteEvent>, CreateTagDeleteEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Project, Tag, TagEvent>, CreateProjectTagEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<TagRequirement>>, IntegrationEventHandler<PlantEntityModifiedEvent<TagRequirement>, TagRequirement>>();
            services.AddTransient<INotificationHandler<TagRequirementDeletedEvent>, DeleteTagRequirementEventHandler>();
            services.AddTransient<IPublishEntityEventHelper<TagRequirement>, PublishEntityEventHelper<TagRequirement, TagRequirementEvent>>();
            services.AddTransient<ICreateEventHelper<TagRequirement, TagRequirementEvent>, CreateTagRequirementEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent>, CreateProjectTagRequirementEventHelper>();
            
            services.AddTransient<INotificationHandler<ChildEntityAddedEvent<RequirementType, RequirementDefinition>>, ChildEntityAddedEventHandler<RequirementType, RequirementDefinition, RequirementDefinitionEvent>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<RequirementDefinition>>, IntegrationEventHandler<PlantEntityModifiedEvent<RequirementDefinition>, RequirementDefinition>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<RequirementDefinition>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<RequirementDefinition>, RequirementDefinition>>();
            services.AddTransient<IPublishEntityEventHelper<RequirementDefinition>, PublishEntityEventHelper<RequirementDefinition, RequirementDefinitionEvent>>();
            services.AddTransient<ICreateEventHelper<RequirementDefinition, RequirementDefinitionEvent>, CreateRequirementDefinitionEventHelper>();
            services.AddTransient<ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>, CreateRequirementTypeRequirementDefinitionEventHelper>();
            services.AddTransient<ICreateEventHelper<RequirementDefinition, RequirementDefinitionDeleteEvent>, CreateRequirementDefinitionDeletedEventHelper>();
            services.AddTransient<IPublishDeleteEntityEventHelper<RequirementDefinition>, PublishDeleteEntityEventHelper<RequirementDefinition, RequirementDefinitionDeleteEvent>>();

            services.AddTransient<INotificationHandler<ChildEntityAddedEvent<Tag, Action>>, ChildEntityAddedEventHandler<Tag, Action, ActionEvent>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Action>>, IntegrationEventHandler<PlantEntityModifiedEvent<Action>, Action>>();
            services.AddTransient<IPublishEntityEventHelper<Action>, PublishEntityEventHelper<Action, ActionEvent>>();
            services.AddTransient<ICreateEventHelper<Action, ActionEvent>, CreateActionEventHelper>();
            services.AddTransient<ICreateChildEventHelper<Tag, Action, ActionEvent>, CreateTagActionEventHelper>();
            
            services.AddTransient<INotificationHandler<ChildEntityAddedEvent<RequirementDefinition, Field>>, ChildEntityAddedEventHandler<RequirementDefinition, Field, FieldEvent>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Field>>, IntegrationEventHandler<PlantEntityModifiedEvent<Field>, Field>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Field>>, IntegrationDeleteEventHandler<PlantEntityModifiedEvent<Field>, Field>>();
            services.AddTransient<IPublishEntityEventHelper<Field>, PublishEntityEventHelper<Field, FieldEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Field>, PublishDeleteEntityEventHelper<Field, RequirementFieldDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Field, FieldEvent>, CreateFieldEventHelper>();
            services.AddTransient<ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent>, CreateRequirementDefinitionFieldEventHelper>();
            services.AddTransient<ICreateEventHelper<Field, RequirementFieldDeleteEvent>, CreateRequirementFieldDeleteEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityCreatedEvent<Journey>>, IntegrationEventHandler<PlantEntityCreatedEvent<Journey>, Journey>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Journey>>, IntegrationEventHandler<PlantEntityModifiedEvent<Journey>, Journey>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<Journey>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<Journey>, Journey>>();
            services.AddTransient<IPublishEntityEventHelper<Journey>, PublishEntityEventHelper<Journey, JourneyEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Journey>, PublishDeleteEntityEventHelper<Journey, JourneyDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Journey, JourneyEvent>, CreateJourneyEventHelper>();
            services.AddTransient<ICreateEventHelper<Journey, JourneyDeleteEvent>, CreateJourneyDeletedEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityCreatedEvent<Mode>>, IntegrationEventHandler<PlantEntityCreatedEvent<Mode>, Mode>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Mode>>, IntegrationEventHandler<PlantEntityModifiedEvent<Mode>, Mode>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<Mode>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<Mode>, Mode>>();
            services.AddTransient<IPublishEntityEventHelper<Mode>, PublishEntityEventHelper<Mode, ModeEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Mode>, PublishDeleteEntityEventHelper<Mode, ModeDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Mode, ModeEvent>, CreateModeEventHelper>();
            services.AddTransient<ICreateEventHelper<Mode, ModeDeleteEvent>, CreateModeDeletedEventHelper>();
            
            services.AddTransient<INotificationHandler<ChildEntityAddedEvent<TagRequirement, PreservationPeriod>>, ChildEntityAddedEventHandler<TagRequirement, PreservationPeriod, PreservationPeriodsEvent>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<PreservationPeriod>>, IntegrationEventHandler<PlantEntityModifiedEvent<PreservationPeriod>, PreservationPeriod>>();
            services.AddTransient<IPublishEntityEventHelper<PreservationPeriod>, PublishEntityEventHelper<PreservationPeriod, PreservationPeriodsEvent>>();
            services.AddTransient<ICreateChildEventHelper<TagRequirement, PreservationPeriod, PreservationPeriodsEvent>, CreateTagRequirementPreservationPeriodEventHelper>();
            services.AddTransient<ICreateEventHelper<PreservationPeriod, PreservationPeriodsEvent>, CreatePreservationPeriodEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityCreatedEvent<RequirementType>>, IntegrationEventHandler<PlantEntityCreatedEvent<RequirementType>, RequirementType>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<RequirementType>>, IntegrationEventHandler<PlantEntityModifiedEvent<RequirementType>, RequirementType>>();
            services.AddTransient<IPublishEntityEventHelper<RequirementType>, PublishEntityEventHelper<RequirementType, RequirementTypeEvent>>();
            services.AddTransient<ICreateEventHelper<RequirementType, RequirementTypeEvent>, CreateRequirementTypeEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityCreatedEvent<Responsible>>, IntegrationEventHandler<PlantEntityCreatedEvent<Responsible>, Responsible>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Responsible>>, IntegrationEventHandler<PlantEntityModifiedEvent<Responsible>, Responsible>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<Responsible>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<Responsible>, Responsible>>();
            services.AddTransient<IPublishEntityEventHelper<Responsible>, PublishEntityEventHelper<Responsible, ResponsibleEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Responsible>, PublishDeleteEntityEventHelper<Responsible, ResponsibleDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Responsible, ResponsibleEvent>, CreateResponsibleEventHelper>();
            services.AddTransient<ICreateEventHelper<Responsible, ResponsibleDeleteEvent>, CreateResponsibleDeleteEventHelper>();
            
            services.AddTransient<INotificationHandler<PlantEntityCreatedEvent<Step>>, IntegrationEventHandler<PlantEntityCreatedEvent<Step>, Step>>();
            services.AddTransient<INotificationHandler<PlantEntityModifiedEvent<Step>>, IntegrationEventHandler<PlantEntityModifiedEvent<Step>, Step>>();
            services.AddTransient<INotificationHandler<PlantEntityDeletedEvent<Step>>, IntegrationDeleteEventHandler<PlantEntityDeletedEvent<Step>, Step>>();
            services.AddTransient<IPublishEntityEventHelper<Step>, PublishEntityEventHelper<Step, StepEvent>>();
            services.AddTransient<IPublishDeleteEntityEventHelper<Step>, PublishDeleteEntityEventHelper<Step, StepDeleteEvent>>();
            services.AddTransient<ICreateEventHelper<Step, StepEvent>, CreateStepEventHelper>();
            services.AddTransient<ICreateEventHelper<Step, StepDeleteEvent>, CreateStepDeleteEventHelper>();
            
            services.AddTransient<IIntegrationEventPublisher, IntegrationEventPublisher>();

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
