using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Assets.Script.Localization.SceneTranslating;
using Cynthia.Card;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Offline check of actual serialized Unity objects. Does not save scenes or log in.
public static class ClientUiPortVerification
{
    public static void Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode first.");
        var setup = EditorSceneManager.GetSceneManagerSetup();
        if (setup.Any(s => s.isLoaded && UnityEngine.SceneManagement.SceneManager.GetSceneByPath(s.path).isDirty))
            throw new InvalidOperationException("Save open scenes first.");
        var savedContainer = DependencyResolver.Container;
        IContainer container = null;
        GameObject prefab = null;
        var checks = new List<string>();
        try
        {
            var type = typeof(EditorInfo).Assembly.GetType("Assets.Script.Localization.LocalizationService", true);
            var builder = new ContainerBuilder();
            builder.RegisterType(type).SingleInstance();
            container = builder.Build();
            DependencyResolver.Container = container;
            var service = container.Resolve(type);
            var locale = type.GetField("TextLocalization").GetValue(service);
            var choose = locale.GetType().GetMethod("ChooseLanguage");
            var getText = type.GetMethod("GetText");
            var contains = type.GetMethod("IsContainsKey");
            prefab = PrefabUtility.LoadPrefabContents("Assets/Resources/Prefab/ViewListRow.prefab");
            var watch = prefab.GetComponentsInChildren<GameObjectTranslator>(true)
                .Single(t => t.ObjectsToTranslate.Any(e => e.Id == "Spectator_Watch_Button"));
            var watchEntry = watch.ObjectsToTranslate.Single(e => e.Id == "Spectator_Watch_Button");
            Require(watchEntry.TextObject != null && watchEntry.TextObject.GetComponent<Text>() != null, "watch target resolves", checks);
            var scene = EditorSceneManager.OpenScene("Assets/Resources/Scenes/Game.unity");
            var translators = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<GameObjectTranslator>(true));
            var backs = translators.SelectMany(t => t.ObjectsToTranslate).Where(e => e.Id == "EditorMenu_BackButton").ToArray();
            Require(backs.Length == 4 && backs.All(e => e.TextObject != null && e.TextObject.GetComponent<Text>() != null), "four back targets resolve", checks);
            // Exercise the existing translation method against the real target objects.
            var single = prefab.AddComponent<GameObjectTranslator>();
            single.ObjectsToTranslate = backs.ToList();
            for (var i = 0; i < 4; i++)
            {
                choose.Invoke(locale, new object[] { i });
                Require((bool)contains.Invoke(service, new object[] { "Spectator_Watch_Button" }) &&
                    (bool)contains.Invoke(service, new object[] { "EditorMenu_BackButton" }), "translation keys exist " + i, checks);
                watch.TranslateAll();
                single.TranslateAll();
                Require(watchEntry.TextObject.GetComponent<Text>().text == (string)getText.Invoke(service, new object[] { "Spectator_Watch_Button" }), "watch language " + i, checks);
                Require(backs.All(e => e.TextObject.GetComponent<Text>().text == (string)getText.Invoke(service, new object[] { "EditorMenu_BackButton" })), "back language " + i, checks);
            }
            var editor = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<EditorInfo>(true)).Single();
            typeof(EditorInfo).GetField("<EditorStatus>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(editor, EditorStatus.SwitchLeader);
            foreach (var premium in new[] { false, true })
            {
                var expected = (string)type.GetMethod("GetCardInfo").Invoke(service, new object[] { "41001" });
                Require(!string.IsNullOrWhiteSpace(expected) && expected != "41001_Info", "leader locale exists " + premium, checks);
                editor.SelectSwitchUICard(new CardStatus("41001") { IsPremium = premium });
                var content = editor.RightSwitchMenu.GetComponentsInChildren<CardContent>(true).Single();
                Require(content.gameObject.activeSelf && content.CardInfoText.text == expected, "leader description " + premium, checks);
            }
            var output = Environment.GetEnvironmentVariable("GWENT_UI_VERIFY_OUTPUT");
            if (string.IsNullOrEmpty(output)) output = Path.GetFullPath("../../../../work/client-ui-verification.json");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output, JsonConvert.SerializeObject(new { passed = true, checks, boundary = "Unity edit-mode object and text validation; not rendered player or device acceptance" }, Formatting.Indented));
            Debug.Log("Client UI port verification passed: " + checks.Count);
        }
        finally
        {
            try
            {
                if (prefab != null) PrefabUtility.UnloadPrefabContents(prefab);
                EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
            finally
            {
                DependencyResolver.Container = savedContainer;
                if (container != null) container.Dispose();
            }
        }
    }

    private static void Require(bool value, string label, List<string> checks)
    {
        if (!value) throw new InvalidOperationException(label);
        checks.Add(label);
    }
}
