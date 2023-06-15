using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Ticket.WebApi.EntityFramework;
using Xtremax.Base.Core.Application;
using Xtremax.Base.Core.Extension;
using Xtremax.Base.Extension;

namespace Ticket.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration.ResolveReferences();
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // Added code

            // Enable Logging to Log4Net (Optional)
            services.AddLogging(loggingBuilder => loggingBuilder.AddXtrLog4Net());

            // Register Base Application

            string applicationAssemblyName = GetType().Assembly.GetName().Name;
            bool useRowNumberForPaging = false;
            // TODO: replace CustomAppDbContext with your AppDbContext
            services.AddXtrApplication<XTicketDbContext>(builder =>
            {
                string connectionString = Configuration.GetConnectionString("Ticket");
                builder.UseSqlServer(connectionString, contextOptionsBuilder =>
                {
                    contextOptionsBuilder.MigrationsAssembly(applicationAssemblyName);
                    contextOptionsBuilder.UseRowNumberForPaging(useRowNumberForPaging);
                });
            },
            options =>
            {
                Configuration.Bind("Authentication:Options", options);
            })
                .AddBaseServices(Configuration, options =>
                {
                    options.ApplicationAssemblyName = applicationAssemblyName;
                    options.UseRowNumberForPaging = useRowNumberForPaging;
                });

            //Added Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ticket Management WebApi",
                    Version = "v1",
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                options.AddSecurityDefinition("ResourceUri", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert ResourceUri into field",
                    Name = "ResourceUri",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ResourceUri"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  Configuration.GetSection("Authentication:JwtBearer").Bind(options);
                  options.RequireHttpsMetadata = false;
              });

            services.ConfigureSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
            });
            //end of added swagger

            services.AddCors();

            // End of added code

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var APP_PREFIX = Configuration.GetSection("APP_PREFIX").Value;
            app.UsePathBase($"{APP_PREFIX}");
			
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "/swagger/{documentName}/swagger.json";
                });

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"{APP_PREFIX}/swagger/v1/swagger.json", "App API V1");
                    options.RoutePrefix = "swagger";
                });
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            //app.UseCors();
			app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            app.UseXtrLoggerMiddleware();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            var xtrAppOptions = app.ApplicationServices.GetRequiredService<XtrApplicationOptions>();
            app.UseEndpoints(endpoints =>
            {
                if (xtrAppOptions.DisableAuthFlag)
                {
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                }
                else
                {
                    endpoints.MapControllers();
                }
            });
        }
    }
}
