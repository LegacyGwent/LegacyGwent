using System.ComponentModel;
using UnityEngine;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;
using System.Linq;
using Alsein.Extensions.LifetimeAnnotations;
using Cynthia.Card.Client;
using Autofac.Extensions.DependencyInjection;
using Alsein.Extensions;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Net;

public class Bootstrapper : MonoBehaviour
{
    public void Awake()
    {
        if (DependencyResolver.Container != null)
            return;
        var IP = Dns.GetHostEntry("cynthia.ovyno.com").AddressList[0];
        var builder = new ContainerBuilder();
        builder.Register(x => DependencyResolver.Container).SingleInstance();
        builder.Register(x => new HubConnectionBuilder().WithUrl($"http://{IP}:5005/hub/gwent").Build()).SingleInstance();
        //builder.Register(x => new HubConnectionBuilder().WithUrl("http://localhost:5005/hub/gwent").Build()).SingleInstance();

        DependencyResolver.Container = AutoRegisterService(builder).Build();
    }

    public ContainerBuilder AutoRegisterService(ContainerBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var services = types.Where(x => x.Name.EndsWith("Service") && x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition);
        // services.Select(x => x.Name).ForAll(Debug.Log);
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(SingletonAttribute))).ToArray()).PropertiesAutowired().AsSelf().SingleInstance();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(ScopedAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerLifetimeScope();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(TransientAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerDependency();
        return builder;
    }
}
