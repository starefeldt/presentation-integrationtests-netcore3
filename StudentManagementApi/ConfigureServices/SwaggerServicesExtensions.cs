using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;

namespace StudentManagementApi.ConfigureServices
{
    public static class SwaggerServicesExtensions
    {
        private static SwaggerOptions _swaggerOptions;

        public static void AddSwagger(this IServiceCollection services)
        {
            _swaggerOptions = GetSwaggerOptions();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_swaggerOptions.DocumentationVersion, new OpenApiInfo
                {
                    Title = _swaggerOptions.ApiInfoTitle,
                    Contact = new OpenApiContact
                    {
                        Name = _swaggerOptions.ApiInfoContactName,
                        Email = _swaggerOptions.ApiInfoContactEmail
                    },
                    Description = _swaggerOptions.ApiInfoDescription
                });

                options.IncludeXmlComments($@"api-comments.xml", true);
            });
        }

        private static SwaggerOptions GetSwaggerOptions()
        {
            return new SwaggerOptions
            {
                DocumentationVersion = "1",
                ApiInfoTitle = "StudentManagementApp",
                ApiInfoVersion = "1",
                ApiInfoDescription = "Hantering av studenter",
                ApiInfoContactName = "Peter Starefeldt",
                ApiInfoContactEmail = "peter.starefeldt@sogeti.se"
            };
        }

        public static void GetSwaggerUIOptions(this SwaggerUIOptions options)
        {
            if(_swaggerOptions == null)
            {
                throw new ArgumentNullException(nameof(SwaggerOptions), $"Make sure a call to {nameof(AddSwagger)} has been made");
            }

            options.SwaggerEndpoint($"swagger/{_swaggerOptions.DocumentationVersion}/swagger.json",
                                    $"{_swaggerOptions.ApiInfoTitle}");
            options.DocExpansion(DocExpansion.None);
            options.DefaultModelRendering(ModelRendering.Model);
            options.DefaultModelExpandDepth(3);
            options.DocumentTitle = $"{_swaggerOptions.ApiInfoTitle} {_swaggerOptions.ApiInfoVersion}";
            options.RoutePrefix = "";
        }

        private class SwaggerOptions
        {
            public string DocumentationVersion { get; set; }
            public string ApiInfoTitle { get; set; }
            public string ApiInfoVersion { get; set; }
            public string ApiInfoDescription { get; set; }
            public string ApiInfoContactName { get; set; }
            public string ApiInfoContactEmail { get; set; }
        }
    }
}
