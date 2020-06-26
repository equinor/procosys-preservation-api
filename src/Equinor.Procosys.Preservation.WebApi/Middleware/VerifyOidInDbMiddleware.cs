using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.WebApi.Middleware
{
    public class VerifyOidInDbMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyOidInDbMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, PreservationContext preservationContext, ICurrentUserProvider currentUserProvider)
        {
            var exists = preservationContext.Persons.Any(p => p.Oid.Equals(currentUserProvider.GetCurrentUserOid()));

            if (!exists)
            {
                var givenName = context.User.Claims.TryGetGivenName();
                var surName = context.User.Claims.TryGetSurName();

                if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surName))
                {
                    throw new Exception("Not able to find necessary claims to create user");
                }

                var person = new Person(currentUserProvider.GetCurrentUserOid(), givenName, surName);

                try
                {
                    preservationContext.Persons.Add(person);
                    preservationContext.SaveChanges();
                    
                }
                catch (Exception)
                {
                    // We have to do this silently as concurrency is a very likely problem.
                    // For a user accessing preservation for the first time, there will probably be multiple
                    // requests in parallel.
                    preservationContext.Entry(person).State = EntityState.Detached;
                }
            }
            
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
