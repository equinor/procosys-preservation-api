using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public static class ValidatorRegistrator
    {
        public static void AddGenericValidator(this IServiceCollection services, Type genericType, Type validatorType, Assembly assembly)
        {
            var abstractValidatorType = typeof(IValidator<>);
            foreach (var implementationType in assembly.ExportedTypes.Where(type => !type.IsAbstract && !type.IsInterface && genericType.IsAssignableFrom(type)))
            {
                var genericValidatorType = abstractValidatorType.MakeGenericType(implementationType);
                services.AddTransient(genericValidatorType, validatorType);
            }
        }
    }
}
