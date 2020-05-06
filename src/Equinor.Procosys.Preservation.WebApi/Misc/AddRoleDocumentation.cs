using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class AddRoleDocumentation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.ApiDescription.GetAuthorizeAttributes();
            if (authorizeAttributes.Any(a => a.GetType() == typeof(AuthorizeAttribute)))
            {
                operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(),
                    new OpenApiResponse
                    {
                        Description = "User is not authenticated"
                    });

                var authorizeAttributesWithRoles = authorizeAttributes
                    .Where(attr => !string.IsNullOrEmpty(attr.Roles))
                    .ToList();

                var response = new OpenApiResponse
                {
                    Description = CreateAuthorizeDescriptionWithRequiredPermissions(authorizeAttributesWithRoles)
                };
                operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), response);
            }
        }

        private static string CreateAuthorizeDescriptionWithRequiredPermissions(List<AuthorizeAttribute> authorizeAttributesWithRoles)
        {
            var description = "User does not have the required permissions ";
            for (var i = 0; i < authorizeAttributesWithRoles.Count; i++)
            {
                if (i > 0)
                {
                    description = AppendAndClause(description);
                }
                var roles = authorizeAttributesWithRoles[i].Roles;
                description = AppendOrClause(description, roles);
            }

            return description;
        }

        private static string AppendOrClause(string description, string roles)
        {
            description += $"({roles.Replace(",", " | ")})";
            return description;
        }

        private static string AppendAndClause(string description)
        {
            description += " & ";
            return description;
        }
    }
}
