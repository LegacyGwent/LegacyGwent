using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public void Awake()
    {
        if (DependencyResolver.Container != null)
            return;
        var IP = Dns.GetHostEntry("cynthia.ovyno.com").AddressList[0];
        var builder = new ContainerBuilder();
        builder.Register(x => DependencyResolver.Container).SingleInstance();
        builder.Register(
            x => new HubConnectionBuilder().WithUrl($"http://{IP}:5005/hub/gwent", HttpTransportType.WebSockets, options => { options.SkipNegotiation = true; })
                    .AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new BoolConverter()))
                    .AddJsonProtocol(options => options.PayloadSerializerOptions.Converters.Add(new ListOperationConverter()))
                    .Build()
                    ).Named<HubConnection>("game").SingleInstance();
        //builder.Register(x => new HubConnectionBuilder().WithUrl("http://localhost:5005/hub/gwent").Build()).Named<HubConnection>("game").SingleInstance();

        DependencyResolver.Container = AutoRegisterService(builder).Build();
    }

    public ContainerBuilder AutoRegisterService(ContainerBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var services = types.Where(x => x.Name.EndsWith("Service") && x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition);
        // services.Select(x => x.Name).ForAll(Debug.Log);//显示注入的服务都有哪些
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(SingletonAttribute))).ToArray()).PropertiesAutowired().AsSelf().SingleInstance();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(ScopedAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerLifetimeScope();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(TransientAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerDependency();

        return builder;
    }
}
