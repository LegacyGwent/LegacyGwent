using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    // Small, resumable builds avoid loading every premium scene into the editor at once.
    public static class DynamicCardBundleBuilder
    {
        public const int CardsPerPart = 32;
        [Serializable] private class SourceEntry { public string path, hash; public long length, ticks; }
        [Serializable] private class SourceCache { public SourceEntry[] files; }
        private static Dictionary<string, SourceEntry> sourceCache;
        private static int sourceHashReads;
        private static int savedSourceHashReads;

        private static void LoadSourceCache(string directory)
        {
            sourceCache = new Dictionary<string, SourceEntry>(StringComparer.Ordinal);
            sourceHashReads = 0;
            savedSourceHashReads = 0;
            string path = directory + "/cards.source-hashes.json";
            if (!File.Exists(path)) return;
            try
            {
                var cache = JsonUtility.FromJson<SourceCache>(File.ReadAllText(path));
                if (cache != null && cache.files != null)
                    foreach (var entry in cache.files) sourceCache[entry.path] = entry;
            }
            catch (Exception exception) { Debug.LogWarning("Recalculating dynamic card source hashes: " + exception.Message); }
        }

        private static void SaveSourceCache(string directory)
        {
            string path = directory + "/cards.source-hashes.json";
            if (sourceHashReads == savedSourceHashReads && File.Exists(path)) return;
            File.WriteAllText(path, JsonUtility.ToJson(new SourceCache { files = sourceCache.Values.ToArray() }));
            savedSourceHashReads = sourceHashReads;
        }

        internal static bool TryGetBuildHash(string path, out string hash)
        {
            hash = null;
            SourceEntry cached;
            path = path.Replace('\\', '/');
            if (sourceCache == null || !sourceCache.TryGetValue(path, out cached)) return false;
            var file = new FileInfo(path);
            if (!file.Exists || cached.length != file.Length || cached.ticks != file.LastWriteTimeUtc.Ticks) return false;
            hash = cached.hash;
            return true;
        }

        public static string Build(BuildTarget target)
        {
            var catalog = JsonUtility.FromJson<DynamicCardCatalog>(File.ReadAllText(DynamicCardLibrary.CatalogAsset));
            var cards = catalog.cards.OrderBy(c => c.prefab, StringComparer.Ordinal).ToArray();
            if (cards.Any(c => !c.prefab.StartsWith(DynamicCardLibrary.ContentRoot + "Old/Thronebreaker/", StringComparison.Ordinal) &&
                               !c.prefab.StartsWith(DynamicCardLibrary.ContentRoot + "Old/Legacy2017/", StringComparison.Ordinal) &&
                               !c.prefab.StartsWith(DynamicCardLibrary.ContentRoot + "Latest/", StringComparison.Ordinal)))
                throw new BuildFailedException("Premium scenes must use an explicit supported source directory.");
            if (cards.GroupBy(c => c.id).Any(g => g.Count() != 1) ||
                cards.SelectMany(c => c.artIds ?? new string[0]).GroupBy(id => id).Any(g => g.Count() != 1))
                throw new BuildFailedException("Each premium scene and card art must have one catalog entry.");
            DynamicCardMaterialValidation.Validate(cards);
            DynamicCardSkinValidation.Validate(cards);
            int batchSize;
            if (!int.TryParse(Environment.GetEnvironmentVariable("DYNAMIC_CARDS_PER_PART"), out batchSize)) batchSize = CardsPerPart;
            batchSize = Math.Max(1, Math.Min(CardsPerPart, batchSize));
            string directory = "Library/DynamicCardsBundles/" + target;
            Directory.CreateDirectory(directory);
            LoadSourceCache(directory);
            string ready = directory + "/" + DynamicCardLibrary.BundleFile + ".editor-ready";
            if (File.Exists(ready)) File.Delete(ready);
            var parts = new List<DynamicCardBundlePart>();
            var included = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fileHashes = new Dictionary<string, string>(StringComparer.Ordinal);
            int completed = 0;
            // Keep existing source partitions stable when adding missing modern cards.
            foreach (string source in new[] { "Thronebreaker", "Legacy2017", "Latest" })
            {
                string prefix = DynamicCardLibrary.ContentRoot + (source == "Latest" ? "Latest/" : "Old/" + source + "/");
                var cohort = cards.Where(c => c.prefab.StartsWith(prefix, StringComparison.Ordinal)).ToArray();
                for (int offset = 0; offset < cohort.Length; offset += batchSize)
                {
                    var group = cohort.Skip(offset).Take(batchSize).ToArray();
                    var assets = group.SelectMany(c => new[] { c.prefab, c.audio }).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToArray();
                    // Implicit dependencies cannot be loaded by type from an AssetBundle.
                    // Explicit roots let the runtime initialize all animation graphs without
                    // loading the other cards' renderers and textures in this partition.
                    var controllers = AssetDatabase.GetDependencies(assets, true)
                        .Where(p => p.EndsWith(".controller", StringComparison.OrdinalIgnoreCase) || p.EndsWith(".overrideController", StringComparison.OrdinalIgnoreCase))
                        .Distinct().OrderBy(p => p, StringComparer.Ordinal).ToArray();
                    assets = assets.Concat(controllers).Distinct().ToArray();
                    foreach (var asset in assets) included.Add(asset);
                    var part = new DynamicCardBundlePart { file = "cards-" + source.ToLowerInvariant() + "-" + (offset / batchSize).ToString("000") + ".bundle", prefabs = group.Select(c => c.prefab).ToArray() };
                    part.animationControllers = controllers.Length;
                    BuildOne(directory, part.file, assets, part.prefabs, target, fileHashes);
                    parts.Add(part);
                    completed += group.Length;
                    Debug.Log("DYNAMIC_PART_PROGRESS cards=" + completed + "/" + cards.Length);
                }
            }
            var extras = Directory.GetFiles(DynamicCardLibrary.ContentRoot + "Old", "*", SearchOption.AllDirectories)
                .Select(p => p.Replace('\\', '/')).Where(p => (p.EndsWith(".wav") || p.EndsWith(".bytes")) && !included.Contains(p)).OrderBy(p => p).ToArray();
            for (int offset = 0; offset < extras.Length; offset += CardsPerPart)
            {
                var part = new DynamicCardBundlePart { file = "cards-extra-" + (offset / CardsPerPart).ToString("000") + ".bundle", prefabs = new string[0] };
                BuildOne(directory, part.file, extras.Skip(offset).Take(CardsPerPart).ToArray(), part.prefabs, target, fileHashes);
                parts.Add(part);
            }
            BuildOne(directory, DynamicCardLibrary.BundleFile, new[] { DynamicCardLibrary.CatalogAsset }, new string[0], target, fileHashes);
            File.WriteAllText(directory + "/" + DynamicCardLibrary.BundleIndexFile, JsonUtility.ToJson(new DynamicCardBundleIndex { parts = parts.ToArray() }, true));
            Debug.Log("DYNAMIC_PARTITION_BUILD_DONE cards=" + cards.Length + " parts=" + parts.Count);
            return directory + "/" + DynamicCardLibrary.BundleFile;
        }

        private static string SourceHash(string path, Dictionary<string, string> hashes)
        {
            string hash;
            if (hashes.TryGetValue(path, out hash)) return hash;
            var file = new FileInfo(path);
            SourceEntry cached;
            if (sourceCache.TryGetValue(path, out cached) && cached.length == file.Length && cached.ticks == file.LastWriteTimeUtc.Ticks)
                hash = cached.hash;
            else
            {
                sourceHashReads++;
                using (var stream = File.OpenRead(path))
                using (var sha = SHA256.Create()) hash = Convert.ToBase64String(sha.ComputeHash(stream));
                sourceCache[path] = new SourceEntry { path = path, hash = hash, length = file.Length, ticks = file.LastWriteTimeUtc.Ticks };
            }
            hashes.Add(path, hash);
            return hash;
        }

        private static void BuildOne(string directory, string file, string[] assets, string[] prefabs, BuildTarget target, Dictionary<string, string> fileHashes)
        {
            string path = directory + "/" + file;
            string stamp = path + ".inputs";
            // Unity 2019's dependency hash did not invalidate these bundles after a bulk
            // texture reimport. Include actual dependency and importer bytes; hash shared
            // files once per build so unchanged parts remain safely reusable.
            var dependencies = AssetDatabase.GetDependencies(assets, true).Concat(assets)
                .SelectMany(p => new[] { p, p + ".meta" }).Where(File.Exists)
                .Distinct().OrderBy(p => p, StringComparer.Ordinal);
            string input = Application.unityVersion + "\n" + target + "\npartition-source-v2\n" +
                string.Join("\n", assets) + "\n" +
                string.Join("\n", dependencies.Select(p => p + ":" + SourceHash(p, fileHashes)));
            string hash;
            using (var sha = SHA256.Create()) hash = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
            bool reusable = File.Exists(path) && File.Exists(stamp) && File.ReadAllText(stamp) == hash + ":" + new FileInfo(path).Length;
            if (!reusable)
            {
                Debug.Log("DYNAMIC_PART_BUILD " + file + " assets=" + assets.Length);
                var build = new AssetBundleBuild { assetBundleName = file, assetNames = assets };
                var result = BuildPipeline.BuildAssetBundles(directory, new[] { build }, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ForceRebuildAssetBundle, target);
                if (result == null) throw new BuildFailedException("Dynamic card part failed: " + file);
            }
            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null) throw new BuildFailedException("Dynamic card part cannot be read: " + file);
            try
            {
                var actual = new HashSet<string>(bundle.GetAllAssetNames().Where(p => p.EndsWith("/card.prefab")), StringComparer.OrdinalIgnoreCase);
                if (!actual.SetEquals(prefabs)) throw new BuildFailedException("Dynamic card part has incorrect scenes: " + file);
                var animationRoots = new HashSet<string>(bundle.GetAllAssetNames().Where(IsAnimationController), StringComparer.OrdinalIgnoreCase);
                if (!animationRoots.SetEquals(assets.Where(IsAnimationController))) throw new BuildFailedException("Dynamic card part has incorrect animation roots: " + file);
            }
            finally { bundle.Unload(true); }
            File.WriteAllText(stamp, hash + ":" + new FileInfo(path).Length);
            SaveSourceCache(directory);
            Debug.Log("DYNAMIC_PART_READY " + file + " bytes=" + new FileInfo(path).Length + " reused=" + reusable + " sourceHashReads=" + sourceHashReads);
            EditorUtility.UnloadUnusedAssetsImmediate();
            GC.Collect();
        }

        private static bool IsAnimationController(string path)
        { return path.EndsWith(".controller", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".overrideController", StringComparison.OrdinalIgnoreCase); }

        public static string[] PayloadFiles(string directory)
        {
            var names = new List<string> { DynamicCardLibrary.BundleFile };
            string indexPath = Path.Combine(directory, DynamicCardLibrary.BundleIndexFile);
            if (File.Exists(indexPath))
            {
                var index = JsonUtility.FromJson<DynamicCardBundleIndex>(File.ReadAllText(indexPath));
                if (index == null || index.version != DynamicCardBundleIndex.CurrentVersion || index.parts == null) throw new BuildFailedException("Dynamic card packages need rebuilding with animation dependencies (index v2).");
                names.Add(DynamicCardLibrary.BundleIndexFile);
                foreach (var part in index.parts)
                {
                    if (Path.GetFileName(part.file) != part.file || !part.file.StartsWith("cards-") || !part.file.EndsWith(".bundle")) throw new BuildFailedException("Invalid dynamic card part filename.");
                    names.Add(part.file);
                }
            }
            var paths = names.Distinct().Select(name => Path.Combine(directory, name)).ToArray();
            foreach (var path in paths) if (!File.Exists(path)) throw new BuildFailedException("Missing dynamic card package: " + path);
            return paths;
        }
    }
}
