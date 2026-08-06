using Blazored.LocalStorage;
using Cynthia.Card.Server.Services;
using Cynthia.Card.Server.Services.GwentGameService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Protocol;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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
            services.AddDataProtection();
            services.AddHealthChecks();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR().AddHubOptions<GwentHub>(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(90);
                options.EnableDetailedErrors = _env?.IsDevelopment() == true;
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
            services.AddSingleton<GameFeatureService>();
            services.AddSingleton<CounterService>();
            services.AddSingleton<SiteTextService>();
            services.AddSingleton<SiteSessionService>();
            services.AddSingleton<SeasonOverviewService>();
            services.AddSingleton<Random>(x => new Random((int)DateTime.UtcNow.Ticks));
            services.AddHostedService<ScheduledEventService>();
            services.AddAntDesign();
            services.AddBlazoredLocalStorage();
            services.AddTransient<IMongoClient, MongoClient>(x => new MongoClient(GetConnectionString()));

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("zh-CN"),
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("zh-CN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
                options.RequestCultureProviders.Add(new CustomRequestCultureProvider(context =>
                {
                    var culture = ResolveBrowserCulture(context.Request.Headers["Accept-Language"].ToString());
                    return Task.FromResult(
                        culture == null ? null : new ProviderCultureResult(culture));
                }));
            });
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

            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapGet("/download", context =>
                {
                    context.Response.StatusCode = StatusCodes.Status410Gone;
                    return Task.CompletedTask;
                });
                endpoints.MapPost("/api/GwentData/AwardTrinketToUsers", context =>
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    return Task.CompletedTask;
                });
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<GwentHub>("/hub/gwent");
            });
        }

        private static string ResolveBrowserCulture(string acceptLanguage)
        {
            if (string.IsNullOrWhiteSpace(acceptLanguage))
            {
                return null;
            }

            string selectedCulture = null;
            double selectedQuality = -1;

            foreach (var rawCandidate in acceptLanguage.Split(','))
            {
                var segments = rawCandidate.Split(';');
                var language = segments[0].Trim();
                var quality = 1d;

                for (var index = 1; index < segments.Length; index++)
                {
                    var parameter = segments[index].Trim();
                    if (parameter.StartsWith("q=", StringComparison.OrdinalIgnoreCase))
                    {
                        double.TryParse(
                            parameter.Substring(2),
                            NumberStyles.AllowDecimalPoint,
                            CultureInfo.InvariantCulture,
                            out quality);
                    }
                }

                string supportedCulture = null;
                if (language.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
                {
                    supportedCulture = "zh-CN";
                }
                else if (language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                {
                    supportedCulture = "en-US";
                }

                if (supportedCulture != null && quality > selectedQuality)
                {
                    selectedCulture = supportedCulture;
                    selectedQuality = quality;
                }
            }

            return selectedCulture;
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
