using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Autofac;
using System;
using Microsoft.AspNetCore.SignalR.Client;

public class RegisterClick : MonoBehaviour
{
    private static bool IsRegistering = false;
    public InputField Username;
    public InputField Playername;
    public InputField Password;
    public InputField Password2;
    public Text RegisterMessage;
    public Text LoginMessage;

    private GwentClientService server;

    private void Start()
    {
        if (server != null)
            return;
        server = DependencyResolver.Container.Resolve<GwentClientService>();
    }
    public async void Register()
    {
        if (IsRegistering) return;
        IsRegistering = true;
        if (Password.text.Length == 0 || Username.text.Length == 0 || Playername.text.Length == 0)
        {
            RegisterMessage.text = "输入不能为空..请重新输入";
            IsRegistering = false;
            return;
        }
        if (Password.text != Password2.text)
        {
            RegisterMessage.text = "两次密码输入不一致,请重新输入";
            IsRegistering = false;
            return;
        }
        RegisterMessage.text = "正在注册...请稍等片刻";
        try
        {
            var hub = DependencyResolver.Container.Resolve<HubConnection>();
            if (hub.State == HubConnectionState.Disconnected)
                await hub.StartAsync();
            var result = await server.Register(Username.text, Password.text, Playername.text);
            if (!result)
            {
                RegisterMessage.text = "注册失败,该用户名或该游戏名已经存在,尝试更换后重新注册";
                IsRegistering = false;
                return;
            }
            IsRegistering = false;
            RegisterMessage.text = "注册成功~点击登陆切换到登录页面进行登陆~";
        }
        catch(Exception e)
        {
            RegisterMessage.text = "发生异常,原因或许是服务器未开启,尝试重试或者联系作者";
            Debug.Log(e.Message);
            //RegisterMessage.text = e.Message;
        }
        finally
        {
            IsRegistering = false;
        }
    }
    public void Clean()
    {
        Username.text = "";
        Playername.text = "";
        Password.text = "";
        Password2.text = "";
        RegisterMessage.text = "";
    }
}
