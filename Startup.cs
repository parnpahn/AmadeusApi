﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationApi.Models;
using ReservationApi.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options => options.UseMemberCasing());

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


            // requires using Microsoft.Extensions.Options
            services.Configure<ReservationDataBaseSettings>(
                Configuration.GetSection(nameof(ReservationDataBaseSettings)));

            services.AddSingleton<IReservationDataSettings>(sp =>
                sp.GetRequiredService<IOptions<ReservationDataBaseSettings>>().Value);

            services.AddSingleton<ReservationService>();

           

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
            app.UseMvc();
        }
    }
}
