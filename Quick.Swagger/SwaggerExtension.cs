using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

namespace Quick.SwaggerWidthApiVersion;

public static class SwaggerCustomExtension
{
    private static List<OpenApiInfo> _OpenApiInfos = new List<OpenApiInfo>();

    /// <summary>
    /// Add quick swaggle and Api version
    /// sample controller Attribute
    /// [ApiVersion("2.0", Deprecated = false)] 
    /// [Route("/api/v{version:apiVersion}/[controller]")]
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="title">title of application api or custom title for swagger</param>
    /// <param name="countVersion">number of version created</param>
    /// <returns>services</returns>
    public static IServiceCollection AddQuickSwaggerWidthApiVersion(this IServiceCollection services, string title, int countVersion)
    {
        for (int i = 1; i <= countVersion; i++)
        {
            var version = $"v{i}";
            _OpenApiInfos.Add(new OpenApiInfo { Title = title, Version = version });
        }

        return services.AddQuickSwaggerWidthApiVersion(_OpenApiInfos);
    }

    /// <summary>
    /// Add quick swaggle and Api version
    /// sample controller Attribute
    /// [ApiVersion("2.0", Deprecated = false)] 
    /// [Route("/api/v{version:apiVersion}/[controller]")]
    /// </summary>
    /// <param name="services">service</param>
    /// <param name="openApiInfos">List of api version info</param>
    /// <returns></returns>
    public static IServiceCollection AddQuickSwaggerWidthApiVersion(this IServiceCollection services, List<OpenApiInfo> openApiInfos)
    {
        if (openApiInfos?.Count > 0)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                foreach (var item in openApiInfos)
                {
                    c.SwaggerDoc(item.Version, item);
                }

                c.OperationFilter<RemoveVersionParameterFilter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                c.EnableAnnotations();

                //First we define the security scheme
                c.AddSecurityDefinition("Bearer", //Name the security scheme
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme = JwtBearerDefaults.AuthenticationScheme //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                    });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = JwtBearerDefaults.AuthenticationScheme, //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                    });

            });

            AddApiVersion(services);
        }
        else
        {
            System.Diagnostics.Debug.Write("Error no definition OpenApiInfo");
        }

        return services;
    }

    /// <summary>
    /// Add Api version  o.GroupNameFormat = "'v'VVV";
    /// </summary>
    /// <param name="services"></param>
    private static void AddApiVersion(IServiceCollection services)
    {
        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = true;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new HeaderApiVersionReader();
        });
        services.AddVersionedApiExplorer(o =>
        {
            o.GroupNameFormat = "'v'VVV";
            o.SubstituteApiVersionInUrl = true;
        });
    }

    /// <summary>
    /// Use Quick Swagger 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication AddQuickUseSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() && _OpenApiInfos?.Count>0)
        {
            app.Logger.LogInformation($"Mode Development");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var item in _OpenApiInfos)
                {
                    c.SwaggerEndpoint($"/swagger/{item.Version}/swagger.json", $"{item.Title} {item.Version}");
                }

            });

            //bonus
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }
      
        return app;
    }
}