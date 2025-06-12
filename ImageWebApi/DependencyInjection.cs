using Microsoft.OpenApi.Models;

namespace ImageWebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentationLayer(this IServiceCollection services)
        {
            // To allow API Documentation
            services.AddApiDocumentationServices();
            // To allow API Controllers
            services.AddControllers();
            services.AddProblemDetails();
            // To allow MediatR for CQRS pattern
            return services;
        }
        private static IServiceCollection AddApiDocumentationServices(this IServiceCollection services)
        {
            // Swagger API for documenttion
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ImageApp", Version = "v1" });
            });
            // For documentation (.NET 9): Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi("v1_development");
            return services;
        }
    }
}
