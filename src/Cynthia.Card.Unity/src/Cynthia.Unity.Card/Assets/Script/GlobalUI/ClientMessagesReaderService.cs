using Alsein.Extensions.IO;
using Alsein.Extensions.LifetimeAnnotations;
using Assets.Script.Localization;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
        private readonly SemaphoreSlim _checkGate = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _displayGate = new SemaphoreSlim(1, 1);
        private string _checkingUsername;
        private bool _checkNewOwner;
        

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
            hubConnection.On<IList<string>, IList<string>, IList<string>, int, int, string>("DisplaySeasonEndMessage", (avatars, borders, titles, mmrBeforeReset, rank, seasonName) =>
            {
                // SignalR 5 dispatches server invocations serially. UI waits must not hold its reader.
                _ = HandleLiveSeasonEndMessage(avatars, borders, titles, mmrBeforeReset, rank, seasonName);
                return Task.CompletedTask;
            });

            _ = CheckMessages();
        }

        private async Task HandleLiveSeasonEndMessage(IList<string> avatars, IList<string> borders,
            IList<string> titles, int mmrBeforeReset, int rank, string seasonName)
        {
            try
            {
                await HandleSeasonEndMessage(avatars, borders, titles, mmrBeforeReset, rank, seasonName);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public async Task CheckMessages()
        {
            var username = _clientService.User?.UserName;
            if (username == null) return;
            if (!await _checkGate.WaitAsync(0))
            {
                if (username != _checkingUsername) _checkNewOwner = true;
                return;
            }
            _checkingUsername = username;
            try
            {
                var messages = await _clientService.CheckUserMessages(username);
                while (_clientService.User?.UserName == username)
                {
                    string pending = null;
                    UserSeasonEndMessage seasonEndMessage = null;
                    foreach (var condensedMessage in messages)
                    {
                        if (UserMessage.ReCreateMessage(condensedMessage) is UserSeasonEndMessage season)
                        {
                            pending = condensedMessage;
                            seasonEndMessage = season;
                            break;
                        }
                    }
                    if (seasonEndMessage == null) return;
                    await HandleSeasonEndMessage(seasonEndMessage.avatars, seasonEndMessage.borders,
                        seasonEndMessage.titles, seasonEndMessage.mmrBeforeReset, seasonEndMessage.rank,
                        seasonEndMessage.seasonName, seasonEndMessage.MessageId, username);
                    if (_clientService.User?.UserName != username) return;
                    messages = await _clientService.CheckUserMessages(username);
                    // Old servers return false even after removing a message. Inspect the actual queue
                    // instead; if it did not advance, stop rather than reopening the same modal.
                    if (messages.Contains(pending))
                    {
                        Debug.LogWarning("Season message acknowledgement did not advance the queue.");
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                _checkingUsername = null;
                _checkGate.Release();
                if (_checkNewOwner)
                {
                    _checkNewOwner = false;
                    _ = CheckMessages();
                }
            }
        }

        public async Task HandleSeasonEndMessage(IList<string> avatars, IList<string> borders,
            IList<string> titles, int mmrBeforeReset, int rank, string seasonName, int messageId = -1, string expectedUsername = null)
        {
            var username = expectedUsername ?? _clientService.User?.UserName;
            await _displayGate.WaitAsync();
            try
            {
                await WaitForStandby();
                if (_clientService.User?.UserName != username) return;
                await _globalUIService.YNMessageBoxEnhanced("SeasonEnd_MessageTitle",
                    string.Format(_translator.GetText("Season_EndMessageRewards"),
                        _translator.GetText(seasonName), rank.ToString(), mmrBeforeReset.ToString()),
                    yes: "PopupWindow_YesButton", no: "PopupWindow_NoButton", isOnlyYes: true,
                    message2: "", message3: "", avatars: avatars, borders: borders, titles: titles);
                if (messageId != -1 && _clientService.User?.UserName == username)
                    await _clientService.RemoveUserMessage(messageId, username);
            }
            finally
            {
                _displayGate.Release();
            }
        }

        private Task WaitForStandby()
        {
            if (_clientService.ClientState == ClientState.Standby) return Task.CompletedTask;
            var ready = new TaskCompletionSource<bool>();
            void OnClientStateChanged()
            {
                if (_clientService.ClientState != ClientState.Standby) return;
                // Unsubscribe before completing: completion can synchronously display a modal.
                _clientService.ClientStateChanged -= OnClientStateChanged;
                ready.TrySetResult(true);
            }
            _clientService.ClientStateChanged += OnClientStateChanged;
            OnClientStateChanged();
            return ready.Task;
        }
    }
}
