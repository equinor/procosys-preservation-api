using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class VerifyOidInDbMiddleware
    {
        public const string PlantHeader = "x-plant";

        private readonly RequestDelegate _next;

        public VerifyOidInDbMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IHttpContextAccessor httpContextAccessor, PreservationContext preservationContext)
        {
            var oid = httpContextAccessor.HttpContext.User.Claims.TryGetOid().Value;
            var exists = preservationContext.Persons.Any(p => p.Oid.Equals(oid));

            if (!exists)
            {
                var givenName = httpContextAccessor.HttpContext.User.Claims.TryGetGivenName();
                var surName = httpContextAccessor.HttpContext.User.Claims.TryGetSurName();

                if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surName))
                {
                    throw new Exception("Not able to find necessary claims to create user");
                }
                
                var person = new Person(oid, givenName, surName);
                preservationContext.Persons.Add(person);
                preservationContext.SaveChanges();
            }
            
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
