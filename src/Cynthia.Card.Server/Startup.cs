using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Alsein.Utilities;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Cynthia.Card.Server.Services;
using MongoDB.Driver;
using Cynthia.Card.Common;

namespace Cynthia.Card.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
            builder.Populate(services);

            builder.RegisterType<MongoClient>()
                .WithParameter("connectionString", "mongodb://localhost:27017")
                .As<IMongoClient>()
                .PropertiesAutowired()
                .AsSelf();
            builder.RegisterAll("Hub", x => x.ExternallyOwned());//.ExternallyOwned();
            builder.RegisterAllServices(x => x.PreserveExistingDefaults());//.PreserveExistingDefaults();

            ApplicationContainer = builder.Build();
            ApplicationContainer.Resolve<InitializationService>().Start();
            return new AutofacServiceProvider(ApplicationContainer);
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
                app.UseHsts();
            }
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/hub/chat");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
