using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Alsein.Utilities;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using MongoDB.Driver;

namespace Cynthia.Card.Server
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = default(IContainer);
            var builder = new ContainerBuilder();
            services.AddSignalR();
            builder.Populate(services);
            builder.RegisterType<MongoClient>()
                .WithParameter("connectionString", "mongodb://cynthia.ovyno.com:27017/gwent")
                .As<IMongoClient>()
                .PropertiesAutowired()
                .AsSelf();
            builder.RegisterAll("Hub", option => option.ExternallyOwned());
            builder.RegisterAllServices(option => option.PreserveExistingDefaults());
            builder.Register(x => container).SingleInstance();
            container = builder.Build();
            //container.Resolve<InitializationService>().Start();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSignalR(routes =>
            {
                routes.MapHub<GwentHub>("/hub/gwent");
                routes.MapHub<ChatHub>("/chathub");
            });
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
