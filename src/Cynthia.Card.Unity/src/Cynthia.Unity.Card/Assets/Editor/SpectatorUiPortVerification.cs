using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Cynthia.Card;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Uses the real serialized scene and menu methods without Play mode, login or scene saves.
public static class SpectatorUiPortVerification
{
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Stop Play mode first.");
        var setup = EditorSceneManager.GetSceneManagerSetup();
        if (setup.Any(s => s.isLoaded && UnityEngine.SceneManagement.SceneManager.GetSceneByPath(s.path).isDirty))
            throw new InvalidOperationException("Save open scenes first.");
        var originalContainer = DependencyResolver.Container;
        IContainer container = null;
        GameObject cardPrefab = null;
        GameCardShowControl control = null;
        var checks = new List<string>();
        try
        {
            var localizationType = typeof(GameCardShowControl).Assembly.GetType("Assets.Script.Localization.LocalizationService", true);
            var builder = new ContainerBuilder();
            builder.RegisterType(localizationType).SingleInstance();
            container = builder.Build();
            DependencyResolver.Container = container;
            var localization = container.Resolve(localizationType);
            Require((bool)localizationType.GetMethod("IsContainsKey").Invoke(localization, new object[] { "IngameMenu_PlayerDeck" }), "deck title translation exists", checks);
            var expectedTitle = (string)localizationType.GetMethod("GetText").Invoke(localization, new object[] { "IngameMenu_PlayerDeck" });

            cardPrefab = PrefabUtility.LoadPrefabContents("Assets/Resources/Prefab/Cards/CardObj.prefab");
            var cardTargets = cardPrefab.GetComponentsInChildren<CanRightOn>(true);
            Require(cardTargets.Length > 0 && cardTargets.All(t => t.Type == RightOnType.Card && (int)t.Type == 3), "CardObj keeps serialized Card=3", checks);
            Require((int)RightOnType.EnemyDeck == 4 && (int)MenuShowType.EnemyDeck == 5, "new menu modes keep appended IDs", checks);

            var scene = EditorSceneManager.OpenScene("Assets/Resources/Scenes/GamePlay.unity");
            var roots = scene.GetRootGameObjects();
            var deckTarget = roots.SelectMany(g => g.GetComponentsInChildren<CanRightOn>(true))
                .Single(t => t.gameObject.name == "EnemyDeck");
            Require(deckTarget.Type == RightOnType.EnemyDeck && (int)deckTarget.Type == 4, "EnemyDeck scene target resolves to 4", checks);
            control = roots.SelectMany(g => g.GetComponentsInChildren<GameCardShowControl>(true)).Single();
            Require(control.GameEvent != null && control.UICardPrefab != null && control.NullCardPrefab != null &&
                control.CardsContent != null && control.ShowCardMessage != null && control.Scroll != null &&
                control.CardSelectUI != null && control.ArtCard != null && control.MulliganEndButton != null &&
                control.CloseButton != null && control.AffirmButton != null && control.HideButton != null, "real menu references resolve", checks);

            var mine = new CardStatus(CardId.GeraltOfRivia) { IsPremium = false };
            var enemy = new CardStatus(CardId.Dandelion) { IsPremium = true };
            control.MyDeck = new List<CardStatus> { mine };
            control.EnemyDeck = new List<CardStatus> { enemy };
            // Remove temporary grid objects immediately between calls: the runtime menu uses
            // deferred Destroy, which Unity cannot execute in edit mode.
            ClearGrid(control);
            control.ArtCard.gameObject.SetActive(true);
            control.LastHoveredCard = 7;
            control.ShowEnemyDeck();
            CheckMenu(control, enemy, expectedTitle, "enemy", checks);
            ClearGrid(control);
            control.ArtCard.gameObject.SetActive(true);
            control.LastHoveredCard = 0;
            control.ShowMyDeck();
            CheckMenu(control, mine, expectedTitle, "my", checks);
            ClearGrid(control);
            control.ArtCard.gameObject.SetActive(true);
            control.ShowEnemyDeck();
            CheckMenu(control, enemy, expectedTitle, "enemy again", checks);
            control.CloseButtonClick();
            Require(!control.CardSelectUI.activeSelf && !control.ArtCard.gameObject.activeSelf, "close clears menu and preview", checks);
        }
        finally
        {
            try
            {
                if (control != null) ClearGrid(control);
                if (cardPrefab != null) PrefabUtility.UnloadPrefabContents(cardPrefab);
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
            finally
            {
                DependencyResolver.Container = originalContainer;
                if (container != null) container.Dispose();
            }
        }
        var output = Environment.GetEnvironmentVariable("GWENT_SPECTATOR_VERIFY_OUTPUT");
        if (string.IsNullOrEmpty(output)) output = Path.GetFullPath("../../../../work/spectator-ui-verification.json");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output)));
        File.WriteAllText(output, JsonConvert.SerializeObject(new { passed = true, checks,
            boundary = "Unity edit-mode serialized bindings and real menu population/cleanup; excludes input events, rendered art, premium motion and network match acceptance" }, Formatting.Indented));
        Debug.Log("Spectator UI port verification passed: " + checks.Count);
    }

    private static void ClearGrid(GameCardShowControl control)
    {
        while (control.CardsContent.childCount > 0)
            UnityEngine.Object.DestroyImmediate(control.CardsContent.GetChild(0).gameObject);
    }

    private static void CheckMenu(GameCardShowControl control, CardStatus expected, string title, string label, List<string> checks)
    {
        var displayed = control.CardsContent.GetComponentsInChildren<CardShowInfo>(true);
        Require(displayed.Length == 1 && displayed[0].CurrentCore.CardId == expected.CardId &&
            displayed[0].CurrentCore.IsPremium == expected.IsPremium, label + " actual card list and premium status", checks);
        Require(control.CardSelectUI.activeSelf && control.CloseButton.activeSelf &&
            !control.MulliganEndButton.activeSelf && !control.AffirmButton.activeSelf && !control.HideButton.activeSelf &&
            control.ShowCardMessage.text == title, label + " menu presentation controls", checks);
        Require(!control.ArtCard.gameObject.activeSelf && control.LastHoveredCard == -1, label + " stale hover preview cleared", checks);
    }

    private static void Require(bool condition, string message, List<string> checks)
    {
        if (!condition) throw new InvalidOperationException(message);
        checks.Add(message);
    }
}
