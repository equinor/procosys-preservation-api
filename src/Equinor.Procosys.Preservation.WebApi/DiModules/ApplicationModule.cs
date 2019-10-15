using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Command.EventHandlers;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services)
        {
            services.AddDbContext<PreservationContext>(c =>
            {
                
            });

            services.AddTransient<IReadOnlyContext, PreservationContext>();
            services.AddTransient<IUnitOfWork, PreservationContext>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IEventDispatcher, EventDispatcher>();
            services.AddTransient<ITimeService, TimeService>();
        }
    }
}
