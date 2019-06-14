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
    public InputField TestText;
    public void Awake()
    {
        //throw new Exception("!!!");
        var IP = Dns.GetHostEntry("cynthia.ovyno.com").AddressList[0];
        if (DependencyResolver.Container != null)
            return;
        var builder = new ContainerBuilder();
        builder.Register(x => DependencyResolver.Container).SingleInstance();
        builder.RegisterType<HubConnectionBuilder>().SingleInstance();
        //builder.Register(x => DependencyResolver.Container.Resolve<HubConnectionBuilder>().WithUrl($"http://{IP}:5000/hub/gwent").Build()).SingleInstance();
        builder.Register(x => DependencyResolver.Container.Resolve<HubConnectionBuilder>().WithUrl("http://localhost:5000/hub/gwent").Build()).SingleInstance();
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var services = types.Where(x => x.Name.EndsWith("Service") && x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition);
        //services.Select(x => x.Name).ForAll(Debug.Log);
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(SingletonAttribute))).ToArray()).PropertiesAutowired().AsSelf().SingleInstance();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(ScopedAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerLifetimeScope();
        builder.RegisterTypes(services.Where(x => x.IsDefined(typeof(TransientAttribute))).ToArray()).PropertiesAutowired().AsSelf().InstancePerDependency();
        DependencyResolver.Container = builder.Build();
        //await DependencyResolver.Container.Resolve<HubConnection>().StartAsync();
        ////-------------------------------------------------------------------------------------------------------------------------------
        //DependencyResolver.Container.Resolve<HubConnection>().Closed += (async x =>
        //{
        //    Debug.Log("断线!");
        //    var user = DependencyResolver.Container.Resolve<GwentClientService>().User;
        //    await Task.Delay(100);
        //    await DependencyResolver.Container.Resolve<HubConnection>().StartAsync();
        //    var result = await DependencyResolver.Container.Resolve<HubConnection>().InvokeAsync<bool>("Reconnect", user.UserName, user.PassWord);
        //    if (result) Debug.Log("尝试重连成功");
        //    else
        //    {
        //        SceneManager.LoadScene("LoginSecen");
        //        await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("断开连接", "请尝试重新登陆");
        //    }
        //});
        ////-------------------------
        //DependencyResolver.Container.Resolve<HubConnection>().On("RepeatLogin",async () => 
        //{
        //    SceneManager.LoadScene("LoginSecen");
        //    await DependencyResolver.Container.Resolve<GlobalUIService>().YNMessageBox("账号被其他人强制登陆", "账号被登陆,被挤下了线");
        //});
        //在启动时就链接上服务器
    }
}
