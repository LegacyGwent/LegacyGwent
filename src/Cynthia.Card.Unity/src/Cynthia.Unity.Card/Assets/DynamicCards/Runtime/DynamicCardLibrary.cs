using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Script.DynamicCards
{
    public sealed class DynamicCardLibrary : MonoBehaviour
    {
        public const string ContentRoot = "Assets/DynamicCards/Content/";
        public const string CatalogAsset = ContentRoot + "catalog.json";
        public const string BundleFile = "cards.bundle";
        public const string BundleIndexFile = "cards.index.json";
#if UNITY_EDITOR
        // Source prefabs can stall Unity's main thread for seconds. Keep that path
        // available for content authoring, but never enter it implicitly in the game UI.
        public static bool AllowEditorSourceLoading
        {
            get { return UnityEditor.EditorPrefs.GetBool("LegacyGwent.DynamicCards.SourceLoading." + Application.dataPath, false); }
            set { UnityEditor.EditorPrefs.SetBool("LegacyGwent.DynamicCards.SourceLoading." + Application.dataPath, value); }
        }
#endif
        private static DynamicCardLibrary instance;
        private readonly Dictionary<string, DynamicCardEntry> entries = new Dictionary<string, DynamicCardEntry>();
        private AssetBundle bundle;
        private string bundleRoot;
        private sealed class Part
        {
            public string File;
            public AssetBundle Bundle;
            public int Leases;
            public int ExpectedControllers;
            public bool Opening;
            public RuntimeAnimatorController[] Controllers;
            public AnimationClip[] Clips;
            public readonly Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
            public readonly Dictionary<string, AudioClip> Audio = new Dictionary<string, AudioClip>();
        }
        private readonly Dictionary<string, Part> parts = new Dictionary<string, Part>();
        private readonly Dictionary<string, Part> prefabParts = new Dictionary<string, Part>(StringComparer.OrdinalIgnoreCase);
        private bool loading, loaded, failed;
        private readonly List<PendingCard> queue = new List<PendingCard>();
        private bool creating;
        private float nextLoad, interactionUntil;
        private float averageFrame = 1f / 60f;
        internal static float ThumbnailDelay(float secondsPerFrame)
        { return Mathf.Lerp(.025f, .1f, Mathf.InverseLerp(.018f, .04f, secondsPerFrame)); }
        private bool collecting;
        private int released;
        private float lastRelease, lastCollection;
        public static void Released(DynamicCardEntry entry)
        {
            if (instance == null) return;
            Part part;
            if (entry != null && instance.prefabParts.TryGetValue(entry.prefab, out part))
            {
                if (part.Leases <= 0) Debug.LogError("Dynamic card package released without a matching request: " + part.File);
                else part.Leases--;
            }
            instance.released++; instance.lastRelease = Time.realtimeSinceStartup;
        }
        private sealed class PendingCard
        {
            public DynamicCardView View;
            public int Version;
            public float ReadyAt;
            public int ReadyFrame;
        }

        public void Enqueue(DynamicCardView view, int version)
        {
            Cancel(view);
            var item = new PendingCard { View = view, Version = version,
                ReadyAt = Time.realtimeSinceStartup + (view.IsPreview ? 0 : .08f),
                ReadyFrame = Time.frameCount + (view.IsPreview ? 0 : 1) };
            // Finish an in-flight grid read promptly so the selected card can take over.
            // CreateOne restores the application's previous priority when that read ends.
            if (view.IsPreview && creating) Application.backgroundLoadingPriority = ThreadPriority.Normal;
            // A visible detail card can begin now; only the grid waits for layout/debounce.
            if (view.IsPreview && !creating && !collecting && view.IsCurrent(version) && view.IsVisible())
                StartCoroutine(CreateOne(item));
            else queue.Add(item);
        }

        public static void Cancel(DynamicCardView view)
        {
            if (instance != null) instance.queue.RemoveAll(item => item.View == view);
        }

        private void Update()
        {
            averageFrame = Mathf.Lerp(averageFrame, Mathf.Min(Time.unscaledDeltaTime, .1f), .2f);
            if (Input.anyKey || Input.mouseScrollDelta.sqrMagnitude > 0) interactionUntil=Time.realtimeSinceStartup+.2f;
            if (creating || collecting) return;
            if (released > 0 && (released >= 12 || Time.realtimeSinceStartup - lastRelease > 2f) && Time.realtimeSinceStartup - lastCollection > 5f)
            { StartCoroutine(CollectUnused()); return; }
            if (!DynamicCardSettings.Enabled) return;
            PendingCard selected = null;
            for (int i = queue.Count - 1; i >= 0; i--)
            {
                var item = queue[i];
                if (item.View == null || !item.View.IsCurrent(item.Version)) { queue.RemoveAt(i); continue; }
                if (!item.View.IsPreview && (Time.realtimeSinceStartup < nextLoad || Time.realtimeSinceStartup<interactionUntil)) continue;
                if (Time.frameCount < item.ReadyFrame || Time.realtimeSinceStartup < item.ReadyAt || !item.View.IsVisible()) continue;
                if (selected == null || ComesFirst(item.View, selected.View)) selected = item;
            }
            if (selected == null) return;
            queue.Remove(selected);
            StartCoroutine(CreateOne(selected));
        }

        private static bool ComesFirst(DynamicCardView left, DynamicCardView right)
        {
            if (left.IsPreview != right.IsPreview) return left.IsPreview;
            var a = left.DisplayPosition; var b = right.DisplayPosition;
            // Layout may have changed since binding: use the current visual row and column.
            int rowA = Mathf.RoundToInt(a.y / 8f), rowB = Mathf.RoundToInt(b.y / 8f);
            return rowA != rowB ? rowA > rowB : a.x < b.x;
        }

        private IEnumerator CollectUnused()
        {
            collecting = true;
            try
            {
                yield return null; yield return null;
                released = 0;
                // The preview owns a lease just like a grid card. Its presence must not
                // retain unrelated packages accumulated while browsing the collection.
                foreach (var part in parts.Values)
                    if (part.Leases == 0 && part.Bundle != null)
                    { part.Prefabs.Clear(); part.Audio.Clear(); part.Controllers = null; part.Clips = null; part.Bundle.Unload(true); part.Bundle = null; }
                yield return Resources.UnloadUnusedAssets();
            }
            finally { lastCollection = Time.realtimeSinceStartup; collecting = false; }
        }

        private IEnumerator CreateOne(PendingCard item)
        {
            creating = true;
            var previousPriority=Application.backgroundLoadingPriority;
            Application.backgroundLoadingPriority=item.View.IsPreview ? ThreadPriority.Normal : ThreadPriority.Low;
            try { yield return item.View.Create(item.Version); }
            finally { Application.backgroundLoadingPriority=previousPriority; creating = false; nextLoad = Time.realtimeSinceStartup + ThumbnailDelay(averageFrame); }
        }
        public static DynamicCardLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("Dynamic card library");
                    instance = go.AddComponent<DynamicCardLibrary>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public IEnumerator Load(string artId, bool withAudio, Action<DynamicCardEntry, GameObject, AudioClip> complete)
        {
            // The library owns this coroutine: recycling one card must not cancel everybody's load.
            if (!loaded && !failed && !loading) StartCoroutine(LoadCatalog());
            while (loading) yield return null;
            DynamicCardEntry entry;
            if (!loaded || string.IsNullOrEmpty(artId) || !entries.TryGetValue(artId, out entry))
            { complete(null, null, null); yield break; }
            GameObject prefab=null;AudioClip audio=null;
            Part part = null;
            bool transferred = false;
            if (bundle != null) prefabParts.TryGetValue(entry.prefab, out part);
            if (part != null) part.Leases++;
            try
            {
                if(bundle!=null)
                {
                    if (part != null) yield return OpenPart(part);
                    var owner = part == null ? bundle : part.Bundle;
                    if (owner != null)
                    {
                        if (part != null) part.Prefabs.TryGetValue(entry.prefab, out prefab);
                        if (prefab == null)
                        {
                            var request=owner.LoadAssetAsync<GameObject>(entry.prefab);yield return request;prefab=request.asset as GameObject;
                            if (part != null && prefab != null) part.Prefabs[entry.prefab] = prefab;
                        }
                        if(withAudio && !string.IsNullOrEmpty(entry.audio))
                        {
                            if (part != null) part.Audio.TryGetValue(entry.audio, out audio);
                            if (audio == null)
                            {
                                var sound=owner.LoadAssetAsync<AudioClip>(entry.audio);yield return sound;audio=sound.asset as AudioClip;
                                if (part != null && audio != null) part.Audio[entry.audio] = audio;
                            }
                        }
                    }
                }
#if UNITY_EDITOR
                else
                {
                    prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(entry.prefab);yield return null;
                    if(withAudio && !string.IsNullOrEmpty(entry.audio))audio=UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(entry.audio);
                }
#endif
                complete(entry, prefab, audio);
                transferred = prefab != null;
            }
            finally { if (part != null && !transferred) part.Leases = Math.Max(0, part.Leases - 1); }
        }

        private void Awake() { DynamicCardSettings.Changed += SettingsChanged; }
        private void SettingsChanged() { if (!DynamicCardSettings.Enabled) { queue.Clear(); StartCoroutine(ReleaseDisabledContent()); } }
        private IEnumerator ReleaseDisabledContent()
        {
            yield return null;
            yield return null;
            while (loading || creating || collecting) yield return null;
            if (DynamicCardSettings.Enabled) yield break;
            foreach (var part in parts.Values) if (part.Bundle != null) part.Bundle.Unload(true);
            parts.Clear(); prefabParts.Clear();
            if (bundle != null) { bundle.Unload(true); bundle = null; }
            entries.Clear(); loaded = false; failed = false;
        }

        private IEnumerator LoadCatalog()
        {
            loading = true;
            string json = null;
            string indexJson = null;
#if UNITY_EDITOR
            const string editorBundle="Library/DynamicCardsBundles/StandaloneWindows64/cards.bundle";
            if(File.Exists(editorBundle) && File.Exists(editorBundle+".editor-ready"))
            {
                bundleRoot = Path.GetDirectoryName(editorBundle);
                yield return OpenBundle(BundleFile, result => bundle = result);
            }
            if (bundle == null)
            {
                if (AllowEditorSourceLoading)
                {
                    Debug.LogWarning("Dynamic cards: source asset loading was explicitly enabled for content development. Synchronous reads may stall the editor. Rebuild packages through Tools > Dynamic Cards > Build Options for asynchronous page loading.");
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(CatalogAsset);
                    if (asset != null) json = asset.text;
                }
                else
                    Debug.LogWarning("Dynamic card package cache is missing or outdated; static art remains active to avoid synchronous editor stalls. Rebuild through Tools > Dynamic Cards > Build Options > 仅构建动态卡资源包, then restart Play Mode.");
                yield return null;
            }
#else
            bundleRoot = Path.Combine(Application.streamingAssetsPath, "DynamicCards");
            yield return OpenBundle(BundleFile, result => bundle = result);
#endif
            if (bundle != null)
            {
                var request = bundle.LoadAssetAsync<TextAsset>(CatalogAsset);
                yield return request;
                var asset = request.asset as TextAsset;
                if (asset != null) json = asset.text;
                var path = Path.Combine(bundleRoot, BundleIndexFile);
                if (path.Contains("://"))
                {
                    using (var indexRequest = UnityWebRequest.Get(path))
                    {
                        yield return indexRequest.SendWebRequest();
                        if (!indexRequest.isNetworkError && !indexRequest.isHttpError) indexJson = indexRequest.downloadHandler.text;
                    }
                }
                else if (File.Exists(path)) indexJson = File.ReadAllText(path);
            }
            try
            {
                if (!string.IsNullOrEmpty(indexJson))
                {
                    var index = JsonUtility.FromJson<DynamicCardBundleIndex>(indexJson);
                    if (index == null || index.version != DynamicCardBundleIndex.CurrentVersion || index.parts == null)
                        throw new InvalidDataException("Dynamic card packages need rebuilding with animation dependencies (index v2).");
                    foreach (var item in index.parts)
                    {
                        if (Path.GetFileName(item.file) != item.file || !item.file.StartsWith("cards-") || !item.file.EndsWith(".bundle"))
                            throw new InvalidDataException("Invalid dynamic card part filename.");
                        var part = new Part { File = item.file, ExpectedControllers = item.animationControllers }; parts.Add(item.file, part);
                        foreach (var prefab in item.prefabs) prefabParts.Add(prefab, part);
                    }
                }
                var catalog = json == null ? null : JsonUtility.FromJson<DynamicCardCatalog>(json);
                if (catalog != null && catalog.version == 1 && catalog.cards != null)
                    foreach (var card in catalog.cards)
                        if (card.artIds != null)
                            foreach (var artId in card.artIds)
                                if (!entries.ContainsKey(artId)) entries.Add(artId, card);
                loaded = catalog != null;
            }
            catch (Exception exception) { Debug.LogWarning("Dynamic card catalog: " + exception.Message); }
            failed = !loaded;
            loading = false;
            if (failed) Debug.LogWarning("Dynamic card content is unavailable; static card art remains active.");
        }

        private IEnumerator OpenPart(Part part)
        {
            while (part.Opening) yield return null;
            if (part.Bundle != null) yield break;
            part.Opening = true;
            try
            {
                yield return OpenBundle(part.File, result => part.Bundle = result);
                if (part.Bundle == null) yield break;
                // Unity 2019 can lose the state graphs of deferred controller dependencies
                // after an asset refresh. Load the explicitly indexed animation roots before
                // the first prefab, and retain them for the same lifetime as their package.
                // Models, textures and audio still load only when a card requests them.
                var request = part.Bundle.LoadAllAssetsAsync<RuntimeAnimatorController>();
                yield return request;
                var controllers = new List<RuntimeAnimatorController>();
                var clips = new HashSet<AnimationClip>();
                foreach (var asset in request.allAssets)
                {
                    var controller = asset as RuntimeAnimatorController;
                    if (controller == null) continue;
                    controllers.Add(controller);
                    foreach (var clip in controller.animationClips) if (clip != null) clips.Add(clip);
                }
                if (controllers.Count != part.ExpectedControllers)
                {
                    Debug.LogError("Dynamic card package animation dependencies are incomplete: " + part.File + ". Rebuild the packages.");
                    part.Bundle.Unload(true); part.Bundle = null;
                    yield break;
                }
                part.Controllers = controllers.ToArray();
                part.Clips = new AnimationClip[clips.Count]; clips.CopyTo(part.Clips);
            }
            finally { part.Opening = false; }
        }

        private IEnumerator OpenBundle(string file, Action<AssetBundle> complete)
        {
            var path = Path.Combine(bundleRoot, file);
            if (!path.Contains("://"))
            {
                if (!File.Exists(path)) { complete(null); yield break; }
                var request = AssetBundle.LoadFromFileAsync(path); yield return request; complete(request.assetBundle);
            }
            else
            {
                using (var request = UnityWebRequestAssetBundle.GetAssetBundle(path))
                {
                    yield return request.SendWebRequest();
                    complete(request.isNetworkError || request.isHttpError ? null : DownloadHandlerAssetBundle.GetContent(request));
                }
            }
        }

        private void OnDestroy()
        {
            DynamicCardSettings.Changed -= SettingsChanged;
            foreach (var part in parts.Values) if (part.Bundle != null) part.Bundle.Unload(false);
            if (bundle != null) bundle.Unload(false);
            if (instance == this) instance = null;
        }
    }
}
