using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.DynamicCards.Editor
{
    // Explicit local benchmark commands. Generated scene is temporary and original scene setup is restored.
    [InitializeOnLoad]
    public static class DynamicCardQualityBenchmarkEditor
    {
        private static readonly string Work = Path.GetFullPath("../../../../work/DynamicCards/QualityBenchmark");
        private const string GeneratedScene = "Assets/DynamicCards/Editor/__QualityBenchmark.unity";
        private const string Running = "DynamicCards.QualityBenchmark.Running";
        private const string PreviousStartScene = "DynamicCards.QualityBenchmark.PreviousStartScene";
        private static double nextPoll;

        static DynamicCardQualityBenchmarkEditor()
        { EditorApplication.update += Poll; EditorApplication.playModeStateChanged += StateChanged; }

        private static void Poll()
        {
            if (EditorApplication.timeSinceStartup < nextPoll || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            nextPoll = EditorApplication.timeSinceStartup + 1;
            string result = Path.Combine(Work, "Editor/result.json");
            if (SessionState.GetBool(Running, false) && EditorApplication.isPlaying && File.Exists(result) &&
                Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(result)).Value<bool>("complete"))
            { EditorApplication.isPlaying = false; return; }
            string request = Path.Combine(Work, "request.txt");
            if (!File.Exists(request) || EditorApplication.isPlayingOrWillChangePlaymode) return;
            string command = File.ReadAllText(request).Trim(); File.Delete(request);
            try
            {
                if (command == "editor") RunInEditor();
                else if (command == "build") BuildPlayer();
                else if (command == "scripts") BuildPlayer(true);
                else throw new ArgumentException("Unknown quality benchmark command: " + command);
            }
            catch (Exception exception)
            { File.WriteAllText(Path.Combine(Work, "command-error.txt"), exception.ToString()); Debug.LogException(exception); }
        }

        [MenuItem("Tools/Dynamic Cards/Quality/Run repeatable benchmark in Play Mode")]
        public static void RunInEditor()
        {
            EnsureIdle(); Directory.CreateDirectory(Path.Combine(Work, "Editor"));
            string result = Path.Combine(Work, "Editor/result.json"); if (File.Exists(result)) File.Delete(result);
            CreateScene(false);
            SessionState.SetString(PreviousStartScene, AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene));
            SessionState.SetBool(Running, true);
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(GeneratedScene);
            EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView")).Show();
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Tools/Dynamic Cards/Quality/Build Windows benchmark player")]
        public static void BuildPlayer()
        { BuildPlayer(false); }

        private static void BuildPlayer(bool scriptsOnly)
        {
            EnsureIdle(); Directory.CreateDirectory(Work);
            File.WriteAllText(Path.Combine(Work, "build-result.txt"), "RUNNING");
            string previousContent = Environment.GetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS");
            bool previousTiming = PlayerSettings.enableFrameTimingStats;
            byte[] previousSettings = File.ReadAllBytes("ProjectSettings/ProjectSettings.asset");
            try
            {
                CreateScene(true);
                // The benchmark packages the currently validated platform cache after the player build.
                // No resource reimport/recompression and no changes to normal release build options.
                Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS", "0");
                PlayerSettings.enableFrameTimingStats = true;
                string player = Path.Combine(Work, "Player/CardQualityBenchmark.exe");
                Directory.CreateDirectory(Path.GetDirectoryName(player));
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { GeneratedScene }, target = BuildTarget.StandaloneWindows64,
                    locationPathName = player, options = BuildOptions.Development | (scriptsOnly ? BuildOptions.BuildScriptsOnly : BuildOptions.None)
                });
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException("Benchmark player build failed: " + report.summary.result + " errors=" + report.summary.totalErrors);
                string destination = Path.Combine(Work, "Player/CardQualityBenchmark_Data/StreamingAssets/DynamicCards");
                Directory.CreateDirectory(destination);
                var files = DynamicCardBundleBuilder.PayloadFiles("Library/DynamicCardsBundles/StandaloneWindows64");
                foreach (var file in files) File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                File.WriteAllText(Path.Combine(Work, "build-result.txt"), "Succeeded errors=" + report.summary.totalErrors + " payloadFiles=" + files.Length);
            }
            catch (Exception exception) { File.WriteAllText(Path.Combine(Work, "build-result.txt"), exception.ToString()); throw; }
            finally
            {
                PlayerSettings.enableFrameTimingStats = previousTiming;
                AssetDatabase.SaveAssets();
                // Older projects otherwise acquire unrelated defaults when Unity serializes PlayerSettings.
                File.WriteAllBytes("ProjectSettings/ProjectSettings.asset", previousSettings);
                Environment.SetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS", previousContent);
                AssetDatabase.DeleteAsset(GeneratedScene);
            }
        }

        private static void EnsureIdle()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode before starting a benchmark.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save the open scene before starting a benchmark.");
        }

        private static void CreateScene(bool player)
        {
            var catalog = JsonUtility.FromJson<DynamicCardCatalog>(File.ReadAllText(DynamicCardLibrary.CatalogAsset));
            var selected = new List<DynamicCardEntry>();
            foreach (var source in new[] { "/Thronebreaker/", "/Legacy2017/", "/Latest/" })
                selected.AddRange(catalog.cards.Where(c => c.prefab.Contains(source) && c.artIds != null && c.artIds.Length > 0).OrderBy(c => c.id).Take(6));
            foreach (var id in new[] { "15210401", "11220601", "13330100", "15231401", "11240100", "11770100", "13380100" })
            {
                var card = catalog.cards.FirstOrDefault(c => c.id == id && c.artIds != null && c.artIds.Length > 0);
                if (card != null && !selected.Contains(card) && selected.Count < 24) selected.Add(card);
            }
            selected.Add(catalog.cards.First(c => c.id == "11770100")); // Original bloom/postprocessing preview.
            var setup = EditorSceneManager.GetSceneManagerSetup();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                var root = new GameObject("Explicit dynamic card quality benchmark"); SceneManager.MoveGameObjectToScene(root, scene);
                var benchmark = root.AddComponent<DynamicCardQualityBenchmark>();
                benchmark.ArtIds = selected.Select(c => c.artIds[0]).ToArray();
                benchmark.Artworks = benchmark.ArtIds.Select(id => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Addressables/Cards/" + id + ".png")).ToArray();
                benchmark.SampleSeconds = 12; benchmark.QuitWhenComplete = player;
                benchmark.OutputDirectory = Path.Combine(Work, player ? "Standalone" : "Editor");
                EditorSceneManager.SaveScene(scene, GeneratedScene);
            }
            finally { EditorSceneManager.CloseScene(scene, true); EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }

        private static void StateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode || !SessionState.GetBool(Running, false)) return;
            SessionState.SetBool(Running, false);
            string previous = SessionState.GetString(PreviousStartScene, "");
            EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(previous) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(previous);
            AssetDatabase.DeleteAsset(GeneratedScene);
        }
    }
}
