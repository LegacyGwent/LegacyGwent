using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// Offline acceptance checks for the AI quick-match catalogue and its language packs.
public static class AiQuickMatchVerification
{
    private static readonly string[] Languages = { "cn", "en", "pl", "ru" };

    [MenuItem("Tools/Legacy Gwent/Verify/AI Quick Match")]
    public static void Run()
    {
        Require(Resources.Load<GameObject>("Prefab/GwentButton") != null,
            "AI quick match must use the extracted old-client GwentButton prefab.");
        Require(AiQuickMatchSelector.Opponents.Count == 6, "Expected exactly AI 0 through AI 5.");

        for (var index = 0; index < 6; index++)
        {
            var opponent = AiQuickMatchSelector.Opponents[index];
            var expected = index == 0 ? "ai" : "ai" + index;
            Require(opponent.Index == index, "AI slots are out of order.");
            Require(opponent.Password == expected, "Wrong password for AI " + index);
            Require(opponent.NameKey == "ai" + index + "_name", "Wrong localized name key for AI " + index);
            Require(MatchInfo.ForcedAiPassword(opponent.Password) == expected + "#f",
                "AI " + index + " does not route to its forced-match password.");
        }

        foreach (var language in Languages)
        {
            VerifyPack(Resources.Load<TextAsset>("Locales/" + language).text, language + " bundled");
            var streaming = Path.Combine(Application.dataPath, "StreamingFile", "Locales", language + ".json");
            VerifyPack(File.ReadAllText(streaming), language + " streaming");
        }
        Debug.Log("AI quick-match verification passed: six forced routes, legacy button prefab, and all localized AI names.");
    }

    private static void VerifyPack(string text, string source)
    {
        var menu = (JObject)JObject.Parse(text.TrimStart('\uFEFF'))["MenuLocales"];
        Require(menu != null && !string.IsNullOrWhiteSpace((string)menu["MainMenu_PlayingvsAIText"]),
            source + " is missing the selector label.");
        for (var index = 0; index < 6; index++)
            Require(!string.IsNullOrWhiteSpace((string)menu["ai" + index + "_name"]),
                source + " is missing the name for AI " + index);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
