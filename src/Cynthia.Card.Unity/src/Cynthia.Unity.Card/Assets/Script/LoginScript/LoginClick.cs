using Autofac;
using Cynthia.Card.Client;
using Cynthia.Card.Common.Models;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginClick : MonoBehaviour
{
    private static bool IsLogining = false;

    public InputField Username;
    public InputField Password;
    public Text LogMessage;
    public Toggle RecordStatus;

    private GwentClientService _client;
    private ITranslator _translator;

    private void Start()
    {
        void IsOnPreservation(bool isOn)
        {
            if (isOn)
            {
                PlayerPrefs.SetString("Username", Username.text);
                PlayerPrefs.SetString("Password", Password.text);
            }
        }
        RecordStatus.isOn = PlayerPrefs.GetInt("RecordBox", 0) != 0;
        if (RecordStatus.isOn)
        {
            Username.text = PlayerPrefs.GetString("Username", "");
            Password.text = PlayerPrefs.GetString("Password", "");
        }
        RecordStatus.onValueChanged.AddListener(x =>
        {
            PlayerPrefs.SetInt("RecordBox", x ? 1 : 0);
            IsOnPreservation(x);
        });
        Username.onValueChanged.AddListener(x =>
        {
            IsOnPreservation(RecordStatus.isOn);
        });
        Password.onValueChanged.AddListener(x =>
        {
            IsOnPreservation(RecordStatus.isOn);
        });

        if (_client != null)
            return;
        _client = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<ITranslator>();
    }
    public async void Login()
    {
        if (IsLogining)
            Debug.Log("因为是true,所以登陆请求被本地拦截");
        if (IsLogining) return;
        IsLogining = true;
        LogMessage.text = _translator.GetText("LoginMenu_LoggingIn");
        try
        {
            var hub = DependencyResolver.Container.ResolveNamed<HubConnection>("game");
            if (hub.State == HubConnectionState.Disconnected)
                await hub.StartAsync();
            await _client.Login(Username.text, Password.text);
            if (_client.User == null)
            {
                LogMessage.text = _translator.GetText("LoginMenu_WrongCredentials");
                IsLogining = false;
                return;
            }
            //Debug.Log($"用户名是:{_client.User.UserName},密码是:{_client.User.PassWord}");
            LogMessage.text = string.Format(_translator.GetText("LoginMenu_WelcomeMessage"), _client.User.PlayerName);
            SceneManager.LoadScene("Game");
            _client.ClientState = ClientState.Standby;
            // Debug.Log("执行了!跳转后");
            IsLogining = false;
        }
        catch
        {
            //await DependencyResolver.Container.ResolveNamed<HubConnection>().Named("game").StartAsync();
            //await _client.Login(Username.text, Password.text);
            //if (_client.User == null)
            //{
            LogMessage.text = _translator.GetText("LoginMenu_LoginError");
            //"发生异常,原因或许是服务器未开启,尝试重试或者联系作者";
            //    return;
            //}
        }
        finally
        {
            // Debug.Log("执行了!finally");
            IsLogining = false;
        }
    }
    public void Clean()
    {
        Username.text = "";
        Password.text = "";
        LogMessage.text = "";
    }
}
