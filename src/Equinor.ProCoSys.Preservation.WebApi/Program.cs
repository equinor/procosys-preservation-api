using System.Collections.Generic;
using System.Reflection;
using Equinor.ProCoSys.Auth;
using Equinor.ProCoSys.Completion.WebApi.DIModules;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi.DiModules;
using Equinor.ProCoSys.Preservation.WebApi.DIModules;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAuthentication(out var credential);

builder.ConfigureAzureAppConfig(credential);

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = null;
});

builder.ConfigureHttp();

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

builder.ConfigureServiceBus(credential);

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

app.UseCors(HttpConfig.AllowAllOriginsCorsPolicy);

app.UseCompletionSwagger(builder.Configuration);

app.UseHttpsRedirection();

app.UseRouting();

// order of adding middlewares are crucial. Some depend on that other has been run in advance
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

public abstract partial class Program;
