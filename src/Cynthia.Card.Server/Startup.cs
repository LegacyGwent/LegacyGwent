using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //var container = default(IContainer);
            //var builder = new ContainerBuilder();
            services.AddSignalR();
            services.AddSingleton<GwentServerService>();
            services.AddSingleton<GwentDatabaseService>();
            services.AddSingleton<IMongoClient, MongoClient>((ctx) =>
            {
                return new MongoClient("mongodb://cynthia.ovyno.com:27017/gwent");
            });
            //services.AddAutofac(builder=> 
            //{
            //    builder.Populate(services);
            //    builder.RegisterType<MongoClient>()
            //        .WithParameter("connectionString", "mongodb://cynthia.ovyno.com:27017/gwent")
            //        .As<IMongoClient>()
            //        .PropertiesAutowired()
            //        .AsSelf();
            //    //builder.RegisterAll("Hub", option => option.ExternallyOwned());
            //    builder.RegisterAllServices(option => option.PreserveExistingDefaults());
            //    //builder.Register(x => container).SingleInstance();
            //});
            //container = builder.Build();
            //return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSignalR(routes =>
            {
                routes.MapHub<GwentHub>("/hub/gwent");
            });
        }
    }
}