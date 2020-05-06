using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;  

namespace Equinor.Procosys.Preservation.WebApi.Swagger
{
    public class AddUploader : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var uploadAttribute = context.ApiDescription.GetUploadAttribute();
            if (uploadAttribute != null)
            {
                operation.Parameters ??= new List<OpenApiParameter>();
                operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = uploadAttribute.Name, Required = true, Type = "file", In = "formData"
                    }
                );
                operation.Consumes.Add("multipart/form-data");
            }
        }
    }
}
