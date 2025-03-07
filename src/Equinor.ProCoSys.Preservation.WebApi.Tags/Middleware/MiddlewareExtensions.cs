﻿using Microsoft.AspNetCore.Builder;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Middleware
{
    public static class MiddlewareExtensions
    {
        public static void UseGlobalExceptionHandling(this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionHandler>();
        public static void UseCurrentUser(this IApplicationBuilder app) => app.UseMiddleware<CurrentUserMiddleware>();
        public static void UseVerifyOidInDb(this IApplicationBuilder app) => app.UseMiddleware<VerifyOidInDbMiddleware>();
        public static void UseCurrentPlant(this IApplicationBuilder app) => app.UseMiddleware<CurrentPlantMiddleware>();
        public static void UsePersonValidator(this IApplicationBuilder app) => app.UseMiddleware<PersonValidatorMiddleware>();
        public static void UsePlantValidator(this IApplicationBuilder app) => app.UseMiddleware<PlantValidatorMiddleware>();
    }
}
