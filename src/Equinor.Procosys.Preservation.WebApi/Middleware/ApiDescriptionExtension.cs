using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public static class ApiDescriptionExtension
    {
        public static AuthorizeAttribute[] GetAuthorizeAttributes(this ApiDescription apiDescription)
        {
            var hasAuthAttribute = apiDescription.ActionDescriptor.EndpointMetadata
                //.Select(filterInfo => filterInfo.Instance)
                .Where(i => i.GetType() == typeof(AuthorizeAttribute) || i.GetType().BaseType == typeof(AuthorizeAttribute))
                .Cast<AuthorizeAttribute>();

            return hasAuthAttribute as AuthorizeAttribute[] ?? hasAuthAttribute.ToArray();
        }
    }
}
