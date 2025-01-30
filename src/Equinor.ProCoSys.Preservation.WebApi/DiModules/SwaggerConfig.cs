using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Common.Swagger;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class SwaggerConfig
{
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        var scopes = builder.GetSwaggerScopes();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProCoSys Preservation API", Version = "v1" });

            //Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
            c.AddSecurityDefinition("oauth2",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(builder.GetConfig<string>("Swagger:AuthorizationUrl")),
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

        builder.Services.ConfigureSwaggerGen(options =>
        {
            options.CustomSchemaIds(x => x.FullName);
        });

        builder.Services.AddFluentValidationRulesToSwagger();

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
    }
    
    public static void UseCompletionSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProCoSys Preservation API V1");
            c.DocExpansion(DocExpansion.List);
            c.DisplayRequestDuration();

            c.OAuthClientId(configuration["Swagger:ClientId"]);
            c.OAuthAppName("ProCoSys Preservation API V1");
            c.OAuthScopeSeparator(" ");
            c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "resource", configuration["Swagger:Audience"] } });
        });
    }
    
    private static Dictionary<string, string> GetSwaggerScopes(this WebApplicationBuilder builder)
    {
        var scopes = builder.Configuration.GetSection("Swagger:Scopes").Get<Dictionary<string, string>>();
        if (scopes != null)
        {
            return scopes;
        }

        return new Dictionary<string, string>();
    }
}
