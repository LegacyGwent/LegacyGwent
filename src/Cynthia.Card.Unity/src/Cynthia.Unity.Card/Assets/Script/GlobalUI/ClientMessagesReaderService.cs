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
        private readonly HashSet<int> _processingMessageIds = new HashSet<int>();
        

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

        }

        public async Task CheckMessages(string username = null)
        {
            username = string.IsNullOrWhiteSpace(username)
                ? _clientService.User?.UserName
                : username;
            if (string.IsNullOrWhiteSpace(username)) return;

            var messages = _clientService.CheckUserMessages(username);
            var messagesList = await messages;
            var seasonMessages = new List<UserSeasonEndMessage>();
            foreach (var condensedMessage in messagesList ?? new List<string>())
            {
                try
                {
                    if (UserMessage.ReCreateMessage(condensedMessage) is UserSeasonEndMessage seasonEndMessage)
                        seasonMessages.Add(seasonEndMessage);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Ignoring malformed user message: {e.Message}");
                }
            }

            // Imported/legacy accounts can contain several years of unacknowledged
            // season notices. Show only the newest one, then persistently consume the
            // complete stale backlog so the player is not prompted once per login.
            var latest = seasonMessages.OrderByDescending(x => x.MessageId).FirstOrDefault();
            if (latest == null) return;
            await HandleSeasonEndMessage(
                latest.avatars,
                latest.borders,
                latest.titles,
                latest.mmrBeforeReset,
                latest.rank,
                latest.seasonName,
                latest.MessageId,
                seasonMessages.Select(x => x.MessageId).Distinct().ToList(),
                username);
        }

        public async Task HandleSeasonEndMessage(IList<string> avatars, IList<string> borders, IList<string> titles, int mmrBeforeReset, int rank, string seasonName, int messageId = -1, IList<int> acknowledgementIds = null, string acknowledgementUsername = null)
        {
            Debug.Log($"handling message {messageId}");
            async Task SpawnMessage()
            {
                if (messageId != -1 && !_processingMessageIds.Add(messageId)) return;
                try
                {
                    await _globalUIService.YNMessageBoxEnhanced("SeasonEnd_MessageTitle", string.Format(_translator.GetText("Season_EndMessageRewards"), _translator.GetText(seasonName), rank.ToString(), mmrBeforeReset.ToString()), yes: "PopupWindow_YesButton", no: "PopupWindow_NoButton", isOnlyYes: true, message2: "", message3: "", avatars: avatars, borders: borders, titles: titles);
                    if (messageId != -1)
                    {
                        var ids = (acknowledgementIds ?? new List<int> { messageId })
                            .Where(x => x >= 0)
                            .Distinct()
                            .ToList();
                        foreach (var id in ids)
                        {
                            var username = string.IsNullOrWhiteSpace(acknowledgementUsername)
                                ? _clientService.User?.UserName
                                : acknowledgementUsername;
                            if (string.IsNullOrWhiteSpace(username) ||
                                !await _clientService.RemoveUserMessage(username, id))
                                Debug.LogError($"Failed to acknowledge user message {id} for {username ?? "<unknown>"}.");
                        }
                    }
                }
                finally
                {
                    if (messageId != -1) _processingMessageIds.Remove(messageId);
                }
            }

            async void OnClientStateChanged()
            {
                if (_clientService.ClientState == ClientState.Standby)
                {
                    _clientService.ClientStateChanged -= OnClientStateChanged;
                    await SpawnMessage();
                }
            }


            if (_clientService.ClientState != ClientState.Standby)
                _clientService.ClientStateChanged += OnClientStateChanged;

            else
                await SpawnMessage();
        }
    }
}
