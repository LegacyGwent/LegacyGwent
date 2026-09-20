using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace Assets.Script.DynamicCards
{
    public static class PremiumCollectionClient
    {
        public static event Action Changed;
        public static PremiumCollection Account { get; private set; }
        public static Dictionary<string, int> Costs { get; private set; } = new Dictionary<string, int>();
        public static bool Ready { get; private set; }
        private static int session;
        private static GwentClientService Client => DependencyResolver.Container.Resolve<GwentClientService>();
        public static int Count(string card, bool premium) => premium ? (Ready ? CardInventory.PremiumCount(Account, card) : 0) : CardInventory.Limit(card);
        public static bool Owns(string card) => Count(card, true) > 0;
        public static bool Selected(string card) => Owns(card) && Account.SelectedCards.Contains(card);
        public static bool Show(CardStatus card) => card != null && !card.IsCardBack && !card.Conceal &&
            (UnityEngine.SceneManagement.SceneManager.GetSceneByName("GamePlay").isLoaded ? card.IsPremium == true :
                Owns(card.CardId) && (card.IsPremium ?? Selected(card.CardId)));

        public static void Reset()
        {
            DailyQuestClient.Reset();
            session++;
            Ready = false; Account = null; Costs = new Dictionary<string, int>();
            Changed?.Invoke();
        }

        public static async Task<PremiumCollectionResult> Refresh() => await Request("GetPremiumCollection");
        public static async Task<PremiumCollectionResult> Craft(string card) => await Craft(card, Guid.NewGuid().ToString("N"));
        public static async Task<PremiumCollectionResult> Craft(string card, string requestId) => await Request("CraftPremiumCopy", card, requestId);
        public static async Task<PremiumCollectionResult> Select(string card, bool premium) => await Request("SelectPremium", card, premium ? 1 : 0);

        private static async Task<PremiumCollectionResult> Request(string method, params object[] args)
        {
            int generation = session;
            var userId = Client.User?.Id;
            if (userId == null) return new PremiumCollectionResult { Status = "unauthenticated" };
            var result = await Client.HubConnection.InvokeCoreAsync<PremiumCollectionResult>(method, args);
            if (generation != session || userId != Client.User?.Id) return new PremiumCollectionResult { Status = "session_changed" };
            Accept(result,userId);
            return result;
        }

        public static void Accept(PremiumCollectionResult result, string userId)
        {
            if (result != null && result.Collection != null && result.Collection.Id == userId && userId == Client.User?.Id &&
                (Account == null || result.Collection.Revision >= Account.Revision))
            {
                Account = result.Collection;
                Costs = result.Costs ?? new Dictionary<string, int>();
                if (Client.User?.Decks != null && Account.DeckSelections != null)
                {
                    foreach (var deck in Client.User.Decks)
                    {
                        if (deck == null || !Account.DeckSelections.TryGetValue(deck.Id, out var selection) || selection == null)
                            continue;
                        deck.PremiumCards = new Dictionary<string, int>(selection.PremiumCards ?? new Dictionary<string, int>());
                        deck.PremiumLeader = selection.PremiumLeader;
                    }
                }
                Ready = true;
                Changed?.Invoke();
            }
        }
    }
}
