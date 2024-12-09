using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynthia.Card.Client;
using Autofac;

public class DisplayGG : MonoBehaviour
{
    private GwentClientService server;
    public GameObject GGObject;
    // bool issendgg = false;
    private void Awake ()
    {
        server = DependencyResolver.Container.Resolve<GwentClientService>();
        // DisplayGGObject();
    }
    // public bool SendGG
    // {
    //     get {return issendgg;}
    //     set {
    //         issendgg = await server.SendGG();
    //         DisplayGGObject();
    //         }
    // }
    // public async void DisplayGGObject()
    // {
    //     bool issendgg = false;
    //     issendgg = await server.SendGG();
    //     // issendgg = false;
    //     if (issendgg) {GGObject.SetActive(true);}
    // }
}
