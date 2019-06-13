using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Autofac;
using System;

public class RegisterClick : MonoBehaviour
{
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
        if (Password.text.Length == 0 || Username.text.Length == 0 || Playername.text.Length == 0)
        {
            RegisterMessage.text = "输入不能为空..请重新输入";
            return;
        }
        if (Password.text != Password2.text)
        {
            RegisterMessage.text = "两次密码输入不一致,请重新输入";
            return;
        }
        RegisterMessage.text = "正在注册...请稍等片刻";
        try
        {
            var result = await server.Register(Username.text, Password.text, Playername.text);
            if (!result)
            {
                RegisterMessage.text = "注册失败...该用户名或该游戏名已经存在,尝试更换后重新注册";
                return;
            }
            RegisterMessage.text = "成功注册~切换到登录页面可以进行登录~";
        }
        catch//(Exception e)
        {
            RegisterMessage.text = "发生了一个未知的错误...尝试重启游戏?";
            //RegisterMessage.text = e.Message;
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
