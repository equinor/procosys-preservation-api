using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Equinor.ProCoSys.Auth;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Swagger;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi.DIModules;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Equinor.ProCoSys.Preservation.WebApi.Seeding;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Equinor.ProCoSys.Preservation.WebApi
{
    public class Startup
    {
        private const string AllowAllOriginsCorsPolicy = "AllowAllOrigins";
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _environment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_environment.IsDevelopment())
            {
                DebugOptions.DebugEntityFrameworkInDevelopment = Configuration.GetValue<bool>("DebugEntityFrameworkInDevelopment");

                if (Configuration.GetValue<bool>("MigrateDatabase"))
                {
                    services.AddHostedService<DatabaseMigrator>();
                }

                if (Configuration.GetValue<bool>("SeedDummyData"))
                {
                    services.AddHostedService<Seeder>();
                }
            }
            
            var devOnLocalhost = Configuration.IsDevOnLocalhost();
            
            // ChainedTokenCredential iterates through each credential passed to it in order, when running locally
            // DefaultAzureCredential will probably fail locally, so if an instance of Azure Cli is logged in, those credentials will be used
            // If those credentials fail, the next credentials will be those of the current user logged into the local Visual Studio Instance
            // which is also the most likely case
            TokenCredential credential = devOnLocalhost switch
            {
                true
                    => new ChainedTokenCredential(
                        new AzureCliCredential(),
                        new VisualStudioCredential(),
                        new DefaultAzureCredential()
                    ),
                false => new DefaultAzureCredential()
            };
            services.AddSingleton(credential);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    Configuration.Bind("API", options);
                });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsCorsPolicy,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // this to solve "Multipart body length limit exceeded"
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });

            if (Configuration.GetValue<bool>("Application:UseAzureAppConfiguration"))
            {
                services.AddAzureAppConfiguration();
            }

            services.AddFluentValidationAutoValidation(fv =>
            {
                fv.DisableDataAnnotationsValidation = true;
            });
            services.AddValidatorsFromAssemblies(new List<Assembly>
            {
                typeof(IQueryMarker).GetTypeInfo().Assembly,
                typeof(ICommandMarker).GetTypeInfo().Assembly,
                typeof(Startup).Assembly
            });

            var scopes = Configuration.GetSection("Swagger:Scopes")?.Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProCoSys Preservation API", Version = "v1" });

                //Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["Swagger:AuthorizationUrl"]),
                            Scopes = scopes
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        scopes.Keys.ToArray()
                    }
                });

                c.OperationFilter<AddRoleDocumentation>();
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
            });

            services.AddFluentValidationRulesToSwagger();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddPcsAuthIntegration();

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = Configuration["ApplicationInsights:ConnectionString"];
            });
            services.AddMediatrModules();
            services.AddApplicationModules(Configuration);

            var serviceBusEnabled = Configuration.GetValue<bool>("ServiceBus:Enable") && 
                (!_environment.IsDevelopment() || Configuration.GetValue<bool>("ServiceBus:EnableInDevelopment"));
            if (serviceBusEnabled)
            {
                // Env variable used in radix. Configuration is added for easier use locally
                // Url will be validated during startup of service bus integration and give a
                // Uri exception if invalid.
             
                var leaderElectorUrl = Environment.GetEnvironmentVariable("LEADERELECTOR_SERVICE") ?? ( Configuration["ServiceBus:LeaderElectorUrl"] + ":3003");

                services.AddPcsServiceBusIntegration(options => options
                    .UseBusConnection(Configuration.GetConnectionString("ServiceBus"))
                    .WithLeaderElector(leaderElectorUrl)
                    .WithRenewLeaseInterval(int.Parse(Configuration["ServiceBus:LeaderElectorRenewLeaseInterval"]))
                    .WithSubscription(PcsTopicConstants.Tag, "preservation_tag")
                    .WithSubscription(PcsTopicConstants.TagFunction, "preservation_tagfunction")
                    .WithSubscription(PcsTopicConstants.Project, "preservation_project")
                    .WithSubscription(PcsTopicConstants.CommPkg, "preservation_commpkg")
                    .WithSubscription(PcsTopicConstants.McPkg, "preservation_mcpkg")
                    .WithSubscription(PcsTopicConstants.Responsible, "preservation_responsible")
                    .WithSubscription(PcsTopicConstants.Certificate, "preservation_certificate")
                    //THIS METHOD SHOULD BE FALSE IN NORMAL OPERATION.
                    //ONLY SET TO TRUE WHEN A LARGE NUMBER OF MESSAGES HAVE FAILED AND ARE COPIED TO DEAD LETTER.
                    //WHEN SET TO TRUE, MESSAGES ARE READ FROM DEAD LETTER QUEUE INSTEAD OF NORMAL QUEUE
                    .WithReadFromDeadLetterQueue(Configuration.GetValue("ServiceBus:ReadFromDeadLetterQueue", defaultValue: false))); 
                
            }
            services.AddHostedService<VerifyApplicationExistsAsPerson>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Configuration.GetValue<bool>("Application:UseAzureAppConfiguration"))
            {
                app.UseAzureAppConfiguration();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGlobalExceptionHandling();

            app.UseCors(AllowAllOriginsCorsPolicy);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProCoSys Preservation API V1");
                c.DocExpansion(DocExpansion.List);
                c.DisplayRequestDuration();

                c.OAuthClientId(Configuration["Swagger:ClientId"]);
                c.OAuthAppName("ProCoSys Preservation API V1");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "resource", Configuration["API:Audience"] } });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // order of adding middelwares are crucial. Some depend that other has been run in advance
            app.UseCurrentPlant();
            app.UseAuthentication();
            app.UseCurrentUser();
            app.UsePersonValidator();
            app.UsePlantValidator();
            app.UseVerifyOidInDb();
            app.UseAuthorization();
            
            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
