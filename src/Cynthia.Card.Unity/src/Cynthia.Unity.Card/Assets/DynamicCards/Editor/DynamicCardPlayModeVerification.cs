using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Autofac;
using Cynthia.Card;
using Cynthia.Card.Client;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards.Editor
{
    // File requests are explicit commands, never automatic login on Editor startup.
    // Place ui-request.json in <repo>/work/PremiumEditorReadiness20261002.
    // {"action":"capture","run":"first-play","cards":["14003","12011"]}
    [InitializeOnLoad]
    public static class DynamicCardPlayModeVerification
    {
        [Serializable] public class Request { public string action; public string run; public string[] cards; }
        [Serializable] private class Fixture { public string username; public string password; }
        [Serializable] private class State
        {
            public bool playing, canAnimate, explicitSourceOnly, qualityStored, legacyStored, isLocalTest, runtimeVisible, runtimeSurfaceEnabled;
            public int storedQuality, legacyEnabled;
            public string quality, scene, editorStatus, endpoint, runtimeModel, runtimeArt, runtimePrefab;
            public string contentMode, effectiveContentMode, contentSource, contentReason;
            public string[] ownedPremiumCards;
        }
        [Serializable] private class Sample
        {
            public int frame, modelInstance, changedPixels;
            public float realtime, modelAge;
            public bool visible, surfaceEnabled, modelActive;
            public string hash, artFrame, gameFrame;
            public string[] animators;
        }
        [Serializable] private class CardResult
        {
            public string card, art, status;
            public List<Sample> samples = new List<Sample>();
        }
        [Serializable] private class Result
        {
            public string action, status, reason, startedUtc, finishedUtc;
            public bool passed;
            public int newErrors;
            public State state;
            public List<CardResult> cards = new List<CardResult>();
        }
        private static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
        private static Request request;
        private static Result result;
        private static EditorInfo editor;
        private static int cardIndex;
        private static double deadline, nextSample, nextPoll;
        private static bool captureQueued;
        private static string output;
        private static Color32[] priorPixels;
        private static int modelInstance;
        private static string Work
        {
            get
            {
                var directory = new DirectoryInfo(Application.dataPath);
                while (directory != null && !File.Exists(Path.Combine(directory.FullName, "skills/legacy-gwent-maintainer/SKILL.md")))
                    directory = directory.Parent;
                if (directory == null) throw new InvalidOperationException("Repository root unavailable");
                return Path.Combine(directory.FullName, "work/PremiumEditorReadiness20261002");
            }
        }
        static DynamicCardPlayModeVerification() { EditorApplication.update += Poll; }
        private static object Field(object value, string name)
        {
            if (value == null) return null;
            var field = value.GetType().GetField(name, Private);
            return field == null ? null : field.GetValue(value);
        }
        private static GwentClientService Client()
        {
            return DependencyResolver.Container == null ? null : DependencyResolver.Container.Resolve<GwentClientService>();
        }
        private static EditorInfo FindEditor()
        {
            var main = UnityEngine.Object.FindObjectOfType<MainCode>();
            if (main != null && main.EditorMenu != null) return main.EditorMenu;
            // Scene-owned inactive components only; never pick prefab assets from Resources.
            return Resources.FindObjectsOfTypeAll<EditorInfo>().FirstOrDefault(info =>
                info != null && info.gameObject.scene.IsValid() && info.gameObject.scene.isLoaded && info.gameObject.scene.name == "Game");
        }
        private static Uri RuntimeEndpoint()
        {
            var client = EditorApplication.isPlaying ? Client() : null;
            var factory = Field(client == null ? null : client.HubConnection, "_connectionFactory");
            var options = Field(factory, "_httpConnectionOptions");
            var property = options == null ? null : options.GetType().GetProperty("Url");
            return property == null ? null : property.GetValue(options, null) as Uri;
        }
        private static bool Local(Uri uri)
        {
            return uri != null && uri.IsLoopback && uri.Port == 5005 && uri.Scheme == "http" &&
                string.IsNullOrEmpty(uri.UserInfo) && string.IsNullOrEmpty(uri.Query);
        }
        private static State Inspect()
        {
            var info = FindEditor();
            var view = info == null || info.ShowArtCard == null ? null : info.ShowArtCard.CardImg.GetComponent<DynamicCardView>();
            var model = Field(view, "model") as GameObject;
            var surface = Field(view, "surface") as RawImage;
            var entry = Field(view, "entry") as DynamicCardEntry;
            var endpoint = RuntimeEndpoint();
            var client = EditorApplication.isPlaying ? Client() : null;
            var content = DynamicCardEditorContentPolicy.Evaluate(Directory.GetParent(Application.dataPath).FullName,
                DynamicCardEditorContentPolicy.Mode, Application.isBatchMode);
            return new State
            {
                playing = EditorApplication.isPlaying, scene = SceneManager.GetActiveScene().name,
                quality = DynamicCardSettings.Quality.ToString(), canAnimate = ClientContent.CanAnimate,
                explicitSourceOnly = DynamicCardLibrary.AllowEditorSourceLoading,
                contentMode = content.Mode.ToString(), effectiveContentMode = content.EffectiveMode.ToString(),
                contentSource = content.Source, contentReason = content.Reason,
                qualityStored = PlayerPrefs.HasKey("DynamicCards.Quality"), storedQuality = PlayerPrefs.GetInt("DynamicCards.Quality", -1),
                legacyStored = PlayerPrefs.HasKey("DynamicCards.Enabled"), legacyEnabled = PlayerPrefs.GetInt("DynamicCards.Enabled", 0),
                editorStatus = info == null ? "unavailable" : info.EditorStatus.ToString(),
                endpoint = endpoint == null ? "unavailable" : endpoint.GetLeftPart(UriPartial.Authority),
                isLocalTest = client != null && client.User != null && client.User.UserName == "localtest",
                ownedPremiumCards = client != null && client.User != null && client.User.UserName == "localtest"
                    ? GwentMap.CardMap.Keys.Where(PremiumCollectionClient.Owns).ToArray() : new string[0],
                runtimeModel = model == null ? "unavailable" : model.name,
                runtimeArt = info == null || info.ShowArtCard == null || info.ShowArtCard.CurrentCore == null ? "unavailable" : info.ShowArtCard.CurrentCore.CardArtsId,
                runtimePrefab = entry == null ? "unavailable" : entry.prefab,
                runtimeVisible = Visible(view), runtimeSurfaceEnabled = surface != null && surface.enabled
            };
        }
        private static void Log(string message, string trace, LogType type)
        {
            // Raw log text and stack traces may carry account data: record only new-error count.
            if (result != null && (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)) result.newErrors++;
        }
        private static void Save() { File.WriteAllText(Path.Combine(output, "results.json"), JsonUtility.ToJson(result, true)); }
        private static void Finish(string status, string reason)
        {
            result.status = status; result.reason = reason;
            result.passed = status == "passed" && result.newErrors == 0;
            if (status == "passed" && result.newErrors != 0) { result.status = "failed"; result.reason = "New errors during this capture"; }
            result.finishedUtc = DateTime.UtcNow.ToString("O");
            Application.logMessageReceived -= Log;
            Save(); result = null; request = null; editor = null; captureQueued = false;
        }
        private static void Poll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            try
            {
                if (result != null) { Tick(); return; }
                if (EditorApplication.timeSinceStartup < nextPoll) return;
                nextPoll = EditorApplication.timeSinceStartup + 1;
                var path = Path.Combine(Work, "ui-request.json");
                if (!File.Exists(path)) return;
                request = JsonUtility.FromJson<Request>(File.ReadAllText(path));
                File.Delete(path); // Consume once before any action, including Play-mode domain reload.
                if (request == null) throw new InvalidOperationException("Invalid request");
                var run = string.IsNullOrEmpty(request.run) ? DateTime.UtcNow.ToString("yyyyMMddTHHmmssfff") : request.run;
                if (run == "." || run == ".." || run.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || run.Contains("/") || run.Contains("\\"))
                    throw new InvalidOperationException("run must be a filename component");
                output = Path.Combine(Work, run); Directory.CreateDirectory(output);
                result = new Result { action = request.action, status = "running", startedUtc = DateTime.UtcNow.ToString("O"), state = Inspect() };
                if (request.action == "inspect") { Finish("inspected", "Read-only snapshot; no visual acceptance claim"); return; }
                if (request.action == "prepare-local")
                {
                    if (EditorApplication.isPlayingOrWillChangePlaymode) { Finish("blocked", "prepare-local requires EditMode"); return; }
                    Environment.SetEnvironmentVariable("GWENT_SERVER_URL", "http://127.0.0.1:5005", EnvironmentVariableTarget.Process);
                    Finish("prepared", "Process endpoint set; LoginScene Play requested");
                    LoginPlayToolbar.PlayLogin(); return;
                }
                if (!EditorApplication.isPlaying || !Local(RuntimeEndpoint())) { Finish("blocked", "Requires PlayMode and actual loopback 5005 Hub endpoint"); return; }
                if (request.action == "login-local")
                {
                    if (SceneManager.GetActiveScene().name != "LoginScene") { Finish("blocked", "login-local requires LoginScene"); return; }
                    Uri environment;
                    if (!Uri.TryCreate(Environment.GetEnvironmentVariable("GWENT_SERVER_URL"), UriKind.Absolute, out environment) || !Local(environment))
                    { Finish("blocked", "Process endpoint is not loopback 5005"); return; }
                    var fixture = JsonUtility.FromJson<Fixture>(File.ReadAllText(Path.Combine(Work, "local-fixture.json")));
                    var login = UnityEngine.Object.FindObjectOfType<LoginClick>();
                    if (fixture == null || fixture.username != "localtest" || string.IsNullOrEmpty(fixture.password) || login == null)
                    { Finish("blocked", "Valid localtest fixture and LoginClick required"); return; }
                    login.Username.SetTextWithoutNotify(fixture.username); login.Password.SetTextWithoutNotify(fixture.password);
                    Application.logMessageReceived += Log; login.Login(); deadline = EditorApplication.timeSinceStartup + 60; Save(); return;
                }
                if (request.action != "capture") { Finish("blocked", "Unknown action"); return; }
                if (SceneManager.GetActiveScene().name != "Game" || !result.state.isLocalTest) { Finish("blocked", "capture requires Game authenticated as localtest"); return; }
                if (!DynamicCardSettings.Enabled) { Finish("blocked", "QualityOff: enable quality through normal UI before capture"); return; }
                editor = FindEditor();
                if (editor == null) { Finish("blocked", "Production EditorInfo unavailable"); return; }
                if (!editor.isActiveAndEnabled) { Finish("blocked", "Open the production collection through the main menu before capture"); return; }
                request.cards = request.cards == null || request.cards.Length == 0 ? new[] { "14003", "12011" } : request.cards;
                if (request.cards.Any(id => !GwentMap.CardMap.ContainsKey(id))) { Finish("blocked", "Unknown requested card ID"); return; }
                if (request.cards.Any(id => !PremiumCollectionClient.Owns(id)))
                { Finish("blocked", "Requested premium card is not owned; choose IDs from state.ownedPremiumCards"); return; }
                Application.logMessageReceived += Log; editor.OpenEditor(); cardIndex = 0; BeginCard();
            }
            catch (Exception exception)
            {
                // Exception types provide diagnostics without risking fixture contents in messages.
                if (result != null) Finish("failed", "Verification exception: " + exception.GetType().Name);
                else Debug.LogWarning("[DynamicCardVerification] Request rejected: " + exception.GetType().Name);
            }
        }
        private static void BeginCard()
        {
            var core = new CardStatus(request.cards[cardIndex]) { IsPremium = true };
            result.cards.Add(new CardResult { card = core.CardId, art = core.CardArtsId, status = "loading" });
            priorPixels = null; modelInstance = 0;
            editor.SelectSwitchUICard(core);
            deadline = EditorApplication.timeSinceStartup + 60; nextSample = 0; Save();
        }
        private static bool Visible(DynamicCardView view)
        {
            return view != null && (bool)typeof(DynamicCardView).GetMethod("IsVisible", Private).Invoke(view, null);
        }
        private static void Tick()
        {
            if (!EditorApplication.isPlaying) { Finish("blocked", "PlayMode ended during verification"); return; }
            if (request.action == "login-local")
            {
                var client = Client();
                if (SceneManager.GetActiveScene().name == "Game" && client != null && client.User != null && client.User.UserName == "localtest")
                { result.state = Inspect(); Finish(result.newErrors == 0 ? "authenticated" : "failed", "Login observation complete"); return; }
                if (EditorApplication.timeSinceStartup >= deadline) Finish("blocked", "Local login timed out");
                return;
            }
            var row = result.cards[cardIndex];
            var view = editor == null || editor.ShowArtCard == null ? null : editor.ShowArtCard.CardImg.GetComponent<DynamicCardView>();
            var model = Field(view, "model") as GameObject;
            var surface = Field(view, "surface") as RawImage;
            var entry = Field(view, "entry") as DynamicCardEntry;
            bool visible = Visible(view);
            bool ready = visible && model != null && model.activeInHierarchy && surface != null && surface.enabled && surface.gameObject.activeInHierarchy &&
                surface.texture is RenderTexture && entry != null && entry.artIds != null && entry.artIds.Contains(row.art);
            if (EditorApplication.timeSinceStartup >= deadline)
            { row.status = visible ? "ready-timeout" : "blocked-hidden-game-view"; Finish(visible ? "failed" : "blocked", row.status); return; }
            if (!ready) { if (!visible) row.status = "waiting-for-visible-game-view"; return; }
            if (captureQueued) return;
            if (EditorApplication.timeSinceStartup < nextSample) return;
            if (modelInstance != 0 && modelInstance != model.GetInstanceID()) { row.status = "model-replaced"; Finish("blocked", "Preview changed during sampling"); return; }
            modelInstance = model.GetInstanceID();
            captureQueued = true;
            editor.StartCoroutine(CaptureAtEndOfFrame(result, view, model, surface, row));
        }
        private static IEnumerator CaptureAtEndOfFrame(Result expectedResult, DynamicCardView view, GameObject model, RawImage surface, CardResult row)
        {
            // The Editor update can run while a card RenderTexture is still bound. Reading
            // Screen.width/height there can exceed that target and throw ReadPixels errors.
            // Use the existing production MonoBehaviour to sample after all UI rendering.
            yield return new WaitForEndOfFrame();
            if (result != expectedResult) yield break;
            captureQueued = false;
            if (!EditorApplication.isPlaying || model == null || surface == null || !Visible(view) || !surface.enabled || !model.activeInHierarchy)
                yield break;
            Sample sample;
            try { sample = Capture(view, model, surface, row); }
            catch (Exception exception) { Finish("failed", "Capture exception: " + exception.GetType().Name); yield break; }
            row.samples.Add(sample); row.status = "sampling"; nextSample = EditorApplication.timeSinceStartup + .75; Save();
            if (row.samples.Count < 3) yield break;
            row.status = row.samples.Skip(1).Any(frame => frame.changedPixels > 0) ? "passed" : "surface-not-changing";
            if (row.status != "passed") { Finish("failed", row.status); yield break; }
            cardIndex++;
            if (cardIndex == request.cards.Length) Finish("passed", "All requested production previews visibly rendered changing pixels; inspect saved PNGs");
            else BeginCard();
        }
        private static Sample Capture(DynamicCardView view, GameObject model, RawImage surface, CardResult row)
        {
            var sample = new Sample { frame = Time.frameCount, realtime = Time.realtimeSinceStartup, modelInstance = model.GetInstanceID(),
                visible = Visible(view), surfaceEnabled = surface.enabled, modelActive = model.activeInHierarchy, modelAge = (float)Field(view, "age") };
            var texture = (RenderTexture)surface.texture;
            var previous = RenderTexture.active;
            var image = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            try
            {
                RenderTexture.active = texture; image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0); image.Apply();
                var pixels = image.GetPixels32();
                if (priorPixels != null && pixels.Length == priorPixels.Length)
                    for (int i = 0; i < pixels.Length; i++)
                        if (Math.Abs(pixels[i].r - priorPixels[i].r) > 2 || Math.Abs(pixels[i].g - priorPixels[i].g) > 2 ||
                            Math.Abs(pixels[i].b - priorPixels[i].b) > 2 || Math.Abs(pixels[i].a - priorPixels[i].a) > 2) sample.changedPixels++;
                using (var sha = SHA256.Create()) sample.hash = Convert.ToBase64String(sha.ComputeHash(image.GetRawTextureData()));
                sample.artFrame = row.card + "-art-" + row.samples.Count + ".png";
                File.WriteAllBytes(Path.Combine(output, sample.artFrame), image.EncodeToPNG()); priorPixels = pixels;
            }
            finally { RenderTexture.active = previous; UnityEngine.Object.DestroyImmediate(image); }
            sample.animators = model.GetComponentsInChildren<Animator>(true).Select(a => a.name + ":" +
                (a.runtimeAnimatorController == null || a.layerCount == 0 ? "no-state" : a.GetCurrentAnimatorStateInfo(0).normalizedTime.ToString("R", System.Globalization.CultureInfo.InvariantCulture))).ToArray();
            sample.gameFrame = row.card + "-game-" + row.samples.Count + ".png";
            // Read the actual Game display; do not render replacement cameras or canvases.
            var gameImage = ScreenCapture.CaptureScreenshotAsTexture();
            if (gameImage == null) throw new InvalidOperationException("Game screenshot unavailable");
            try { File.WriteAllBytes(Path.Combine(output, sample.gameFrame), gameImage.EncodeToPNG()); }
            finally { UnityEngine.Object.DestroyImmediate(gameImage); }
            return sample;
        }
    }
}
