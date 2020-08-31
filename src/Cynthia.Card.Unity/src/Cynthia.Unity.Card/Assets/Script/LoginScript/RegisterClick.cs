using Cynthia.Card.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Autofac;
using System;
using Cynthia.Card.Common.Models;
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
    private ITranslator _translator;

    private void Start()
    {
        if (server != null)
            return;
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        _translator = DependencyResolver.Container.Resolve<ITranslator>();
    }
    public async void Register()
    {
        if (IsRegistering) return;
        IsRegistering = true;
        if (Password.text.Length == 0 || Username.text.Length == 0 || Playername.text.Length == 0)
        {
            RegisterMessage.text = _translator.GetText("RegisterMenu_EmptyInput");
            IsRegistering = false;
            return;
        }
        if (Password.text != Password2.text)
        {
            RegisterMessage.text = _translator.GetText("RegisterMenu_PasswordsNotIdentical");
            IsRegistering = false;
            return;
        }
        RegisterMessage.text = _translator.GetText("RegisterMenu_Registering");
        try
        {
            var hub = DependencyResolver.Container.ResolveNamed<HubConnection>("game");
            if (hub.State == HubConnectionState.Disconnected)
                await hub.StartAsync();
            var result = await server.Register(Username.text, Password.text, Playername.text);
            if (!result)
            {
                RegisterMessage.text = _translator.GetText("RegisterMenu_AlreadyRegistered");
                IsRegistering = false;
                return;
            }
            IsRegistering = false;
            RegisterMessage.text = _translator.GetText("RegisterMenu_RegistrationSuccessful");
        }
        catch (Exception e)
        {
            RegisterMessage.text = _translator.GetText("RegisterMenu_ErrorRegistering");
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
