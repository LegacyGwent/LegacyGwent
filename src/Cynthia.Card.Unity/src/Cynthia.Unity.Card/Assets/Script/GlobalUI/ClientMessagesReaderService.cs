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
    public class ClientMessagesReaderService
    {
        public HubConnection HubConnection { get; set; }
        public ClientState ClientState { get; set; } = ClientState.Standby;
        private GlobalUIService _globalUIService;
        private ITubeInlet sender;/*待修改*/
        private ITubeOutlet receiver;/*待修改*/
        private LocalizationService _translator;
        private GwentClientService _clientService;
        

        public Task<string> DisplayMessage()
        {
            try
            {
                return receiver.ReceiveAsync<string>();
            }
            catch
            {
                return Task.Delay(1).ContinueWith(t => ""); // if no taunt was received yet, return "" to avoid errors
            }
        }

        public ClientMessagesReaderService(IContainer container, GlobalUIService globalUIService)
        {
            _clientService = DependencyResolver.Container.Resolve<GwentClientService>();
            _translator = container.Resolve<LocalizationService>();
            _globalUIService = globalUIService;
            (sender, receiver) = Tube.CreateSimplex();
            var hubConnection = container.ResolveNamed<HubConnection>("game");
            hubConnection.On<IList<string>, IList<string>, IList<string>, int, int, string>("DisplaySeasonEndMessage", async (avatars, borders, titles, mmrBeforeReset, rank, seasonName) =>
            {
                await HandleSeasonEndMessage(avatars, borders, titles, mmrBeforeReset, rank, seasonName);
            });

            CheckMessages();
        }

        public async Task CheckMessages()
        {
            var messages = _clientService.CheckUserMessages(_clientService.User.UserName);

            var messagesList = await messages;


            foreach (var condensedMessage in messagesList)
            {
                var deserializedMessage = UserMessage.ReCreateMessage(condensedMessage);
                Debug.Log(condensedMessage);

                if (deserializedMessage is UserSeasonEndMessage seasonEndMessage)
                {
                    await HandleSeasonEndMessage(seasonEndMessage.avatars, seasonEndMessage.borders, seasonEndMessage.titles, seasonEndMessage.mmrBeforeReset, seasonEndMessage.rank, seasonEndMessage.seasonName, seasonEndMessage.MessageId);
                    break;
                }         
            }
        }

        public async Task HandleSeasonEndMessage(IList<string> avatars, IList<string> borders, IList<string> titles, int mmrBeforeReset, int rank, string seasonName, int messageId = -1)
        {
            Debug.Log($"handling message {messageId}");
            async Task SpawnMessage()
            {
                await _globalUIService.YNMessageBoxEnhanced("SeasonEnd_MessageTitle", string.Format(_translator.GetText("Season_EndMessageRewards"), _translator.GetText(seasonName), rank.ToString(), mmrBeforeReset.ToString()), yes: "PopupWindow_YesButton", no: "PopupWindow_NoButton", isOnlyYes: true, message2: "", message3: "", avatars: avatars, borders: borders, titles: titles);
                if (messageId != -1)
                {
                    _clientService.RemoveUserMessage(messageId);
                    await CheckMessages();
                }

            }

            async void OnClientStateChanged()
            {
                if (_clientService.ClientState == ClientState.Standby)
                {
                    await SpawnMessage();
                    _clientService.ClientStateChanged -= OnClientStateChanged;
                }
            }


            if (_clientService.ClientState != ClientState.Standby)
                _clientService.ClientStateChanged += OnClientStateChanged;

            else
                SpawnMessage();
        }
    }
}
