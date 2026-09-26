using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.Script.DynamicCards;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Real local-server/UI integration; uses a fresh fixture account, never the user's collection.
[InitializeOnLoad]
public static class PremiumCopyVerification
{
    static readonly string Work = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../work/PremiumCopies"));
    static bool running;
    static PremiumCopyVerification() { EditorApplication.update += Poll; }
    static async void Poll()
    {
        var request = Path.Combine(Work, "ui.request");
        if (running || !EditorApplication.isPlaying || EditorApplication.isCompiling || !File.Exists(request)) return;
        try { File.Delete(request); } catch (IOException) { return; }
        running = true; var checks = new List<string>();
        void Check(bool pass, string label) { if (!pass) throw new Exception(label); checks.Add(label); }
        try
        {
            if (!EditorPrefs.GetBool("LegacyGwent.UseLocalServer." + Application.dataPath, true)) throw new Exception("Local server required");
            var client = DependencyResolver.Container.Resolve<GwentClientService>();
            if (client.User != null) throw new Exception("Start at the login screen to protect the current account");
            string user = "copy-ui-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            if (client.HubConnection.State == HubConnectionState.Disconnected) await client.HubConnection.StartAsync();
            await client.Register(user, "PremiumCopyTest2026", user);
            await client.Login(user, "PremiumCopyTest2026");
            Check(client.User?.UserName == user, "fresh local UI fixture logged in");
            SceneManager.LoadScene("Game"); await Task.Delay(1800);
            await PremiumCollectionClient.Refresh();
            var editor = Resources.FindObjectsOfTypeAll<EditorInfo>().First(x => x.gameObject.scene.IsValid());
            editor.MainUI.SetActive(false); editor.EditorUI.SetActive(true); editor.OpenEditor(false); await Task.Delay(600);
            string id = PremiumCollectionClient.Costs.First(x => x.Value == 100 && GwentMap.CardMap[x.Key].Faction == Faction.Monsters &&
                GwentMap.CardMap[x.Key].CardType != CardType.Special).Key;
            editor.SetShowCardInfo(new List<CardStatus> { new CardStatus(id) }); await Task.Delay(1600);
            var cards = editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
            Check(cards.Length == 2, "collection has separate standard and premium entries");
            bool NoCount(CardShowInfo card) => card.transform.Find("CountIcon") == null || !card.transform.Find("CountIcon").gameObject.activeSelf;
            Check(cards.All(NoCount), "collection hides both standard and premium quantities");
            foreach (var card in cards)
            {
                var edge = card.CardBorder.transform.Find("PremiumEdge");
                Check(card.CurrentCore.IsPremium == true ? edge != null && edge.gameObject.activeSelf &&
                    edge.GetComponent<Image>().material.shader.name == "LegacyGwent/UI/PremiumBorder" :
                    edge == null || !edge.gameObject.activeSelf,
                    "only premium collection variant uses original animated border materials");
            }
            var shader = Resources.Load<Shader>("PremiumCrafting/LegacyPremiumBorder");
            Check(shader.isSupported && !ShaderUtil.ShaderHasError(shader), "premium border shader compiles on current graphics backend");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "collection-before.png"));
            await Task.Delay(200);
            editor.OpenCardDetails(new CardStatus(id) { IsPremium = true }); await Task.Delay(1500);
            var detail = UnityEngine.Object.FindObjectOfType<righclickLogic>();
            Check(detail.CardImg.GetComponent<PremiumCardAppearance>()?.IsLocked == true, "unowned premium details are grayscale");
            long before = PremiumCollectionClient.Account.MeteoritePowder;
            for (int count = 1; count <= 3; count++)
            {
                var button = Resources.FindObjectsOfTypeAll<Button>().Single(x => x.name == "CraftPremium" && x.gameObject.activeInHierarchy);
                Check(button.interactable, "craft button enables copy " + count);
                button.onClick.Invoke();
                await Until(() => !editor.PremiumPanel.Busy, 20);
                Check(PremiumCollectionClient.Count(id, true) == count && PremiumCollectionClient.Count(id, false) == 3,
                    "craft " + count + " increments premium only and retains standard x3");
                Check(PremiumCollectionClient.Account.MeteoritePowder == before - count * 100, "copy " + count + " deducts exactly 100 powder");
                if (count == 1) { ScreenCapture.CaptureScreenshot(Path.Combine(Work, "detail-one-copy.png")); await Task.Delay(200); }
            }
            var full = Resources.FindObjectsOfTypeAll<Button>().Single(x => x.name == "CraftPremium" && x.gameObject.activeInHierarchy);
            Check(!full.interactable, "craft button disables at three premium copies");
            detail.Closerightclick(); await Task.Delay(700);
            cards = editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
            Check(cards.All(NoCount), "collection quantities remain hidden after three crafts");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "collection-full.png"));
            await Task.Delay(850);
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "collection-full-later.png"));
            await Task.Delay(200);
            var premiumCard = cards.Single(x => x.CurrentCore.IsPremium == true);
            var viewport = premiumCard.GetComponentInParent<ScrollRect>().viewport;
            var cardRect = (RectTransform)premiumCard.transform;
            var savedPosition = cardRect.position;
            var clippedPosition = savedPosition;
            clippedPosition.y = viewport.TransformPoint(new Vector3(0, viewport.rect.yMax, 0)).y;
            cardRect.position = clippedPosition;
            await Task.Delay(400);
            var edgeImage = premiumCard.CardBorder.transform.Find("PremiumEdge").GetComponent<Image>();
            Check(edgeImage.maskable && edgeImage.material.HasProperty("_StencilComp") && edgeImage.material.renderQueue == 3000,
                "premium border participates in UI masks and transparent rendering");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "collection-border-clipped.png"));
            await Task.Delay(200);
            cardRect.position = savedPosition;
            var deck = new DeckModel { Id = Guid.NewGuid().ToString(), Name = "Copy UI mixed", Leader = GwentMap.CardMap.First(x => x.Value.Group == Group.Leader && x.Value.Faction == Faction.Monsters).Key,
                PremiumCards = new Dictionary<string, int>(), PremiumLeader = false };
            client.User.Decks.Add(deck); editor.ShowDeckEditorClick(deck.Id);
            var groupSamples = new[] { Group.Copper, Group.Silver, Group.Gold, Group.Leader }
                .Select(group => new CardStatus(GwentMap.CardMap.First(x => x.Value.Group == group && x.Value.Faction == Faction.Monsters).Key)).ToList();
            editor.SetEditorCardInfo(groupSamples); await Task.Delay(1600);
            var samples = editor.EditorCardsContext.GetComponentsInChildren<EditorUICoreCard>();
            Check(samples.Length == 8, "quantity visibility covers both versions of all four card groups");
            foreach (var sample in samples)
                Check(sample.CountIcon.activeSelf == (sample.cardShowInfo.CurrentCore.Group == Group.Copper && sample.cardShowInfo.CurrentCore.IsPremium == true),
                    "deck quantity visibility " + sample.cardShowInfo.CurrentCore.Group + " premium=" + sample.cardShowInfo.CurrentCore.IsPremium);
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "deck-quantity-visibility.png"));
            await Task.Delay(200);
            editor.SetEditorCardInfo(new List<CardStatus> { new CardStatus(id) });
            editor.ClickEditorUICoreCard(new CardStatus(id) { IsPremium = false });
            editor.ClickEditorUICoreCard(new CardStatus(id) { IsPremium = false });
            editor.ClickEditorUICoreCard(new CardStatus(id) { IsPremium = true }); await Task.Delay(1200);
            var rows = editor.EditorCListContext.GetComponentsInChildren<ListCardShowInfo>();
            Check(rows.Length == 2 && !rows.Single(x => x.CardStatus.IsPremium == false).Count.activeSelf &&
                rows.Single(x => x.CardStatus.IsPremium == true).Count.activeSelf && rows.Single(x => x.CardStatus.IsPremium == true).CountText.text == "x1",
                "deck rows separate versions and show only premium copper quantity");
            Check(rows.Single(x => x.CardStatus.IsPremium == true).Border.transform.Find("PremiumEdge").gameObject.activeSelf,
                "premium miniature has its distinct border");
            editor.ClickEditorUICoreCard(new CardStatus(id) { IsPremium = true });
            Check(deck.Deck.Count == 3, "mixed variants share the three-copy deck cap");
            var editorCards = editor.EditorCardsContext.GetComponentsInChildren<EditorUICoreCard>();
            Check(!editorCards.Single(x => x.cardShowInfo.CurrentCore.IsPremium == false).CountIcon.activeSelf &&
                editorCards.Single(x => x.cardShowInfo.CurrentCore.IsPremium == true).CountText.text == "X2" &&
                editorCards.Single(x => x.cardShowInfo.CurrentCore.IsPremium == true).CountIcon.activeSelf,
                "only premium copper original counter shows remaining copies in deck editor");
            Check(editor.EditorCardsContext.GetComponentsInChildren<EditorUICoreCard>().All(x => x.Gray.activeSelf),
                "both variants show unavailable when the shared deck limit is full");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "deck-mixed.png"));
            await Task.Delay(200);
            editor.ClickEditorListCard(id, false);
            Check(deck.Deck.Count == 2 && deck.PremiumCards[id] == 1, "removing standard row preserves the premium copy");
            editor.ClickEditorListCard(id, true);
            Check(deck.Deck.Count == 1 && CardInventory.DeckPremiumCount(deck, id) == 0, "removing premium row preserves the standard copy");
            editor.ClickEditorUICoreCard(new CardStatus(id) { IsPremium = true });
            Check(await client.HubConnection.InvokeAsync<bool>("AddDeck", deck), "mixed deck saves through real SignalR server");
            File.WriteAllText(Path.Combine(Work, "ui-result.json"), JsonConvert.SerializeObject(new { passed = true, checks, user, card = id, deck }, Formatting.Indented));
        }
        catch (Exception e) { File.WriteAllText(Path.Combine(Work, "ui-result.json"), JsonConvert.SerializeObject(new { passed = false, checks, error = e.ToString() }, Formatting.Indented)); Debug.LogException(e); }
        finally { running = false; }
    }
    static async Task Until(Func<bool> predicate, int seconds)
    {
        var end = DateTime.UtcNow.AddSeconds(seconds);
        while (!predicate()) { if (DateTime.UtcNow > end) throw new TimeoutException("UI operation timed out"); await Task.Delay(100); }
    }
}
