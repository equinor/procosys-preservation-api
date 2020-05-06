using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Equinor.Procosys.Preservation.WebApi.Swagger
{
    public static class ApiDescriptionExtension
    {
        public static AuthorizeAttribute[] GetAuthorizeAttributes(this ApiDescription apiDescription)
        {
            var hasAuthAttribute = apiDescription.ActionDescriptor.EndpointMetadata
                .Where(i => i.GetType() == typeof(AuthorizeAttribute) || i.GetType().BaseType == typeof(AuthorizeAttribute))
                .Cast<AuthorizeAttribute>();

            return hasAuthAttribute as AuthorizeAttribute[] ?? hasAuthAttribute.ToArray();
        }

        //public static UploadAttribute GetUploadAttribute(this ApiDescription apiDescription)
        //    => apiDescription.ActionDescriptor.EndpointMetadata.SingleOrDefault(i => i is UploadAttribute) as UploadAttribute;
    }
}
