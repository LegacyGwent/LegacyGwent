using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    // Invoke these methods in the existing editor, or with -executeMethod in a
    // separate clean project. No scenes, real markers, or preferences are modified.
    [InitializeOnLoad]
    public static class DynamicCardEditorContentVerification
    {
        private const string Prefab = "Assets/DynamicCards/Content/Old/Thronebreaker/fixture/Card.prefab";
        private const string Part = "cards-thronebreaker-000.bundle";
        // The runtime intentionally keeps preference-key constants internal.
        private const string QualityPreference = "DynamicCards.Quality";
        private const string LegacyPreference = "DynamicCards.Enabled";
        private static double nextCommandCheck;
        [Serializable] private class Command { public string action; }
        [Serializable] private class Result
        {
            public bool passed;
            public int checks;
            public string completedUtc;
            public string[] failures;
            public string evidence = "Production policy data fixtures; no AssetBundle or rendered-card acceptance";
        }
        [Serializable] private class CurrentStatus
        {
            public string mode, effectiveMode, source, reason, message, quality;
            public bool enabled, canAnimate, qualityPreferenceExists, legacyPreferenceExists;
            public int qualityPreference, legacyPreference, catalogEntries, missingSourcePrefabs;
        }
        private static string EvidenceRoot
        {
            get { return Path.GetFullPath("../../../../work/PremiumEditorReadiness20261002"); }
        }

        static DynamicCardEditorContentVerification()
        {
            // A bounded command inbox supports the already-open 2019 editor without
            // MCP. One file-presence check per second; no directory/resource scan.
            EditorApplication.update += CheckCommand;
        }

        private static void CheckCommand()
        {
            if (Application.isBatchMode || EditorApplication.isCompiling || BuildPipeline.isBuildingPlayer ||
                EditorApplication.timeSinceStartup < nextCommandCheck) return;
            nextCommandCheck = EditorApplication.timeSinceStartup + 1;
            string path = Path.Combine(EvidenceRoot, "command.json");
            if (!File.Exists(path)) return;
            try
            {
                var command = JsonUtility.FromJson<Command>(File.ReadAllText(path));
                File.Delete(path); // Consume once before execution, including a failing fixture.
                if (command == null) throw new Exception("Invalid editor verification command.");
                if (command.action == "status") WriteStatus();
                else if (command.action == "test") Run();
                else throw new Exception("Use editor verification action status or test.");
            }
            catch (Exception exception) { Debug.LogError("Dynamic card editor verification: " + exception.Message); }
        }

        [MenuItem("Tools/Dynamic Cards/Verification/Write current preview status")]
        public static void WriteStatus()
        {
            var status = DynamicCardEditorContentPolicy.Evaluate();
            var result = new CurrentStatus {
                mode = status.Mode.ToString(), effectiveMode = status.EffectiveMode.ToString(),
                source = status.Source, reason = status.Reason, message = status.Message,
                quality = DynamicCardSettings.Quality.ToString(), enabled = DynamicCardSettings.Enabled,
                canAnimate = ClientContent.CanAnimate, catalogEntries = status.CatalogEntries,
                missingSourcePrefabs = status.MissingSourcePrefabs,
                qualityPreferenceExists = PlayerPrefs.HasKey(QualityPreference),
                legacyPreferenceExists = PlayerPrefs.HasKey(LegacyPreference),
                qualityPreference = PlayerPrefs.GetInt(QualityPreference, -1),
                legacyPreference = PlayerPrefs.GetInt(LegacyPreference, -1)
            };
            Directory.CreateDirectory(EvidenceRoot);
            File.WriteAllText(Path.Combine(EvidenceRoot, "status.json"), JsonUtility.ToJson(result, true));
            Debug.Log("Dynamic card preview status written: " + Path.Combine(EvidenceRoot, "status.json"));
        }

        [MenuItem("Tools/Dynamic Cards/Verify Editor Content Policy")]
        public static void Run()
        {
            string root = Path.Combine(Path.GetTempPath(), "LegacyGwent-PremiumPolicy-" + Guid.NewGuid().ToString("N"));
            var failures = new List<string>();
            int checks = 0;
            var beforeQuality = DynamicCardSettings.Quality;
            var beforeMode = DynamicCardEditorContentPolicy.Mode;
            bool hadQuality = PlayerPrefs.HasKey(QualityPreference);
            bool hadLegacy = PlayerPrefs.HasKey(LegacyPreference);
            int quality = PlayerPrefs.GetInt(QualityPreference, -1);
            int legacy = PlayerPrefs.GetInt(LegacyPreference, -1);
            Action<string, Action> test = (name, body) =>
            {
                checks++;
                try { Reset(root); body(); }
                catch (Exception exception) { failures.Add(name + ": " + exception.Message); }
            };
            try
            {
                test("Auto chooses complete sources when bundle missing", () => Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "bundle-missing"));
                test("Auto prefers complete bundle closure", () => { Bundle(root); Expect(root, DynamicCardEditorContentMode.Automatic, false, "bundles", "bundles-ready"); });
                test("SourceOnly prefers authored resources", () => { Bundle(root); Expect(root, DynamicCardEditorContentMode.SourceOnly, false, "source", "source-requested"); });
                test("BundlesOnly never falls back", () => Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "bundle-missing"));
                test("Batch Automatic remains strict", () => {
                    var result = Expect(root, DynamicCardEditorContentMode.Automatic, true, "unavailable", "bundle-missing");
                    Assert(result.EffectiveMode == DynamicCardEditorContentMode.BundlesOnly, "batch effective mode");
                });
                test("Ready marker missing", () => { Bundle(root); File.Delete(BundlePath(root) + ".editor-ready"); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "ready-missing"); });
                test("Index missing", () => { Bundle(root); File.Delete(IndexPath(root)); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "index-missing"); });
                test("Index invalid JSON", () => { Bundle(root); Write(IndexPath(root), "broken"); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "index-invalid"); });
                test("Index wrong schema", () => { Bundle(root); Write(IndexPath(root), IndexJson(Part, Prefab, 1)); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "index-invalid"); });
                test("Index empty parts", () => { Bundle(root); Write(IndexPath(root), "{\"version\":2,\"parts\":[]}"); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "index-invalid"); });
                test("Partition missing", () => { Bundle(root); File.Delete(Path.Combine(BundleRoot(root), Part)); Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "part-missing"); });
                test("Partition empty", () => { Bundle(root); Write(Path.Combine(BundleRoot(root), Part), ""); Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "part-missing"); });
                test("Catalog/index mapping mismatch", () => { Bundle(root); Write(IndexPath(root), IndexJson(Part, "Assets/DynamicCards/Content/other.prefab", 2)); Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "index-catalog-mismatch"); });
                test("Unsafe partition path", () => { Bundle(root); Write(IndexPath(root), IndexJson("../cards-escape.bundle", Prefab, 2)); Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "index-invalid"); });
                test("Unsafe source path", () => { Write(CatalogPath(root), CatalogJson("Assets/DynamicCards/Content/../escape.prefab")); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid"); });
                test("Empty catalog", () => { Write(CatalogPath(root), "{\"version\":1,\"cards\":[]}"); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid"); });
                test("Wrong catalog version", () => { Write(CatalogPath(root), CatalogJson(Prefab).Replace("\"version\":1", "\"version\":2")); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid"); });
                test("Missing catalog version", () => { Write(CatalogPath(root), CatalogJson(Prefab).Replace("\"version\":1,", "")); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid"); });
                test("Missing index version", () => { Bundle(root); Write(IndexPath(root), IndexJson(Part, Prefab, 2).Replace("\"version\":2,", "")); Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "index-invalid"); });
                test("Invalid catalog JSON", () => { Write(CatalogPath(root), "broken"); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid"); });
                test("Missing catalog", () => { File.Delete(CatalogPath(root)); Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-missing"); });
                test("Missing source prefab is explicit", () => { File.Delete(Path.Combine(root, Prefab)); var result = Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-prefabs-missing"); Assert(result.MissingSourcePrefabs == 1, "missing count"); });
                test("Mixed mapped and unmapped authored scenes are valid", () => {
                    const string unmapped = "Assets/DynamicCards/Content/Old/Thronebreaker/unmapped/Card.prefab";
                    Write(Path.Combine(root, unmapped), "fixture unmapped prefab; presence only");
                    string catalog = CatalogJson(Prefab);
                    Write(CatalogPath(root), catalog.Substring(0, catalog.Length - 2) +
                        ",{\"id\":\"unmapped\",\"artIds\":[],\"prefab\":\"" + unmapped + "\"}]}");
                    var result = Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "bundle-missing");
                    Assert(result.CatalogEntries == 2 && result.MissingSourcePrefabs == 0, "unmapped entry was lost");
                    Bundle(root);
                    Write(IndexPath(root), "{\"version\":2,\"parts\":[{\"file\":\"" + Part +
                        "\",\"animationControllers\":0,\"prefabs\":[\"" + Prefab + "\",\"" + unmapped + "\"]}]}");
                    Expect(root, DynamicCardEditorContentMode.Automatic, false, "bundles", "bundles-ready");
                });
                test("Catalog with no mapped art is rejected", () => {
                    Write(CatalogPath(root), CatalogJson(Prefab).Replace("[\"fixture-art\"]", "[]"));
                    Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid");
                });
                test("Duplicate art across scenes is rejected", () => {
                    const string other = "Assets/DynamicCards/Content/Old/Thronebreaker/other/Card.prefab";
                    Write(Path.Combine(root, other), "fixture duplicate-art prefab; presence only");
                    string catalog = CatalogJson(Prefab);
                    Write(CatalogPath(root), catalog.Substring(0, catalog.Length - 2) +
                        ",{\"id\":\"other\",\"artIds\":[\"fixture-art\"],\"prefab\":\"" + other + "\"}]}");
                    Expect(root, DynamicCardEditorContentMode.Automatic, false, "unavailable", "source-catalog-invalid");
                });
                test("Empty root bundle rejected", () => { Bundle(root); Write(BundlePath(root), ""); Expect(root, DynamicCardEditorContentMode.BundlesOnly, false, "unavailable", "bundle-missing"); });
                test("Policy does not change quality or mode preferences", () => {
                    Expect(root, DynamicCardEditorContentMode.Automatic, false, "source", "bundle-missing");
                    Assert(DynamicCardSettings.Quality == beforeQuality, "quality changed, including Off");
                    Assert(DynamicCardEditorContentPolicy.Mode == beforeMode, "mode changed");
                    Assert(PlayerPrefs.HasKey(QualityPreference) == hadQuality &&
                        PlayerPrefs.HasKey(LegacyPreference) == hadLegacy &&
                        PlayerPrefs.GetInt(QualityPreference, -1) == quality &&
                        PlayerPrefs.GetInt(LegacyPreference, -1) == legacy, "preference storage changed");
                });
            }
            finally
            {
                // Only the GUID-named fixture created above is removed.
                if (Directory.Exists(root)) Directory.Delete(root, true);
                Directory.CreateDirectory(EvidenceRoot);
                File.WriteAllText(Path.Combine(EvidenceRoot, "tests.json"), JsonUtility.ToJson(new Result {
                    passed = failures.Count == 0, checks = checks, failures = failures.ToArray(),
                    completedUtc = DateTime.UtcNow.ToString("O") }, true));
            }
            if (failures.Count != 0) throw new Exception("Dynamic card editor readiness fixtures failed: " + string.Join("; ", failures.ToArray()));
            Debug.Log("Dynamic card editor readiness fixtures passed: " + checks + "; " + Path.Combine(EvidenceRoot, "tests.json"));
        }

        private static DynamicCardEditorContentPolicy.Status Expect(string root, DynamicCardEditorContentMode mode, bool batch, string source, string reason)
        {
            var result = DynamicCardEditorContentPolicy.Evaluate(root, mode, batch);
            Assert(result.Source == source, "expected source " + source + ", got " + result.Source);
            Assert(result.Reason == reason, "expected reason " + reason + ", got " + result.Reason);
            return result;
        }
        private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
        private static void Reset(string root)
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
            Write(CatalogPath(root), CatalogJson(Prefab));
            Write(Path.Combine(root, Prefab), "fixture prefab; presence only");
        }
        private static void Bundle(string root)
        {
            Write(BundlePath(root), "fixture root bundle; presence only");
            Write(BundlePath(root) + ".editor-ready", "fixture");
            Write(Path.Combine(BundleRoot(root), Part), "fixture partition; presence only");
            Write(IndexPath(root), IndexJson(Part, Prefab, 2));
        }
        private static string CatalogJson(string prefab) { return "{\"version\":1,\"cards\":[{\"id\":\"fixture\",\"artIds\":[\"fixture-art\"],\"prefab\":\"" + prefab + "\"}]}"; }
        private static string IndexJson(string part, string prefab, int version) { return "{\"version\":" + version + ",\"parts\":[{\"file\":\"" + part + "\",\"animationControllers\":0,\"prefabs\":[\"" + prefab + "\"]}]}"; }
        private static string CatalogPath(string root) { return Path.Combine(root, DynamicCardLibrary.CatalogAsset); }
        private static string BundleRoot(string root) { return Path.Combine(root, "Library/DynamicCardsBundles/StandaloneWindows64"); }
        private static string BundlePath(string root) { return Path.Combine(BundleRoot(root), DynamicCardLibrary.BundleFile); }
        private static string IndexPath(string root) { return Path.Combine(BundleRoot(root), DynamicCardLibrary.BundleIndexFile); }
        private static void Write(string path, string text) { Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllText(path, text); }
    }
}
