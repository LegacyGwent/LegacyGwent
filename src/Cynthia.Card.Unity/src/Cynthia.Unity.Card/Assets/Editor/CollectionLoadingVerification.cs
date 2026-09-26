using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

// Explicit, local-only Play Mode regression for the real collection entry points.
[InitializeOnLoad]
public static class CollectionLoadingVerification
{
    static readonly string Work = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../work/CollectionLoading"));
    static bool running;
    static EditorInfo editor;
    static readonly List<string> checks = new List<string>();
    static readonly List<object> samples = new List<object>();
    static string violation;
    static int observedFrames;
    static CollectionLoadingVerification() { EditorApplication.update += Poll; }
    static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
    static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception(name);
        checks.Add(name);
        File.WriteAllText(Path.Combine(Work, "progress.txt"), string.Join("\n", checks));
    }
    static CardShowInfo[] Cards() => editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>().Where(x => x.gameObject.activeInHierarchy).ToArray();
    static bool Shown(CardShowInfo card) => card.GetComponentInParent<CanvasGroup>().alpha > .001f;
    static void Observe()
    {
        if (editor == null) return;
        observedFrames++;
        int visible = 0, pending = 0;
        foreach (var card in Cards())
        {
            var group = card.GetComponentInParent<CanvasGroup>();
            if (group == null) { violation = "Collection card has no whole-card loading group"; continue; }
            bool artReady = card.CardImg.sprite != null && Field<string>(card, "_loadedCardArtsId") == card.CurrentCore.CardArtsId;
            var view = card.CardImg.GetComponent<DynamicCardView>();
            bool preparing = view != null && Field<bool>(view, "preparing");
            if (group.alpha > .001f)
            {
                visible++;
                if (!artReady || preparing) violation = "A render frame exposed an unfinished card: " + card.CurrentCore.CardId;
            }
            else
            {
                pending++;
                if (group.blocksRaycasts || group.interactable) violation = "Invisible card still accepts interaction";
            }
        }
        if (samples.Count < 1200) samples.Add(new { frame = Time.frameCount, visible, pending });
    }
    static async Task Until(Func<bool> predicate, string label, int seconds = 30)
    {
        var end = DateTime.UtcNow.AddSeconds(seconds);
        while (!predicate())
        {
            if (DateTime.UtcNow > end) throw new TimeoutException(label);
            if (violation != null) throw new Exception(violation);
            await Task.Delay(40);
        }
        await Task.Delay(50);
    }
    static async void Poll()
    {
        string request = Path.Combine(Work, "check.request");
        if (running || !EditorApplication.isPlaying || EditorApplication.isCompiling || !File.Exists(request)) return;
        File.Delete(request); running = true; checks.Clear(); samples.Clear(); violation = null; observedFrames = 0;
        bool background = Application.runInBackground, dynamicEnabled = DynamicCardSettings.Enabled;
        try
        {
            Application.runInBackground = true;
            if (!EditorPrefs.GetBool("LegacyGwent.UseLocalServer." + Application.dataPath, true)) throw new Exception("Local server required");
            await Until(() => DependencyResolver.Container != null, "Bootstrap");
            var client = DependencyResolver.Container.Resolve<GwentClientService>();
            if (client.User != null && client.User.UserName != "premium-ui-test") throw new Exception("A user session is active; leave it untouched");
            if (client.HubConnection.State == HubConnectionState.Disconnected) await client.HubConnection.StartAsync();
            if (client.User == null) await client.Login("premium-ui-test", "PremiumUiTest2026");
            Check(client.User != null, "Existing local fixture login");
            SceneManager.LoadScene("Game");
            await Until(() => (editor = Resources.FindObjectsOfTypeAll<EditorInfo>().FirstOrDefault(x => x.gameObject.scene == SceneManager.GetActiveScene())) != null, "Game scene");
            await PremiumCollectionClient.Refresh();
            editor.MainUI.SetActive(false); editor.EditorUI.SetActive(true);
            editor.SetPremiumFilter(0); // This first phase deliberately checks ordinary-only cold loading.
            Canvas.willRenderCanvases += Observe;
            editor.OpenEditor(false);
            Check(Cards().Length == 30 && Cards().All(x => !Shown(x)), "Opening collection hides all 30 whole cards before artwork completion");
            await Until(() => Cards().Length == 30 && Cards().All(Shown), "Ordinary collection artwork");
            Check(violation == null, "Ordinary cards become visible with their own artwork");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "ordinary-ready.png"));
            await Task.Delay(150); // CaptureScreenshot writes at frame end; keep this state stable.

            var ids = Cards().Take(3).Select(x => x.CurrentCore.CardId).ToArray();
            var rebound = Cards()[0];
            rebound.setCurrentCore(new CardStatus(ids[1]) { IsPremium = false }, true);
            Check(!Shown(rebound), "Rebinding hides the previous card immediately");
            rebound.setCurrentCore(new CardStatus(ids[2]) { IsPremium = false }, true);
            rebound.setCurrentCore(new CardStatus(ids[1]) { IsPremium = false }, true);
            await Until(() => Shown(rebound), "Rapid rebind");
            Check(Field<string>(rebound, "_loadedCardArtsId") == rebound.CurrentCore.CardArtsId, "Rapid A-B-A requests keep the latest binding");

            editor.SetShowCardInfo(ids.Select(x => new CardStatus(x)).ToList());
            editor.SetShowCardInfo(ids.Reverse().Select(x => new CardStatus(x)).ToList());
            await Until(() => Cards().Length == 3 && Cards().All(Shown), "Rapid filter replacement");
            Check(Cards().Select(x => x.CurrentCore.CardId).SequenceEqual(ids.Reverse()), "Destroyed previous-page callbacks do not replace the new page");

            string premiumId = PremiumCollectionClient.Account.OwnedCards.FirstOrDefault();
            Check(premiumId != null, "Existing fixture owns a premium card (no crafting or account mutation)");
            DynamicCardSettings.Enabled = true;
            editor.SetPremiumFilter(1);
            editor.SetShowCardInfo(new[] { new CardStatus(premiumId) });
            var premium = Cards().Last();
            Check(!Shown(premium), "Premium card and ownership badge start hidden together");
            await Until(() => Shown(premium), "First premium render", 60);
            var view = premium.CardImg.GetComponent<DynamicCardView>();
            Check(Field<GameObject>(view, "model") != null && Field<RawImage>(view, "surface").enabled,
                "Premium first frame is rendered before whole-card reveal");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "premium-ready.png"));
            await Task.Delay(150);

            premium.gameObject.SetActive(false); premium.gameObject.SetActive(true);
            Check(!Shown(premium), "Reopening a premium card hides its cleared render surface");
            await Until(() => Shown(premium), "Premium reopen", 60);
            Check(Field<RawImage>(view, "surface") != null, "Hidden loading gate does not deadlock premium queue");

            var parentGroup = editor.ShowCardsContent.gameObject.AddComponent<CanvasGroup>();
            parentGroup.alpha = 0;
            premium.gameObject.SetActive(false); premium.gameObject.SetActive(true);
            await Task.Delay(600);
            Check(Field<GameObject>(view, "model") == null && !Shown(premium), "Hidden parent does not trigger offscreen premium loading");
            parentGroup.alpha = 1;
            await Until(() => Shown(premium), "Parent restored", 60);
            UnityEngine.Object.Destroy(parentGroup);
            Check(true, "Restoring parent visibility resumes loading");

            DynamicCardView.Bind(premium.CardImg, "__collection_missing_premium__", premium: true);
            Check(!Shown(premium), "Missing premium resource remains hidden during lookup");
            await Until(() => Shown(premium), "Static fallback");
            Check(Field<GameObject>(view, "model") == null && premium.CardImg.sprite != null, "Missing premium finishes on loaded static artwork without stale intermediate content");
            premium.SetCard(true);
            DynamicCardSettings.Enabled = false;
            await Until(() => Shown(premium), "Disabled animation fallback");
            Check(Field<GameObject>(view, "model") == null, "Disabling animations during loading restores complete ordinary card");

            editor.SetPremiumFilter(0);
            var deck = client.User.Decks.First(x => x.Id != "blacklist");
            editor.ShowDeckEditorClick(deck.Id);
            var deckCards = editor.EditorCardsContext.GetComponentsInChildren<EditorUICoreCard>();
            Check(deckCards.Length > 0 && deckCards.All(x => x.GetComponent<CanvasGroup>() != null && x.GetComponent<CanvasGroup>().alpha == 0),
                "Deck-builder loading hides the outer card including count badges");
            await Until(() => deckCards.All(x => x.GetComponent<CanvasGroup>().alpha > 0), "Deck cards");
            Check(true, "Deck-builder cards reveal after artwork completes");
            Check(observedFrames > 10 && violation == null, "Every observed render frame excludes unfinished visible cards");
            File.WriteAllText(Path.Combine(Work, "result.json"), JsonConvert.SerializeObject(new { passed = true, checks, observedFrames, samples }, Formatting.Indented));
        }
        catch (Exception e)
        {
            File.WriteAllText(Path.Combine(Work, "result.json"), JsonConvert.SerializeObject(new { passed = false, checks, observedFrames, error = e.ToString(), samples }, Formatting.Indented));
            Debug.LogException(e);
        }
        finally
        {
            Canvas.willRenderCanvases -= Observe;
            DynamicCardSettings.Enabled = dynamicEnabled;
            Application.runInBackground = background;
            running = false;
        }
    }
}
