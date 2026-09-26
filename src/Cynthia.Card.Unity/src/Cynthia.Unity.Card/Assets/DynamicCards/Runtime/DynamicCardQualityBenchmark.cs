#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;

namespace Assets.Script.DynamicCards
{
    // Explicit benchmark scene only. Never auto-installs in the game or a release player.
    // Reuses production Bind/load/render paths with fixed assets, resolution and tier order.
    public sealed class DynamicCardQualityBenchmark : MonoBehaviour
    {
        public string[] ArtIds;
        public Sprite[] Artworks;
        public string OutputDirectory;
        public float SampleSeconds = 12;
        public bool QuitWhenComplete;
        private readonly List<DynamicCardView> views = new List<DynamicCardView>();
        private readonly List<Sample> samples = new List<Sample>();
        private readonly List<string> checks = new List<string>();
        private readonly List<string> errors = new List<string>();
        private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private Text title;
        private Sprite fallbackArt;
        private bool oldQualityExists, oldLegacyExists, oldBackground, saved, finished;
        private int oldQuality, oldLegacy, oldVsync, oldFrameRate;
        private int width, height, globalQuality;
        private string output;

        [Serializable] private sealed class Sample
        {
            public string quality;
            public int round, views, frames, over50ms, over100ms, cardResolution, previewResolution;
            public double meanFrameMs, p95FrameMs, p99FrameMs, maxFrameMs, fps;
            public double renderCallsPerSecond, renderSubmissionMsPerFrame, renderPixelsPerSecond;
            public long renderTargetNativeBytes, renderTargetColorBytes, renderTargetDepth24EstimateBytes;
            public long unityAllocatedBytes, monoUsedBytes;
            public double meanGpuMs;
            public int gpuTimingSamples;
            public long[] perViewRenderCounts;
            public double[] perViewRenderHz;
        }

        private static T Field<T>(object target, string name) { return (T)target.GetType().GetField(name, Private).GetValue(target); }
        private void Check(bool ok, string message)
        {
            if (!ok) throw new InvalidOperationException(message);
            checks.Add(message); Progress(message);
        }
        private void Progress(string message) { File.WriteAllText(Path.Combine(output, "progress.txt"), DateTime.UtcNow.ToString("O") + " " + message); }
        private void Log(string message, string trace, LogType type)
        { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message); }

        private IEnumerator Start()
        {
            output = string.IsNullOrEmpty(OutputDirectory) ? Path.Combine(Application.persistentDataPath, "DynamicCardQualityBenchmark") : OutputDirectory;
            Directory.CreateDirectory(output);
            oldQualityExists = PlayerPrefs.HasKey(DynamicCardSettings.QualityPreference);
            oldLegacyExists = PlayerPrefs.HasKey(DynamicCardSettings.LegacyPreference);
            oldQuality = PlayerPrefs.GetInt(DynamicCardSettings.QualityPreference);
            oldLegacy = PlayerPrefs.GetInt(DynamicCardSettings.LegacyPreference);
            oldFrameRate = Application.targetFrameRate; oldVsync = QualitySettings.vSyncCount;
            oldBackground = Application.runInBackground;
            width = Screen.width; height = Screen.height; globalQuality = QualitySettings.GetQualityLevel();
            saved = true;
            Application.logMessageReceived += Log;
            Application.runInBackground = true; Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0;
            var stack = new Stack<IEnumerator>(); stack.Push(Run());
            Exception failure = null;
            while (stack.Count > 0)
            {
                object current = null; bool moved = false;
                try { moved = stack.Peek().MoveNext(); if (moved) current = stack.Peek().Current; }
                catch (Exception exception) { failure = exception; break; }
                if (!moved) { stack.Pop(); continue; }
                var nested = current as IEnumerator;
                if (nested != null) { stack.Push(nested); continue; }
                yield return current;
            }
            if (failure != null) errors.Add(failure.ToString());
            Finish();
        }

        private void SettingsContract()
        {
            PlayerPrefs.DeleteKey(DynamicCardSettings.QualityPreference);
            PlayerPrefs.SetInt(DynamicCardSettings.LegacyPreference, 1);
            Check(DynamicCardSettings.Quality == DynamicCardQuality.High, "Legacy On migrates to High");
            PlayerPrefs.SetInt(DynamicCardSettings.LegacyPreference, 0);
            Check(DynamicCardSettings.Quality == DynamicCardQuality.Off, "Legacy Off stays Off");
            PlayerPrefs.SetInt(DynamicCardSettings.QualityPreference, 999);
            Check(DynamicCardSettings.Quality == DynamicCardQuality.Off, "Invalid stored quality safely falls back");
            DynamicCardSettings.Quality = DynamicCardQuality.Medium;
            DynamicCardSettings.Enabled = true;
            Check(DynamicCardSettings.Quality == DynamicCardQuality.Medium, "Legacy enable does not overwrite Medium");
            int notifications = 0; Action changed = () => notifications++;
            DynamicCardSettings.Changed += changed;
            try
            {
                DynamicCardSettings.Quality = DynamicCardQuality.Medium;
                DynamicCardSettings.Quality = DynamicCardQuality.Low;
                Check(notifications == 1 && PlayerPrefs.GetInt(DynamicCardSettings.QualityPreference) == 1,
                    "Quality persists and notifies only on a real change");
            }
            finally { DynamicCardSettings.Changed -= changed; }
        }

        private IEnumerator Run()
        {
            SettingsContract();
            SettingsUiContract();
            Check(ArtIds != null && ArtIds.Length == 25, "Fixed workload: 24 small cards plus one preview");
            DynamicCardSettings.Quality = DynamicCardQuality.High;
            var camera = new GameObject("Benchmark clear camera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.025f, .035f, .05f);
            camera.cullingMask = 0;
            var canvas = new GameObject("Benchmark canvas", typeof(Canvas), typeof(CanvasScaler)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            title = Label(canvas.transform, "Quality benchmark", new Vector2(40, -25), new Vector2(1800, 50), 28);
            fallbackArt = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, Texture2D.blackTexture.width, Texture2D.blackTexture.height), Vector2.one * .5f);
            for (int i = 0; i < ArtIds.Length; i++)
            {
                bool preview = i == 24;
                var art = new GameObject("Card " + ArtIds[i], typeof(RectTransform), typeof(Image)).GetComponent<Image>();
                art.transform.SetParent(canvas.transform, false);
                var rect = art.rectTransform; rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = preview ? new Vector2(1430, -205) : new Vector2(40 + i % 6 * 226, -100 - i / 6 * 237);
                rect.sizeDelta = preview ? new Vector2(920, 920) : new Vector2(300, 300);
                art.sprite = Artworks != null && i < Artworks.Length ? Artworks[i] : null;
                if (art.sprite == null) art.sprite = fallbackArt;
                art.raycastTarget = false;
                Label(canvas.transform, ArtIds[i], preview ? new Vector2(1430, -170) : rect.anchoredPosition + new Vector2(0, -209), new Vector2(210, 25), 17);
                DynamicCardView.Bind(art, ArtIds[i], largePreview: preview, playPreviewAudio: false, premium: true);
                views.Add(art.GetComponent<DynamicCardView>());
            }
            yield return WaitForViews();
            Check(Field<AssetBundle>(DynamicCardLibrary.Instance, "bundle") != null, "Production asynchronous AssetBundle path is active");
            // Warm all tiers before measuring; reverse the order in round two to expose ordering drift.
            foreach (var quality in new[] { DynamicCardQuality.High, DynamicCardQuality.Medium, DynamicCardQuality.Low })
            { DynamicCardSettings.Quality = quality; yield return new WaitForSecondsRealtime(2); }
            var order = new[] { DynamicCardQuality.High, DynamicCardQuality.Medium, DynamicCardQuality.Low,
                DynamicCardQuality.Low, DynamicCardQuality.Medium, DynamicCardQuality.High };
            for (int i = 0; i < order.Length; i++)
            {
                var models = views.Select(v => Field<GameObject>(v, "model").GetInstanceID()).ToArray();
                var ages = views.Select(v => Field<float>(v, "age")).ToArray();
                DynamicCardSettings.Quality = order[i];
                for (int j = 0; j < views.Count; j++)
                    Check(Field<GameObject>(views[j], "model").GetInstanceID() == models[j] && Field<float>(views[j], "age") >= ages[j],
                        "Tier " + i + " view " + j + " preserves model and animation clock");
                yield return new WaitForSecondsRealtime(2);
                yield return Measure(order[i], i < 3 ? 1 : 2);
            }
            Check(Screen.width == width && Screen.height == height && QualitySettings.GetQualityLevel() == globalQuality,
                "All tiers preserve display resolution and global quality");
            // Compare the same animation instant through the real card camera at each tier.
            yield return CaptureQualityFrames();
            DynamicCardSettings.Quality = DynamicCardQuality.Off;
            yield return null; yield return null;
            Check(views.All(v => Field<GameObject>(v, "model") == null && v.RenderTarget == null), "Off releases all models and render targets");
            yield return new WaitForSecondsRealtime(1);
            Check(Field<AssetBundle>(DynamicCardLibrary.Instance, "bundle") == null, "Off releases the catalog and content bundles");
            Check(Resources.FindObjectsOfTypeAll<RenderTexture>().All(t => !t.name.StartsWith("Dynamic card ")), "Off leaves no persistent card render textures");
            DynamicCardSettings.Quality = DynamicCardQuality.Low; yield return null;
            DynamicCardSettings.Quality = DynamicCardQuality.Off; yield return null; yield return null;
            Check(views.All(v => v.RenderTarget == null), "Rapid enable/off cancels pending creation");
            foreach (var quality in new[] { DynamicCardQuality.Low, DynamicCardQuality.Medium, DynamicCardQuality.High })
            {
                DynamicCardSettings.Quality = quality; yield return WaitForViews();
                Check(views.Count(v => v.RenderTarget != null) == 25, "Off to " + quality + " restores every visible card without a count cap");
                DynamicCardSettings.Quality = DynamicCardQuality.Off; yield return new WaitForSecondsRealtime(.3f);
            }
            Check(errors.Count == 0, "No runtime errors during tier transitions");
        }

        private void SettingsUiContract()
        {
            var previousContainer = DependencyResolver.Container;
            var builder = new ContainerBuilder(); builder.RegisterType<LocalizationService>().SingleInstance();
            using (var container = builder.Build())
            {
                DependencyResolver.Container = container;
                var root = new GameObject("Quality selector contract", typeof(RectTransform));
                try
                {
                    var row = new GameObject("Original quality row", typeof(RectTransform)); row.transform.SetParent(root.transform, false);
                    Label(row.transform, "Original quality", Vector2.zero, new Vector2(400, 40), 24);
                    var control = new GameObject("Selector", typeof(RectTransform)).AddComponent<ChoseValue>();
                    control.transform.SetParent(row.transform, false);
                    control.ShowText = Label(control.transform, "Value", Vector2.zero, new Vector2(250, 40), 24);
                    control.ChoseList = new List<string> { "Settings_Off", "Settings_On" };
                    int leaked = 0; control.onValueChanged.AddListener(i => leaked++);
                    DynamicCardSettingRow.Install(control.gameObject);
                    var installed = root.transform.Find("DynamicCardsOption").GetComponentInChildren<ChoseValue>();
                    var locale = container.Resolve<LocalizationService>().TextLocalization;
                    for (int language = 0; language < 4; language++)
                    {
                        locale.ChooseLanguage(language);
                        for (int tier = 0; tier < 4; tier++)
                        {
                            installed.Index = tier;
                            Check((int)DynamicCardSettings.Quality == tier && installed.ShowText.text == locale.GetText(installed.ChoseList[tier]) &&
                                installed.ShowText.text != installed.ChoseList[tier], "Language " + language + " quality " + tier + " selects and translates correctly");
                        }
                        DynamicCardSettings.Quality = DynamicCardQuality.Medium;
                        Check(installed.Index == 2, "Language " + language + " external tier changes refresh selector");
                    }
                    Check(leaked == 0 && QualitySettings.GetQualityLevel() == globalQuality, "Animated card selector does not invoke global quality listener");
                    DynamicCardSettings.Quality = DynamicCardQuality.Low;
                    root.SetActive(false); root.SetActive(true);
                    Check(installed.Index == 1 && DynamicCardSettings.Quality == DynamicCardQuality.Low, "Reopening settings preserves selected Low tier");
                }
                finally { root.SetActive(false); Destroy(root); DependencyResolver.Container = previousContainer; }
            }
        }

        private IEnumerator WaitForViews()
        {
            float deadline = Time.realtimeSinceStartup + 180;
            while (views.Any(v => v.RenderTarget == null || !Field<RawImage>(v, "surface").enabled))
            {
                Progress("Loading cards " + views.Count(v => v.RenderTarget != null) + "/" + views.Count);
                if (Time.realtimeSinceStartup > deadline) throw new TimeoutException("Dynamic card loading did not complete");
                yield return null;
            }
        }

        private IEnumerator Measure(DynamicCardQuality quality, int round)
        {
            title.text = "Animated cards: " + quality + " | 24 thumbnails + 1 preview | round " + round;
            var profile = DynamicCardQualityProfile.Get(quality);
            Check(views.All(v => v.RenderTarget.width == profile.Resolution(v.IsPreview)), quality + " uses its configured render dimensions");
            var startCounts = views.Select(v => v.RenderCount).ToArray();
            var frames = new List<double>(8000);
            var sample = new Sample { quality = quality.ToString(), round = round, views = views.Count,
                cardResolution = profile.CardResolution, previewResolution = profile.PreviewResolution };
            foreach (var view in views)
            {
                sample.renderTargetNativeBytes += Profiler.GetRuntimeMemorySizeLong(view.RenderTarget);
                sample.renderTargetColorBytes += (long)view.RenderTarget.width * view.RenderTarget.height * 4;
                sample.renderTargetDepth24EstimateBytes += (long)view.RenderTarget.width * view.RenderTarget.height * 4;
            }
            var timing = new FrameTiming[1]; double gpuTotal = 0; ulong previousTiming = 0;
            DynamicCardRenderMetrics.Reset(); DynamicCardRenderMetrics.Enabled = true;
            var watch = System.Diagnostics.Stopwatch.StartNew(); double previous = watch.Elapsed.TotalMilliseconds;
            Progress("Measuring " + quality + " round " + round);
            while (watch.Elapsed.TotalSeconds < SampleSeconds)
            {
                yield return null;
                double now = watch.Elapsed.TotalMilliseconds; frames.Add(now - previous); previous = now;
                FrameTimingManager.CaptureFrameTimings();
                if (FrameTimingManager.GetLatestTimings(1, timing) > 0 && timing[0].cpuTimePresentCalled != previousTiming && timing[0].gpuFrameTime > 0)
                { previousTiming = timing[0].cpuTimePresentCalled; gpuTotal += timing[0].gpuFrameTime; sample.gpuTimingSamples++; }
            }
            DynamicCardRenderMetrics.Enabled = false;
            double seconds = watch.Elapsed.TotalSeconds;
            sample.frames = frames.Count; sample.meanFrameMs = frames.Average(); sample.fps = 1000 / sample.meanFrameMs;
            frames.Sort(); sample.p95FrameMs = frames[(int)(frames.Count * .95)]; sample.p99FrameMs = frames[(int)(frames.Count * .99)]; sample.maxFrameMs = frames.Last();
            sample.over50ms = frames.Count(x => x > 50); sample.over100ms = frames.Count(x => x > 100);
            sample.renderCallsPerSecond = DynamicCardRenderMetrics.Renders / seconds;
            sample.renderSubmissionMsPerFrame = DynamicCardRenderMetrics.SubmissionMilliseconds / frames.Count;
            sample.renderPixelsPerSecond = DynamicCardRenderMetrics.Pixels / seconds;
            sample.unityAllocatedBytes = Profiler.GetTotalAllocatedMemoryLong(); sample.monoUsedBytes = Profiler.GetMonoUsedSizeLong();
            sample.meanGpuMs = sample.gpuTimingSamples > 0 ? gpuTotal / sample.gpuTimingSamples : -1;
            sample.perViewRenderCounts = views.Select((v, i) => v.RenderCount - startCounts[i]).ToArray();
            sample.perViewRenderHz = sample.perViewRenderCounts.Select(c => c / seconds).ToArray();
            Check(sample.perViewRenderCounts.Select((c, i) => c >= seconds * Math.Min(sample.fps,
                profile.FramesPerSecond(i == 24) == 0 ? sample.fps : profile.FramesPerSecond(i == 24)) * .5).All(x => x),
                quality + " continuously renders every visible card");
            samples.Add(sample); WriteReport(false);
        }

        private IEnumerator CaptureQualityFrames()
        {
            // No yield between these renders: identical model/particle/animation state.
            var view = views.Last(); var camera = Field<Camera>(view, "renderCamera");
            var post = camera.GetComponent<DynamicCardPostProcessRenderer>();
            var previous = camera.targetTexture; int previousDownsample = post != null ? post.Downsample : 1;
            foreach (var quality in new[] { DynamicCardQuality.High, DynamicCardQuality.Medium, DynamicCardQuality.Low })
            {
                var profile = DynamicCardQualityProfile.Get(quality);
                var rt = new RenderTexture(profile.PreviewResolution, profile.PreviewResolution, 24, RenderTextureFormat.ARGB32);
                rt.Create(); camera.targetTexture = rt;
                if (post != null) post.Downsample = profile.PostProcessDownsample;
                camera.Render(); var previousActive = RenderTexture.active; RenderTexture.active = rt;
                var image = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
                image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0); image.Apply();
                File.WriteAllBytes(Path.Combine(output, "same-frame-" + quality + ".png"), image.EncodeToPNG());
                RenderTexture.active = previousActive; camera.targetTexture = previous;
                rt.Release(); Destroy(rt); Destroy(image);
            }
            if (post != null) post.Downsample = previousDownsample;
            yield return null;
        }

        private void WriteReport(bool complete)
        {
            File.WriteAllText(Path.Combine(output, "result.json"), Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                complete, passed = complete && errors.Count == 0, unity = Application.unityVersion,
                platform = Application.platform.ToString(), gpu = SystemInfo.graphicsDeviceName,
                graphicsApi = SystemInfo.graphicsDeviceType.ToString(), cpu = SystemInfo.processorType,
                screenWidth = width, screenHeight = height, sampleSeconds = SampleSeconds, artIds = ArtIds,
                gpuTimingNote = "-1 means unavailable; CPU camera submission is not GPU time; native RT bytes are Unity-reported, not total VRAM.",
                samples, checks, errors
            }, Newtonsoft.Json.Formatting.Indented));
        }
        private void Finish()
        {
            if (finished) return; finished = true;
            WriteReport(true); Progress(errors.Count == 0 ? "PASS" : "FAIL"); Restore();
            if (QuitWhenComplete && !Application.isEditor) Application.Quit(errors.Count == 0 ? 0 : 1);
        }
        private void Restore()
        {
            if (!saved) return; saved = false;
            DynamicCardRenderMetrics.Enabled = false;
            DynamicCardSettings.Quality = DynamicCardQuality.Off;
            if (oldQualityExists) PlayerPrefs.SetInt(DynamicCardSettings.QualityPreference, oldQuality); else PlayerPrefs.DeleteKey(DynamicCardSettings.QualityPreference);
            if (oldLegacyExists) PlayerPrefs.SetInt(DynamicCardSettings.LegacyPreference, oldLegacy); else PlayerPrefs.DeleteKey(DynamicCardSettings.LegacyPreference);
            PlayerPrefs.Save(); Application.targetFrameRate = oldFrameRate; QualitySettings.vSyncCount = oldVsync; Application.runInBackground = oldBackground;
            Application.logMessageReceived -= Log;
        }
        private void OnDestroy() { Restore(); if (fallbackArt != null) Destroy(fallbackArt); }
        private static Text Label(Transform parent, string caption, Vector2 position, Vector2 size, int fontSize)
        {
            var text = new GameObject(caption, typeof(RectTransform), typeof(Text)).GetComponent<Text>(); text.transform.SetParent(parent, false);
            text.rectTransform.anchorMin = text.rectTransform.anchorMax = text.rectTransform.pivot = new Vector2(0, 1);
            text.rectTransform.anchoredPosition = position; text.rectTransform.sizeDelta = size;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); text.fontSize = fontSize; text.color = Color.white;
            text.text = caption; text.raycastTarget = false; return text;
        }
    }
}
#endif
