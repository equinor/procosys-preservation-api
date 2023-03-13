using System.Reflection;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.Preservation.WebApi.DIModules
{
    public static class MediatorModule
    {
        public static void AddMediatrModules(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                    typeof(ICommandMarker).GetTypeInfo().Assembly,
                    typeof(MediatorModule).GetTypeInfo().Assembly,
                    typeof(IQueryMarker).GetTypeInfo().Assembly
                )
            );
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CheckValidProjectBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CheckAccessBehavior<,>));
        }
    }
}
