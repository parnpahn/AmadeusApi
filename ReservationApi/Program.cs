﻿using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ReservationApi
{
    public class Program
    {


        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(System.IO.Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
           .AddEnvironmentVariables()

           .Build();

        public static void Main(string[] args)
        {




            var _appInsightConfiguration = new TelemetryConfiguration() { InstrumentationKey = Configuration["InstrumentationKey"] };
                Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               //.WriteTo.MSSqlServer(Configuration.GetConnectionString("LogConnection"), "_logs", columnOptions: columnOptions)
               .Enrich.FromLogContext()
               // Log to Console provider
               .WriteTo.Console()
               //Log to Application Insights Provider 
               //App Insights configuration and Converter Type set to Event 
               .WriteTo.ApplicationInsights(_appInsightConfiguration, TelemetryConverter.Events)
               .CreateLogger();


                // Serilog Log Info 
                Log.Information($"Application started using Serilog for logging{DateTime.Now} UTC {DateTime.UtcNow}");
           
        



            //Session 2
           

            CreateWebHostBuilder(args).Build().Run();
          
        }


        //WebHost and Generic Host Explained 
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {

                    //Toggle for development and production 
                    if (context.HostingEnvironment.IsProduction())
                    {
                        var builtConfig = config.Build();

                        using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                        {
                            store.Open(OpenFlags.ReadOnly);
                            var certs = store.Certificates
                                .Find(X509FindType.FindByThumbprint,
                                    builtConfig["AzureADCertThumbprint"], false);

                            config.AddAzureKeyVault(
                                $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                                builtConfig["AzureADApplicationId"],
                                certs.OfType<X509Certificate2>().Single());

                            store.Close();

                        }
                    }
                })
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
