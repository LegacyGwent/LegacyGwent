using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Assets.Script.DynamicCards;
using Cynthia.Card;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only local integration entry. Requests and captures never ship in the player.
[InitializeOnLoad]
public static class PremiumCraftingVerification
{
    private static string Work => Path.GetFullPath(Path.Combine(Application.dataPath, "../../../../../work/PremiumCrafting"));
    private static bool running;
    static PremiumCraftingVerification() { EditorApplication.update += Poll; }
    private static async void Poll()
    {
        var request = Path.Combine(Work, "ui.request");
        if (running || EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(request)) return;
        string mode;
        try { mode = File.ReadAllText(request).Trim(); File.Delete(request); }
        catch (IOException) { return; } // The request writer may still be closing its file.
        running = true;
        try
        {
            foreach (var asset in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/PremiumCrafting", "Assets/Resources/LegacyCardPreview" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(asset);
                if (new[] { "Noise27White", "PortalNoise", "CardShineTex" }.Contains(Path.GetFileNameWithoutExtension(path))) continue;
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                if (importer.textureType != TextureImporterType.Sprite) { importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single; importer.SaveAndReimport(); }
            }
            if (mode == "prepare") { File.WriteAllText(Path.Combine(Work, "ui-result.txt"), "PREPARED"); return; }
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play mode before running the UI check.");
            if (!EditorPrefs.GetBool("LegacyGwent.UseLocalServer." + Application.dataPath, true))
                throw new InvalidOperationException("Premium verification is restricted to the local test server.");
            var client = DependencyResolver.Container.Resolve<GwentClientService>();
            if (client.User != null && client.User.UserName != "premium-ui-test")
                throw new InvalidOperationException("Log out of the current account before running the local premium fixture.");
            if (mode == "cn")
            {
                var type = typeof(ArtCard).Assembly.GetType("Assets.Script.Localization.LocalizationService");
                var localizer = type.GetField("TextLocalization").GetValue(DependencyResolver.Container.Resolve(type));
                localizer.GetType().GetMethod("ChooseLanguage").Invoke(localizer, new object[] { 1 });
            }
            var hub = client.HubConnection;
            if (hub.State == HubConnectionState.Disconnected) await hub.StartAsync();
            if (client.User == null)
            {
                await client.Register("premium-ui-test", "PremiumUiTest2026", "Premium UI Test");
                await client.Login("premium-ui-test", "PremiumUiTest2026");
                if (client.User == null) throw new Exception("Local verification login failed");
                await hub.InvokeAsync<bool>("ClearNewlyUnlockedTrinkets", client.User.UserName);
                SceneManager.LoadScene("Game");
                await Task.Delay(1500);
            }
            var editor = Resources.FindObjectsOfTypeAll<EditorInfo>().First(x => x.gameObject.scene.IsValid());
            editor.MainUI.SetActive(false); editor.EditorUI.SetActive(true); editor.OpenEditor(false);
            await Task.Delay(600);
            if (mode == "premium" || mode == "all") editor.SetPremiumFilter(mode == "premium" ? 1 : 2);
            if (mode == "deck") { editor.ShowDeckEditorClick(client.User.Decks.First(x=>x.Id!="blacklist").Id); editor.SetPremiumFilter(1); }
            string card = mode == "craft" ? PremiumCollectionClient.Costs.First(x => x.Value == 100 && !PremiumCollectionClient.Owns(x.Key)).Key :
                PremiumCollectionClient.Account.OwnedCards.FirstOrDefault() ?? PremiumCollectionClient.Costs.First(x => x.Value == 100).Key;
            editor.SelectSwitchUICard(new CardStatus(card) { IsPremium = mode == "premium" || mode == "craft" });
            await Task.Delay(1600);
            if (mode == "contracts")
            {
                var checks = new System.Collections.Generic.List<string>();
                Action<bool,string> check = (pass,label) => { if(!pass) throw new Exception(label); checks.Add(label); };
                var type = typeof(ArtCard).Assembly.GetType("Assets.Script.Localization.LocalizationService");
                var localizer = DependencyResolver.Container.Resolve(type);
                string name = (string)type.GetMethod("GetCardName").Invoke(localizer, new object[] { card });
                editor.ShowSearchChange(name);
                editor.SetPremiumFilter(0); await Task.Delay(400);
                var ordinary = editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
                check(ordinary.Length==1 && ordinary[0].CurrentCore.IsPremium==false && ordinary[0].CardImg.GetComponentInChildren<UnityEngine.UI.RawImage>()==null,"ordinary filter stays static even when premium is owned");
                editor.SetPremiumFilter(1); await Task.Delay(2400);
                var premium = editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
                check(premium.Length==1 && premium[0].CurrentCore.IsPremium==true && premium[0].CardImg.GetComponentInChildren<UnityEngine.UI.RawImage>()!=null,"premium filter renders the unlocked animation");
                editor.SetPremiumFilter(2); await Task.Delay(600);
                var both = editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>();
                check(both.Length==2 && both.Count(x=>x.CurrentCore.IsPremium==true)==1,"all filter shows separate ordinary and premium versions");
                var deck=client.User.Decks.First(x=>x.Id!="blacklist"); editor.ShowDeckEditorClick(deck.Id);
                int count=deck.Deck.Count;
                var locked=PremiumCollectionClient.Costs.Keys.First(x=>!PremiumCollectionClient.Owns(x));
                editor.ClickEditorUICoreCard(new CardStatus(locked){IsPremium=true}); await Task.Delay(300);
                check(deck.Deck.Count==count,"locked premium cannot be added to deck");
                int premiumBefore = CardInventory.DeckPremiumCount(deck, card);
                editor.ClickEditorUICoreCard(new CardStatus(card){IsPremium=false}); await Task.Delay(500);
                check(CardInventory.DeckPremiumCount(deck, card) == premiumBefore,"adding standard never changes the deck's premium copy count");
                editor.ClickEditorUICoreCard(new CardStatus(card){IsPremium=true}); await Task.Delay(500);
                check(CardInventory.DeckPremiumCount(deck, card) <= PremiumCollectionClient.Count(card, true),"adding premium respects the owned copy count");
                File.WriteAllText(Path.Combine(Work,"ui-contracts.json"),Newtonsoft.Json.JsonConvert.SerializeObject(checks,Newtonsoft.Json.Formatting.Indented));
            }
            if (mode == "craft")
            {
                editor.PremiumPanel.CraftClicked();
                await Task.Delay(400);
                ScreenCapture.CaptureScreenshot(Path.Combine(Work, "ui-in-place-start.png"));
                await Task.Delay(1400);
                ScreenCapture.CaptureScreenshot(Path.Combine(Work, "ui-charge.png"));
                await Task.Delay(1850);
                ScreenCapture.CaptureScreenshot(Path.Combine(Work, "ui-reveal.png"));
                await Task.Delay(4000);
                editor.SelectSwitchUICard(new CardStatus(card) { IsPremium = true });
            }
            if (mode == "equip") { editor.PremiumPanel.SelectClicked(); await Task.Delay(800); }
            ScreenCapture.CaptureScreenshot(Path.Combine(Work, "ui-" + mode + ".png"));
            var details = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>().Where(x => x.gameObject.activeInHierarchy && x.transform.IsChildOf(editor.transform))
                .Select(x => x.name + ": " + x.text).ToArray();
            File.WriteAllLines(Path.Combine(Work, "ui-text.txt"), details);
            File.WriteAllText(Path.Combine(Work, "ui-result.txt"), "PASS " + mode + "\nplayerId=" + client.User.Id + "\ncard=" + card);
            File.WriteAllText(Path.Combine(Work, "ui-" + mode + "-state.json"), Newtonsoft.Json.JsonConvert.SerializeObject(new {
                card, powder=PremiumCollectionClient.Account.MeteoritePowder, owned=PremiumCollectionClient.Owns(card), selected=PremiumCollectionClient.Selected(card),
                visibleCards=editor.ShowCardsContent.GetComponentsInChildren<CardShowInfo>().Select(x=>new {id=x.CurrentCore.CardId,premium=x.CurrentCore.IsPremium,animated=x.CardImg.GetComponentInChildren<UnityEngine.UI.RawImage>()!=null}),
                runtimeAssembly=typeof(PremiumCollectionClient).Assembly.Location
            },Newtonsoft.Json.Formatting.Indented));
        }
        catch (Exception e) { File.WriteAllText(Path.Combine(Work, "ui-result.txt"), "FAILED " + e); Debug.LogException(e); }
        finally { running = false; }
    }
}
