﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservationApi.Data.EFCore.DependencyInjection;
using ReservationApi.Data.MongoDb.DependencyInjection;
using ReservationApi.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReservationApi
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
            //Session 1
            services.AddMvc(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                //XML Formatter
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options => options.UseMemberCasing());

            services.AddSwaggerGen(options =>
            {

                //Swagger Documentation option
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Reservation Api",
                    Version = "v1",
                    Description = "Amadeus Api for Training",
                    Contact = new Contact
                    {
                        Email = "timothy.oleson@microsoft.com",
                        Name = "Tim Oleson",
                        Url = "https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.2"
                    },
                    License = new License
                    {
                        Name = "MIT License",
                        Url = "https://opensource.org/licenses/MIT"
                    }
                });

                //Include XML comments in you Api Documentation 
                // Open Project Properties under Build Tab in Output section check xml documentation file change value to ReservationApi
                //Use Reflection to file name 
                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                //use full path
                options.IncludeXmlComments(xmlCommentsFullPath);

                options.AddSecurityDefinition("Bearer",
                  new ApiKeyScheme
                  {
                      In = "header",
                      Description = "Please enter into field the word 'Bearer' following by space and JWT",
                      Name = "Authorization",
                      Type = "apiKey"
                  });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                { "Bearer", Enumerable.Empty<string>() },
             });
            });


            //Session 3
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 // base-address of your identityserver
                 options.Authority = "https://localhost:5001/";

                 // name of the API resource
                 options.Audience = "reservationapi";
             });

            //Session 1
            //The configuration instance to which the appsettings.json file's BookstoreDatabaseSettings section binds is 
            //registered in the Dependency Injection (DI) container. For example, a BookstoreDatabaseSettings object's 
            //ConnectionString property is populated with the BookstoreDatabaseSettings:ConnectionString property in 
            //appsettings.json.
            //The IBookstoreDatabaseSettings interface is registered in DI with a singleton service lifetime.When 
            //injected, the interface instance resolves to a BookstoreDatabaseSettings object.

            //configure mongodb connection and register an instance of IMongoCollection for use in service

            services.AddScoped<ReservationService>();

            //services.AddEFCoreDatabaseSupport(Configuration);
            services.AddMongoDatabaseSupport(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Session 3
            app.UseAuthentication();
            app.UseHttpsRedirection();

            //Session 2
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reservation Api");

                //c.DocExpansion(DocExpansion.None);

                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}