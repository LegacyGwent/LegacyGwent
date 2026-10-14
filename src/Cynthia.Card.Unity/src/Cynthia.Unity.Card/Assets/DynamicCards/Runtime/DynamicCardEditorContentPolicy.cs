#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    public enum DynamicCardEditorContentMode { Automatic = 0, BundlesOnly = 1, SourceOnly = 2 }

    // Editor authoring policy does not grant package capability or change player quality.
    public static class DynamicCardEditorContentPolicy
    {
        [Serializable] private sealed class VersionProbe { public int version; }

        public sealed class Status
        {
            public DynamicCardEditorContentMode Mode { get; internal set; }
            public DynamicCardEditorContentMode EffectiveMode { get; internal set; }
            public string Source { get; internal set; }
            public string Reason { get; internal set; }
            public string Message { get; internal set; }
            public int CatalogEntries { get; internal set; }
            public int MissingSourcePrefabs { get; internal set; }
            public string BundleRoot { get; internal set; }
        }

        public static DynamicCardEditorContentMode Mode
        {
            get
            {
                int value = UnityEditor.EditorPrefs.GetInt("LegacyGwent.DynamicCards.ContentMode." + Application.dataPath, 0);
                return value >= 0 && value <= 2 ? (DynamicCardEditorContentMode)value : DynamicCardEditorContentMode.Automatic;
            }
            set
            {
                if (value < DynamicCardEditorContentMode.Automatic || value > DynamicCardEditorContentMode.SourceOnly)
                    throw new ArgumentOutOfRangeException("value");
                UnityEditor.EditorPrefs.SetInt("LegacyGwent.DynamicCards.ContentMode." + Application.dataPath, (int)value);
            }
        }

        public static Status Evaluate() { return Evaluate(null, ".", Mode, Application.isBatchMode); }
        public static Status Evaluate(string projectRoot, DynamicCardEditorContentMode mode, bool batchMode)
        { return Evaluate(null, projectRoot, mode, batchMode); }
        internal static Status EvaluateSource(string reason) { return Evaluate(reason, ".", Mode, Application.isBatchMode); }

        private static Status Evaluate(string bundleFailure, string projectRoot, DynamicCardEditorContentMode mode, bool batchMode)
        {
            if (string.IsNullOrEmpty(projectRoot)) throw new ArgumentException("A Unity project root is required.", "projectRoot");
            if (mode < DynamicCardEditorContentMode.Automatic || mode > DynamicCardEditorContentMode.SourceOnly) throw new ArgumentOutOfRangeException("mode");
            var effective = mode == DynamicCardEditorContentMode.Automatic && batchMode
                ? DynamicCardEditorContentMode.BundlesOnly : mode;
            var status = new Status { Mode = mode, EffectiveMode = effective, Source = "unavailable",
                BundleRoot = Path.Combine(projectRoot, "Library/DynamicCardsBundles/StandaloneWindows64") };
            DynamicCardCatalog catalog;
            try
            {
                if (!File.Exists(Path.Combine(projectRoot, DynamicCardLibrary.CatalogAsset))) return Finish(status, "source-catalog-missing");
                string catalogJson = File.ReadAllText(Path.Combine(projectRoot, DynamicCardLibrary.CatalogAsset));
                var version = JsonUtility.FromJson<VersionProbe>(catalogJson);
                if (version == null || version.version != 1) return Finish(status, "source-catalog-invalid");
                catalog = JsonUtility.FromJson<DynamicCardCatalog>(catalogJson);
                if (catalog == null || catalog.version != 1 || catalog.cards == null || catalog.cards.Length == 0)
                    return Finish(status, "source-catalog-invalid");
                var prefabs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var artIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (var card in catalog.cards)
                {
                    if (card == null || !SafeSourcePath(card.prefab) || !prefabs.Add(card.prefab))
                        return Finish(status, "source-catalog-invalid");
                    // Some delivered source scenes intentionally have no game-art mapping.
                    // They remain package members; only mapped entries are lookup targets.
                    if (card.artIds != null)
                        foreach (string id in card.artIds)
                            if (string.IsNullOrEmpty(id) || !artIds.Add(id)) return Finish(status, "source-catalog-invalid");
                    if (!File.Exists(Path.Combine(projectRoot, card.prefab))) status.MissingSourcePrefabs++;
                }
                status.CatalogEntries = catalog.cards.Length;
                if (artIds.Count == 0) return Finish(status, "source-catalog-invalid");
            }
            catch (Exception exception) { return Finish(status, "source-catalog-invalid", exception.Message); }

            string reason = bundleFailure;
            if (effective != DynamicCardEditorContentMode.SourceOnly && reason == null)
            {
                reason = ValidateBundles(status.BundleRoot, catalog);
                if (reason == null) { status.Source = "bundles"; return Finish(status, "bundles-ready"); }
            }
            if (effective == DynamicCardEditorContentMode.BundlesOnly) return Finish(status, reason ?? "bundles-unavailable");
            if (status.MissingSourcePrefabs != 0) return Finish(status, "source-prefabs-missing", reason);
            status.Source = "source";
            return Finish(status, effective == DynamicCardEditorContentMode.SourceOnly ? "source-requested" : reason ?? "source-fallback");
        }

        private static bool SafeSourcePath(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.StartsWith(DynamicCardLibrary.ContentRoot, StringComparison.Ordinal) ||
                path.Contains("\\") || path.Contains(":") || path.Contains("//")) return false;
            foreach (string segment in path.Split('/')) if (segment == "." || segment == "..") return false;
            return path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase);
        }

        private static string ValidateBundles(string root, DynamicCardCatalog catalog)
        {
            string bundle = Path.Combine(root, DynamicCardLibrary.BundleFile);
            if (!File.Exists(bundle) || new FileInfo(bundle).Length == 0) return "bundle-missing";
            if (!File.Exists(bundle + ".editor-ready")) return "ready-missing";
            string indexPath = Path.Combine(root, DynamicCardLibrary.BundleIndexFile);
            if (!File.Exists(indexPath)) return "index-missing";
            try
            {
                string indexJson = File.ReadAllText(indexPath);
                var version = JsonUtility.FromJson<VersionProbe>(indexJson);
                if (version == null || version.version != DynamicCardBundleIndex.CurrentVersion) return "index-invalid";
                var index = JsonUtility.FromJson<DynamicCardBundleIndex>(indexJson);
                if (index == null || index.version != DynamicCardBundleIndex.CurrentVersion || index.parts == null || index.parts.Length == 0)
                    return "index-invalid";
                var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var mapped = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var part in index.parts)
                {
                    if (part == null || string.IsNullOrEmpty(part.file) || part.file.Contains("/") || part.file.Contains("\\") || part.file.Contains(":") ||
                        !part.file.StartsWith("cards-", StringComparison.Ordinal) || !part.file.EndsWith(".bundle", StringComparison.Ordinal) ||
                        !files.Add(part.file) || part.prefabs == null || part.animationControllers < 0) return "index-invalid";
                    string path = Path.Combine(root, part.file);
                    if (!File.Exists(path) || new FileInfo(path).Length == 0) return "part-missing";
                    foreach (string prefab in part.prefabs)
                        if (!SafeSourcePath(prefab) || !mapped.Add(prefab)) return "index-invalid";
                }
                if (!mapped.SetEquals(Array.ConvertAll(catalog.cards, card => card.prefab))) return "index-catalog-mismatch";
                return null;
            }
            catch (Exception) { return "index-invalid"; }
        }

        private static Status Finish(Status status, string reason, string detail = null)
        {
            status.Reason = reason;
            status.Message = "Dynamic cards Editor: mode=" + status.Mode + " effective=" + status.EffectiveMode +
                " source=" + status.Source + " reason=" + reason + " cards=" + status.CatalogEntries +
                " missingPrefabs=" + status.MissingSourcePrefabs + (string.IsNullOrEmpty(detail) ? "" : " (" + detail + ")") + ". ";
            if (status.Source == "source") status.Message += "Source authoring preview uses synchronous reads and may stall on first load. BundlesOnly is required for package/performance acceptance.";
            else if (status.Source == "unavailable") status.Message += "Static art remains active. Restore sources with python scripts/premium-content.py restore at the repository root, or rebuild through Tools > Dynamic Cards > Build Options > 仅构建动态卡资源包; restart Play Mode.";
            return status;
        }
    }
}
#endif
