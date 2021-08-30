﻿using Cynthia.Card.Server.Services.GwentGameService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

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
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSignalR();
            services.AddSingleton<GwentServerService>();
            services.AddSingleton<GwentDatabaseService>();
            services.AddSingleton<GwentCardDataService>();
            services.AddSingleton<GwentLocalizationService>();
            services.AddSingleton<CounterService>();
            services.AddSingleton<Random>(x => new Random((int)DateTime.UtcNow.Ticks));
            services.AddAntDesign();
            if (_env.IsDevelopment())
            {
                services.AddTransient<IMongoClient, MongoClient>(x => new MongoClient("mongodb://localhost:28020/gwent-diy"));
            }
            else
            {
                services.AddTransient<IMongoClient, MongoClient>(x => new MongoClient("mongodb://localhost:28020/gwent-diy"));
            }
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
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapHub<GwentHub>("/hub/gwent");
            });
        }
    }
}