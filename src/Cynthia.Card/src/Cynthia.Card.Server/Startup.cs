using Cynthia.Card.Server.Services.GwentGameService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Protocol;
using MongoDB.Driver;
using Blazored.LocalStorage;
using System;
using System.Linq;

namespace Cynthia.Card.Server
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        public Startup(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR().AddHubOptions<GwentHub>(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(90);
            });
            // Normalize at the protocol boundary because payload converters cannot reach
            // target, invocation ID, and error text. Replacing only the framework JSON
            // protocol keeps Blazor's protocol and avoids double-encoding payload strings.
            var frameworkJsonProtocol = services.Single(descriptor =>
                descriptor.ServiceType == typeof(IHubProtocol) &&
                descriptor.ImplementationType == typeof(JsonHubProtocol));
            services.Remove(frameworkJsonProtocol);
            services.AddSingleton<IHubProtocol, AsciiSafeJsonHubProtocol>();
            services.AddSingleton<GwentServerService>();
            services.AddSingleton<GwentDatabaseService>();
            services.AddSingleton<GwentCardDataService>();
            services.AddSingleton<GwentLocalizationService>();
            services.AddSingleton<CounterService>();
            services.AddSingleton<Random>(x => new Random((int)DateTime.UtcNow.Ticks));
            // Add the scheduled event service
            services.AddHostedService<ScheduledEventService>();
            services.AddAntDesign();
            services.AddBlazoredLocalStorage();
            services.AddTransient<IMongoClient, MongoClient>(x => new MongoClient(GetConnectionString()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<GwentHub>("/hub/gwent");
            });
        }

        private string GetConnectionString()
        {
            string variable = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            Console.WriteLine(variable);
            if (string.IsNullOrEmpty(variable))
            {
                return "mongodb://localhost:28020/gwent-diy";
            }
            return variable;
        }
    }
}
