using Microsoft.AspNetCore.Builder;

namespace MedicalInstitution.Middleware
{
    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DBMiddleware>();
        }

    }
}
