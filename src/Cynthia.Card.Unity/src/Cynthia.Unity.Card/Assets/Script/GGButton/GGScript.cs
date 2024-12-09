using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card.Client;
using Assets.Script.Localization;
using Autofac;
using System.Threading.Tasks;

public class GGScript : MonoBehaviour
{
    // [SerializeField] DisplayGG DisplayGG;
    private LocalizationService _translator;
    private GwentClientService server;
    private async void Start ()
    {
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        SendGGToServer();
    }


    public async void SendGGToServer()
    {
            // bool sendgg =  false;
            // await server.GGSent();
            // bool issendgg = await server.SendGG();
            // DisplayGG.SendGG = issendgg;
            // await Task.CompletedTask;
            return;
    }
}