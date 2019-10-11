using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Query;
using Equinor.Procosys.Preservation.WebApi.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Equinor.Procosys.Preservation.WebApi.DiModules
{
    public class MediatorModule
    {
        public static void Load(IServiceCollection services)
        {
            services.AddMediatR(
                typeof(MediatorModule).GetTypeInfo().Assembly,
                typeof(ICommandMarker).GetTypeInfo().Assembly,
                typeof(IQueryMarker).GetTypeInfo().Assembly
            );
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        }
    }
}
