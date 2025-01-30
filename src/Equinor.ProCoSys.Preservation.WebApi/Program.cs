using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Equinor.ProCoSys.Auth;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi;
using Equinor.ProCoSys.Preservation.WebApi.DiModules;
using Equinor.ProCoSys.Preservation.WebApi.DIModules;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Swashbuckle.AspNetCore.SwaggerUI;

const string AllowAllOriginsCorsPolicy = "AllowAllOrigins";

var builder = WebApplication.CreateBuilder(args);

var devOnLocalhost = builder.Configuration.IsDevOnLocalhost();

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

builder.Services.AddSingleton(credential);

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddCors(options =>
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

builder.Services.AddMvc(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
}).AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// this to solve "Multipart body length limit exceeded"
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});

if (builder.Configuration.GetValue<bool>("Application:UseAzureAppConfiguration"))
{
    builder.Services.AddAzureAppConfiguration();
}

builder.Services.AddFluentValidationAutoValidation(fv =>
{
    fv.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssemblies(new List<Assembly>
{
    typeof(IQueryMarker).GetTypeInfo().Assembly,
    typeof(ICommandMarker).GetTypeInfo().Assembly,
});

builder.ConfigureSwagger();

builder.Services.AddPcsAuthIntegration();

builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});
builder.Services.AddMediatrModules();
builder.Services.AddApplicationModules(builder.Configuration);

builder.ConfigureServiceBus();

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Application:UseAzureAppConfiguration"))
{
    app.UseAzureAppConfiguration();
}

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseGlobalExceptionHandling();

app.UseCors(AllowAllOriginsCorsPolicy);

app.UseCompletionSwagger(builder.Configuration);

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

app.MapControllers();

app.Run();

public partial class Program;
