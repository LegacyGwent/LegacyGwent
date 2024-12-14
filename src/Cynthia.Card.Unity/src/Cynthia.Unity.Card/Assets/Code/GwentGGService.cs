using Alsein.Extensions.IO;
using Alsein.Extensions.LifetimeAnnotations;
using Assets.Script.Localization;
using Assets.Script.ResourceManagement;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.Common.Models.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Cynthia.Card.Client
{
    [Singleton]
    public class GwentGGService
    {
        public HubConnection HubConnection { get; set; }
        private GlobalUIService _globalUIService;
        private ITubeInlet sender;/*待修改*/
        private ITubeOutlet receiver;/*待修改*/

        private LocalizationService _translator;

        public ClientState ClientState { get; set; } = ClientState.Standby;

        /*待修改*/
        public Task<bool> MatchResult()
        {
            return receiver.ReceiveAsync<bool>();
        }
        public Task<bool> DisplayGG()
        {
            return receiver.ReceiveAsync<bool>();
        }
        public GwentGGService(IContainer container, GlobalUIService globalUIService)
        {
            _translator = container.Resolve<LocalizationService>();
            _globalUIService = globalUIService;
            /*待修改*/
            (sender, receiver) = Tube.CreateSimplex();
            var hubConnection = container.ResolveNamed<HubConnection>("game");
            hubConnection.On<bool>("DisplayGG", async x =>
            {
                await sender.SendAsync<bool>(x);
            });
            // HubConnection = hubConnection;
            // hubConnection.StartAsync();
        }
    }
}
