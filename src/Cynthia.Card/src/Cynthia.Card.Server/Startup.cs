using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<GwentServerService>();
            services.AddSingleton<GwentDatabaseService>();
            services.AddSingleton<GwentCardTypeService>();
            services.AddSingleton<Random>(x => new Random((int)DateTime.UtcNow.Ticks));
            services.AddSingleton<IMongoClient, MongoClient>((ctx) =>
            {
                return new MongoClient("mongodb://cynthia.ovyno.com:27017/gwent");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // app.UseSignalR(routes =>
            // {
            //     routes.MapHub<GwentHub>("/hub/gwent");
            // });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GwentHub>("/hub/gwent");
            });
        }
    }
}