using FluentValidation.AspNetCore;
using Infraestructura.Contexto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MSRecursosHumanos.Configurations;
using MSRecursosHumanos.Repo;
using MSRecursosHumanos.Repo.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecursosHumanos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //FluentValidation not working on collection of outer model objects
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddFluentValidation(x => x.ImplicitlyValidateChildProperties = true)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ContractResolver = null;
                    opt.SerializerSettings.Converters.Add(new StringToNumberConverter());
                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            // Contexto de BBDD
            services.AddDbContext<AdventureWorks2016Context>(options => options.UseSqlServer(Configuration.GetConnectionString("AdventureWorks")));

            // Servicios
            services.AddScoped<EmployeeUnit>();
            services.AddScoped<EmployeeService>();
            

            
            //// Validaciones
            //services.AddScoped<IValidator<ProductoDto>, ProductoValidator>();
            //services.AddScoped<IValidator<ProductoOrdenDto>, ProductoOrdenValidator>();
            //services.AddScoped<IValidator<ProductoEstadoDto>, ProductoEstadoValidator>();

            // Versionamiento
            services.AddApiVersioning();

            // Swagger
            services.AddSwaggerGen(swagger =>
            {
                var contact = new OpenApiContact() { Name = SwaggerConfiguration.ContactName, Url = new Uri(SwaggerConfiguration.ContactUrl) };

                swagger.SwaggerDoc(SwaggerConfiguration.DocNameV1, new OpenApiInfo
                {
                    Title = SwaggerConfiguration.DocInfoTitle,
                    Version = SwaggerConfiguration.DocInfoVersion,
                    Description = SwaggerConfiguration.DocInfoDescription,
                    Contact = contact
                });

                // RZevallos - 17/05/2020 - Habilita Bearer Authorization
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = @"JWT Authorization header Example: 'Bearer TOKEN'",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Authorization"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
                swagger.CustomSchemaIds(c => c.FullName);
            });

            // RZevallos - 28/05/2020 - https://stackoverflow.com/questions/53786977/signalr-core-2-2-cors-allowanyorigin-breaking-change
            services.AddCors(options => {
                options.AddPolicy("access", access => access.AllowAnyHeader()
                                                            .AllowAnyMethod()
                                                            .AllowCredentials()
                                                            .SetIsOriginAllowed(_ => true));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("access");

            app.UsePathBase("/MSRecursosHumanos");
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint(SwaggerConfiguration.EndpointUrl, SwaggerConfiguration.EndpointDescription));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
        }
    }
}
