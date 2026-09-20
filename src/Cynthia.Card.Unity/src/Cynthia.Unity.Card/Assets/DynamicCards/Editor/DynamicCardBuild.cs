using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    [InitializeOnLoad]
    public sealed class DynamicCardBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        [Serializable] private class Options { public bool includeAnimatedCards; }
        [Serializable] private class Stage { public bool hadBundle, hadMeta; public string[] originalFiles, stagedFiles; }
        private const string OptionsPath = "ProjectSettings/DynamicCardsBuild.json";
        private const string BackupRoot = "Library/DynamicCardsBuildBackup";
        private const string StreamingRoot = "Assets/StreamingAssets/DynamicCards";
        private const string StageFile = BackupRoot + "/stage.json";
        public int callbackOrder { get { return 100; } }

        static DynamicCardBuild()
        {
            EditorApplication.delayCall += () => { if (!BuildPipeline.isBuildingPlayer) RestoreStage(); };
            BuildPlayerWindow.RegisterBuildPlayerHandler(options =>
            {
                try
                {
                    if (IncludeContent) BuildBundle(options.target);
                    BuildPipeline.BuildPlayer(options);
                }
                finally { RestoreStage(); }
            });
        }

        public static bool IncludeContent
        {
            get
            {
                var environment = Environment.GetEnvironmentVariable("LEGACY_GWENT_DYNAMIC_CARDS");
                if (environment == "1" || environment == "0") return environment == "1";
                return File.Exists(OptionsPath) && JsonUtility.FromJson<Options>(File.ReadAllText(OptionsPath)).includeAnimatedCards;
            }
            set { File.WriteAllText(OptionsPath, JsonUtility.ToJson(new Options { includeAnimatedCards = value }, true)); }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            RestoreStage();
            Directory.CreateDirectory(BackupRoot);
            var payload = IncludeContent ? DynamicCardBundleBuilder.PayloadFiles(Path.GetDirectoryName(PreparedBundle(report.summary.platform))) : new string[0];
            var original = Directory.Exists(StreamingRoot) ? Directory.GetFiles(StreamingRoot).Where(IsPayloadFile).Select(Path.GetFileName).ToArray() : new string[0];
            var staged = original.Concat(payload.SelectMany(p => new[] { Path.GetFileName(p), Path.GetFileName(p) + ".meta" })).Distinct().ToArray();
            var stage = new Stage { originalFiles = original, stagedFiles = staged };
            foreach (var name in original) File.Copy(Path.Combine(StreamingRoot, name), Path.Combine(BackupRoot, name), true);
            File.WriteAllText(StageFile, JsonUtility.ToJson(stage));
            try
            {
                foreach (var name in original) File.Delete(Path.Combine(StreamingRoot, name));
                if (IncludeContent)
                {
                    Directory.CreateDirectory(StreamingRoot);
                    foreach (var file in payload) File.Copy(file, Path.Combine(StreamingRoot, Path.GetFileName(file)), true);
                }
                AssetDatabase.Refresh();
            }
            catch { RestoreStage(); throw; }
        }

        public void OnPostprocessBuild(BuildReport report) { RestoreStage(); }

        public static string BuildBundle(BuildTarget target)
        {
            if (!File.Exists(DynamicCardLibrary.CatalogAsset)) throw new BuildFailedException("Dynamic card content has not been imported. Disable animated content or import it first.");
            if (EditorApplication.isPlaying) throw new BuildFailedException("Stop Play Mode before rebuilding dynamic card packages.");
            if(target==BuildTarget.StandaloneWindows64 || target==BuildTarget.StandaloneWindows)
                DynamicCardTextureCompression.Apply();
            string directory = "Library/DynamicCardsBundles/" + target;
            string bundle = DynamicCardBundleBuilder.Build(target);
            File.WriteAllText(directory + "/content.hash", ContentHash());
            if(target==BuildTarget.StandaloneWindows64)
            {
                DynamicCardEditorCache.WriteManifest();
                File.WriteAllText(directory+"/"+DynamicCardLibrary.BundleFile+".editor-ready",DateTime.UtcNow.ToString("O"));
            }
            return bundle;
        }

        // Unity 2019 cannot nest BuildAssetBundles inside a player-build callback.
        // Standard Build/Build And Run prepare automatically through the registered handler.
        public static void PrepareForBuild()
        { if (IncludeContent) BuildBundle(EditorUserBuildSettings.activeBuildTarget); }

        private static string PreparedBundle(BuildTarget target)
        {
            string directory = "Library/DynamicCardsBundles/" + target;
            string bundle = directory + "/" + DynamicCardLibrary.BundleFile;
            string stamp = directory + "/content.hash";
            if (!File.Exists(bundle) || !File.Exists(stamp) || File.ReadAllText(stamp) != ContentHash())
                throw new BuildFailedException("Animated content is missing or changed. Run DynamicCardBuild.PrepareForBuild before a programmatic BuildPipeline.BuildPlayer call, or use the standard Build window.");
            return bundle;
        }

        private static string ContentHash()
        {
            var paths = Directory.GetFiles(DynamicCardLibrary.ContentRoot, "*", SearchOption.AllDirectories)
                .Where(p => !p.EndsWith(".meta")).Select(p => p.Replace('\\', '/')).OrderBy(p => p);
            var text = "bundle-index-v" + DynamicCardBundleIndex.CurrentVersion + "\n" + string.Join("\n", paths.Select(p => p + ":" + AssetDatabase.GetAssetDependencyHash(p)));
            using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        public static void RestoreStage()
        {
            if (!File.Exists(StageFile)) return;
            var stage = JsonUtility.FromJson<Stage>(File.ReadAllText(StageFile));
            if (stage.originalFiles != null)
            {
                foreach (var name in stage.stagedFiles ?? new string[0])
                {
                    if (Path.GetFileName(name) != name || !IsPayloadFile(name)) throw new BuildFailedException("Invalid dynamic card staging record.");
                    string path = Path.Combine(StreamingRoot, name);
                    if (File.Exists(path)) File.Delete(path);
                }
                foreach (var name in stage.originalFiles)
                {
                    if (Path.GetFileName(name) != name || !IsPayloadFile(name)) throw new BuildFailedException("Invalid dynamic card backup record.");
                    Directory.CreateDirectory(StreamingRoot);
                    File.Copy(Path.Combine(BackupRoot, name), Path.Combine(StreamingRoot, name), true);
                }
                File.Delete(StageFile);
                return;
            }
            string destination = StreamingRoot + "/" + DynamicCardLibrary.BundleFile;
            if (File.Exists(destination)) File.Delete(destination);
            if (File.Exists(destination + ".meta")) File.Delete(destination + ".meta");
            if (stage.hadBundle) { Directory.CreateDirectory(StreamingRoot); File.Copy(BackupRoot + "/original.bundle", destination, true); }
            if (stage.hadMeta) File.Copy(BackupRoot + "/original.meta", destination + ".meta", true);
            File.Delete(StageFile);
        }

        private static bool IsPayloadFile(string path)
        {
            string name = Path.GetFileName(path);
            if (name.EndsWith(".meta")) name = name.Substring(0, name.Length - 5);
            return name == DynamicCardLibrary.BundleIndexFile || (name.StartsWith("cards") && name.EndsWith(".bundle"));
        }
    }

    public sealed class DynamicCardBuildWindow : EditorWindow
    {
        [MenuItem("Tools/Dynamic Cards/Build Options")]
        public static void Open() { GetWindow<DynamicCardBuildWindow>("Dynamic cards"); }
        private void OnGUI()
        {
            EditorGUILayout.LabelField("动态卡资源包 / Animated card content", EditorStyles.boldLabel);
            bool enabled = EditorGUILayout.Toggle("打包动态卡资源", DynamicCardBuild.IncludeContent);
            if (enabled != DynamicCardBuild.IncludeContent) DynamicCardBuild.IncludeContent = enabled;
            EditorGUILayout.HelpBox("关闭时，正常 Build 不包含动态卡素材；开启时，为当前目标平台构建独立资源包并随游戏发布。游戏内开关独立保存。", MessageType.Info);
            EditorGUILayout.LabelField("目标平台", EditorUserBuildSettings.activeBuildTarget.ToString());
            if (GUILayout.Button("打开 Unity Build Settings")) EditorWindow.GetWindow<BuildPlayerWindow>();
            if (GUILayout.Button("仅构建动态卡资源包"))
            {
                var path = DynamicCardBuild.BuildBundle(EditorUserBuildSettings.activeBuildTarget);
                Debug.Log("Dynamic card bundle: " + Path.GetFullPath(path));
            }
            EditorGUILayout.Space();
            bool sourceLoading = EditorGUILayout.Toggle("开发用：允许同步读取原资源", DynamicCardLibrary.AllowEditorSourceLoading);
            if (sourceLoading != DynamicCardLibrary.AllowEditorSourceLoading) DynamicCardLibrary.AllowEditorSourceLoading = sourceLoading;
            if (sourceLoading)
                EditorGUILayout.HelpBox("仅用于素材调试：缓存不可用时直接读取工程资源，可能导致收藏界面长时间停顿。修改后重新进入 Play Mode 生效。", MessageType.Warning);
        }
    }
}
