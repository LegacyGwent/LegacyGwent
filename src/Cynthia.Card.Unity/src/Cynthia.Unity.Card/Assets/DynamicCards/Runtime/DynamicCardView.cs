using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // A child of the existing art image: existing borders, badges and card backs retain their draw order.
    public sealed class DynamicCardView : MonoBehaviour
    {
        private const float HorizontalLimit=.7f;
        private const float VerticalLimit=1f/3f;
        public bool IsDragging => dragging;
        private Image art;
        private RawImage surface;
        private Material surfaceMaterial;
        private string artId;
        private bool hidden, preview, dragging, miniature;
        private bool premiumAllowed;
        private bool preparing;
        private CardShowInfo collectionCard;
        internal bool IsPresentationReady { get { return !preparing; } }
        internal bool KeepStaticDuringUpgrade { get; private set; }
        private bool previewAudio = true;
        private DynamicCardPresentation presentation;
        private int generation;
        private GameObject model;
        private Camera renderCamera;
        private RenderTexture texture;
        private DynamicCardQualityProfile qualityProfile;
        private DynamicCardPostProcessRenderer postProcess;
        // Explicit diagnostics only; no per-frame allocations or timers in normal play.
        internal long RenderCount { get; private set; }
        internal RenderTexture RenderTarget { get { return texture; } }
        private AudioSource sound;
        private DynamicCardEffects effects;
        private DynamicCardSourceControllers sourceControllers;
        private Transform pivot;
        private RectTransform frame;
        private RectTransform portraitFrame;
        private DynamicCardDragHandle dragHandle;
        private Quaternion frameRest, pivotRest;
        private Vector2 dragOrigin, dragStart, target, current;
        private float releasedAt = -10;
        private DynamicCardEntry entry;
        private float age;
        private Scene stage;
        private static readonly HashSet<DynamicCardView> ActiveViews = new HashSet<DynamicCardView>();
        internal static bool HasVisiblePreview
        {
            get
            {
                foreach (var view in ActiveViews)
                    if (view != null && view.preview && !view.hidden && view.IsVisible()) return true;
                return false;
            }
        }
        private static int stageId;
        private static int nextCell;
        private static readonly Stack<int> FreeCells = new Stack<int>();
        private int cell;
        private float invisibleSince = -1;
        private readonly Vector3[] corners = new Vector3[4];
        internal Vector2 DisplayPosition
        {
            get
            {
                var canvas = art.canvas;
                var camera = DisplayCamera(canvas);
                return ScreenBounds(art.rectTransform, camera).center;
            }
        }
        internal bool IsPreview { get { return preview; } }
        internal bool IsCurrent(int version)
        { return this != null && version == generation && isActiveAndEnabled && !hidden && premiumAllowed && DynamicCardSettings.Enabled; }

        internal bool IsVisible()
        {
            if (art == null || !art.isActiveAndEnabled || art.color.a <= .001f || art.canvas == null || !art.canvas.isActiveAndEnabled) return false;
            if (presentation != null && presentation.Waiting)
            { if (!presentation.AncestorsVisible(art.transform)) return false; }
            else if (collectionCard != null && collectionCard.WaitingForCollectionArt)
            { if (!collectionCard.CollectionAncestorsVisible(art.transform)) return false; }
            else if (art.canvasRenderer.cull || art.canvasRenderer.GetInheritedAlpha() <= .001f) return false;
            var canvas = art.canvas;
            var camera = DisplayCamera(canvas);
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && camera == null) return false;
            Rect bounds = ScreenBounds(art.rectTransform, camera);
            Rect screen = camera != null ? camera.pixelRect : new Rect(0, 0, Screen.width, Screen.height);
            if (!bounds.Overlaps(screen)) return false;
            for (var parent = art.transform.parent; parent != null; parent = parent.parent)
            {
                var clip = parent.GetComponent<RectMask2D>();
                var mask = parent.GetComponent<Mask>();
                if ((clip != null && clip.isActiveAndEnabled) || (mask != null && mask.isActiveAndEnabled))
                    if (!bounds.Overlaps(ScreenBounds((RectTransform)parent, camera))) return false;
            }
            return true;
        }

        private static Camera DisplayCamera(Canvas canvas)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) return null;
            // Battle cards have world-space canvases without an assigned event camera.
            // Match their rendering camera instead of treating world units as screen pixels.
            return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        private Rect ScreenBounds(RectTransform rect, Camera camera)
        {
            rect.GetWorldCorners(corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(camera, corners[0]), max = min;
            for (int i = 1; i < 4; i++)
            {
                var point = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
                min = Vector2.Min(min, point); max = Vector2.Max(max, point);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private bool CanCreate(int version)
        {
            if (!IsCurrent(version)) return false;
            if (IsVisible()) return true;
            // A scroll can hide a card while its asset request is in flight.
            DynamicCardLibrary.Instance.Enqueue(this, version);
            return false;
        }

        public static RectTransform FindCardRoot(Transform artTransform, Transform borderTransform)
        {
            for (var parent = artTransform.parent; parent != null; parent = parent.parent)
                if (borderTransform == parent || borderTransform.IsChildOf(parent)) return parent as RectTransform;
            return null;
        }

        public static void Bind(Image image, string id, bool concealed = false, bool largePreview = false, RectTransform wholeCard = null, bool listThumbnail = false, bool playPreviewAudio = true, RectTransform presentationRoot = null, RectTransform portraitBorder = null, bool premium = false)
        {
            if (image == null) return;
            var view = image.GetComponent<DynamicCardView>();
            if (view == null) view = image.gameObject.AddComponent<DynamicCardView>();
            view.art = image;
            view.collectionCard = image.GetComponentInParent<CardShowInfo>();
            bool changed = view.artId != id || view.hidden != concealed || view.preview != largePreview || view.miniature != listThumbnail || view.previewAudio != playPreviewAudio || view.premiumAllowed != premium;
            if (changed)
                view.KeepStaticDuringUpgrade = view.artId == id && !view.premiumAllowed && premium &&
                    image.GetComponent<PremiumCardAppearance>() != null && image.GetComponent<PremiumCardAppearance>().IsLocked;
            view.premiumAllowed = premium;
            view.artId = id; view.hidden = concealed; view.preview = largePreview; view.miniature = listThumbnail;
            view.previewAudio = playPreviewAudio;
            view.portraitFrame = portraitBorder;
            var root = presentationRoot != null ? presentationRoot : wholeCard;
            if (largePreview && root != null && (view.presentation == null || view.presentation.transform != root))
            {
                if (view.presentation != null) view.presentation.Restore();
                view.presentation = root.GetComponent<DynamicCardPresentation>();
                if (view.presentation == null) view.presentation = root.gameObject.AddComponent<DynamicCardPresentation>();
                changed = true;
            }
            if (wholeCard != null && view.frame != wholeCard)
            {
                view.frame = wholeCard; view.frameRest = wholeCard.localRotation;
                var handle = wholeCard.GetComponent<DynamicCardDragHandle>();
                if (handle == null) handle = wholeCard.gameObject.AddComponent<DynamicCardDragHandle>();
                handle.View = view;view.dragHandle=handle;handle.enabled=view.preview && view.model!=null;
            }
            if (changed) view.Refresh();
        }

        private void OnEnable() { ActiveViews.Add(this); DynamicCardSettings.Changed += ApplyQualitySetting; if (art != null) Refresh(); }
        private void OnDisable() { ActiveViews.Remove(this); DynamicCardSettings.Changed -= ApplyQualitySetting; Clear(); }
        private void OnDestroy() { Clear(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ReturnToCenter(); }

        private void ApplyQualitySetting()
        {
            // Off still releases everything. Live quality changes keep the model, lease,
            // animation clock, audio and interaction state; pending loads use the latest tier.
            if (!DynamicCardSettings.Enabled || (model == null && !preparing)) Refresh();
            else qualityProfile = DynamicCardQualityProfile.Get(DynamicCardSettings.Quality);
        }

        private bool ApplyRenderTarget()
        {
            int size = qualityProfile.Resolution(preview);
            if (size == 0) return false;
            if (texture != null && texture.width == size && texture.IsCreated()) return true;
            var replacement = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
            replacement.name = "Dynamic card " + (preview ? "preview " : "thumbnail ") + size;
            if (!replacement.Create()) { Destroy(replacement); return false; }
            var previous = texture;
            // Keep a valid picture even if a tier change happens between UI renders.
            if (previous != null && previous.IsCreated()) Graphics.Blit(previous, replacement);
            texture = replacement;
            renderCamera.targetTexture = replacement;
            if (surface != null) surface.texture = replacement;
            if (previous != null) { previous.Release(); Destroy(previous); }
            return true;
        }

        private void RenderCard()
        {
            UnityEngine.Profiling.Profiler.BeginSample("DynamicCards.Render");
            long started = DynamicCardRenderMetrics.Begin();
            try
            {
                renderCamera.Render();
                RenderCount++;
            }
            finally
            {
                DynamicCardRenderMetrics.End(started, texture.width, texture.height);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private void Refresh()
        {
            Clear();
            if (!isActiveAndEnabled || hidden || !premiumAllowed || !DynamicCardSettings.Enabled || string.IsNullOrEmpty(artId)) return;
            preparing = true;
            if (collectionCard != null) collectionCard.UpdateCollectionVisibility();
            if (preview && presentation != null && !KeepStaticDuringUpgrade) presentation.Begin();
            DynamicCardLibrary.Instance.Enqueue(this, generation);
        }

        internal IEnumerator Create(int version)
        {
            if (!CanCreate(version)) yield break;
            GameObject prefab = null; AudioClip clip = null; DynamicCardEntry data = null;
            yield return DynamicCardLibrary.Instance.Load(artId, preview && previewAudio, (d, p, a) => { data = d; prefab = p; clip = a; });
            if (!preview) yield return null;
            if (prefab == null)
            {
                if (IsCurrent(version))
                {
                    preparing = false;
                    if (presentation != null) presentation.Reveal(false);
                }
                yield break;
            }
            if (!CanCreate(version)) { DynamicCardLibrary.Released(data); yield break; }
            entry = data;
            // An isolated scene plus a unique distant cell prevents card lights/cameras touching gameplay.
            cell = FreeCells.Count > 0 ? FreeCells.Pop() : ++nextCell;
            stage = SceneManager.CreateScene("DynamicCard_" + ++stageId);
            // Keep the isolation offset outside all source Animator bindings. A source root curve
            // can reset its local position without moving this card (or its world-space particles) into gameplay.
            var stagingRoot = new GameObject("Card placement");
            SceneManager.MoveGameObjectToScene(stagingRoot, stage);
            stagingRoot.transform.position = new Vector3(10000 + (cell % 32) * 1024, 10000 + (cell / 32) * 1024, 0);
            var appearanceRoot = new GameObject("Source appearance anchor");
            appearanceRoot.transform.SetParent(stagingRoot.transform, false);
            appearanceRoot.transform.localPosition = DynamicCardFraming.AppearanceOffset;
            model = Instantiate(prefab, appearanceRoot.transform, false);
            // Source scenes retain unassigned authoring surfaces (including emitter
            // meshes). Keep their transforms, animation and emission, but do not draw
            // Unity's pink missing-material fallback for these explicit source paths.
            if (entry.nonRenderingPaths != null && entry.nonRenderingPaths.Length > 0)
            {
                var paths = new HashSet<string>(entry.nonRenderingPaths);
                foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                    if (paths.Contains(DynamicCardPaths.RelativePath(renderer.transform, model.transform)) &&
                        !System.Array.Exists(renderer.sharedMaterials, material => material != null))
                        renderer.forceRenderingOff = true;
            }
            RestoreSceneFacing();
            model.SetActive(true);
            // Premium meshes retain their source skinning even when the game's global quality uses one bone.
            foreach (var skin in model.GetComponentsInChildren<SkinnedMeshRenderer>(true)) skin.quality = SkinQuality.Bone4;
            foreach (Transform child in model.transform)
                if (child.localPosition.sqrMagnitude > 1000000) child.localPosition = Vector3.zero;
            // Spread grid setup across frames; the selected detail card presents immediately.
            if (!preview) yield return null;
            if (!IsCurrent(version)) yield break;
            var cameraObject = new GameObject("Card camera");
            SceneManager.MoveGameObjectToScene(cameraObject, stage);
            cameraObject.transform.position = stagingRoot.transform.position + new Vector3(0, 0, DynamicCardFraming.CameraDistance(entry, miniature))
                + (miniature ? DynamicCardFraming.AppearanceOffset : Vector3.zero);
            renderCamera = cameraObject.AddComponent<Camera>();
            renderCamera.enabled = false;
            var originalPostEffects = model.GetComponentsInChildren<DynamicCardPostEffect>(true);
            if (originalPostEffects.Length != 0)
            {
                postProcess = cameraObject.AddComponent<DynamicCardPostProcessRenderer>();
                postProcess.Effects = originalPostEffects;
            }
            renderCamera.fieldOfView = entry.fieldOfView;
            renderCamera.nearClipPlane = entry.nearClip;
            renderCamera.farClipPlane = entry.farClip;
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.clear;
            renderCamera.allowHDR = false;
            renderCamera.allowMSAA = false;
            // Source cameras render a square; the card art is a portrait region inside that square.
            qualityProfile = DynamicCardQualityProfile.Get(DynamicCardSettings.Quality);
            if (!ApplyRenderTarget()) { Clear(); yield break; }
            if (postProcess != null) postProcess.Downsample = qualityProfile.PostProcessDownsample;
            DynamicCardFraming.Apply(renderCamera, entry, miniature);
            pivot = string.IsNullOrEmpty(entry.pivot) ? null : model.transform.Find(entry.pivot);
            if (pivot != null) pivotRest = pivot.localRotation;
            var overlay = new GameObject("Dynamic art", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            overlay.transform.SetParent(art.transform, false);
            surface = overlay.GetComponent<RawImage>();
            surfaceMaterial = new Material(Resources.Load<Shader>("DynamicCardSurface"));
            surfaceMaterial.hideFlags = HideFlags.HideAndDontSave;
            surface.material = surfaceMaterial;
            surface.enabled = false;
            surface.raycastTarget = false;
            if (presentation != null) presentation.RegisterGraphic(surface);
            // Legacy art sprites contain padding on the right/bottom. Match their content region,
            // not the entire padded texture, so the original portrait mask and frame stay aligned.
            surface.rectTransform.anchorMin = new Vector2(0, 1 - 713f / 1024);
            surface.rectTransform.anchorMax = new Vector2(497f / 1024, 1);
            surface.rectTransform.offsetMin = Vector2.zero; surface.rectTransform.offsetMax = Vector2.zero;
            surface.texture = texture;
            surface.uvRect = DynamicCardFraming.ArtRegion(entry);
            AlignPreviewPortrait();
            if (miniature)
            {
                surface.rectTransform.anchorMin = Vector2.zero; surface.rectTransform.anchorMax = Vector2.one;
                UpdateThumbnailFraming();
            }
            foreach (var animator in model.GetComponentsInChildren<Animator>(true)) { animator.applyRootMotion=false; animator.Rebind(); animator.Update(0); }
            NormalizeStageRoot();
            foreach (var particles in model.GetComponentsInChildren<ParticleSystem>(true))
            { particles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear); if (particles.main.playOnAwake) particles.Play(false); }
            effects = model.AddComponent<DynamicCardEffects>(); effects.Initialize(entry); effects.Tick(0);
            sourceControllers=model.AddComponent<DynamicCardSourceControllers>();sourceControllers.Initialize(entry,renderCamera.transform);
            sourceControllers.Tick(0,0,(entry.xStart+entry.xEnd)*.5f,(entry.yStart+entry.yEnd)*.5f);
            if(dragHandle!=null)dragHandle.enabled=preview;
            if (preview && clip != null)
            { sound = model.AddComponent<AudioSource>(); sound.clip = clip; sound.loop = true; sound.spatialBlend = 0; sound.Play(); }
            ApplyCut();
            NormalizeStageRoot();
            // Animator.Rebind/Update has sampled startup opacity, but LateUpdate has not
            // run yet. Apply it before exposing the first frame of multi-part card rigs.
            foreach (var property in model.GetComponentsInChildren<DynamicCardAnimatedMaterialProperty>())
                if (property.isActiveAndEnabled) property.ApplyNow();
            RenderCard();
            surface.enabled = true;
            preparing = false;
            if (presentation != null)
            {
                if (KeepStaticDuringUpgrade) presentation.RevealInPlace();
                else presentation.Reveal(true);
            }
            KeepStaticDuringUpgrade = false;
        }

        private void AlignPreviewPortrait()
        {
            if (!preview || miniature || surface == null || portraitFrame == null) return;
            // The collection card's 100-wide frame has a 2-unit top inset. The detail
            // prefab uses different padding; align the visible art without changing its camera.
            surface.rectTransform.GetWorldCorners(corners);
            float top = portraitFrame.InverseTransformPoint(corners[1]).y;
            float desired = portraitFrame.rect.yMax - portraitFrame.rect.width * .02f;
            surface.rectTransform.position += portraitFrame.TransformVector(new Vector3(0, desired - top, 0));
        }

        private void UpdateThumbnailFraming()
        {
            if (miniature && surface != null)
                surface.uvRect = DynamicCardFraming.ThumbnailRegion(surface.rectTransform.rect.size);
        }

        private void OnRectTransformDimensionsChange()
        {
            // Layout groups and window resizing can resize a row after its card has loaded.
            UpdateThumbnailFraming();
        }

        private float nextRender;
        private void LateUpdate()
        {
            if (model == null || renderCamera == null || surface == null || !surface.enabled) return;
            bool visible = IsVisible();
            bool running = visible;
            if (model.activeSelf != running) model.SetActive(running);
            if (!visible)
            {
                if (invisibleSince < 0) invisibleSince = Time.realtimeSinceStartup;
                if (Time.realtimeSinceStartup - invisibleSince > 1.5f) Refresh();
                return;
            }
            invisibleSince = -1;
            if (!running) return;
            NormalizeStageRoot();
            age += Time.unscaledDeltaTime;
            if (preview && !dragging)
            {
                // Return to neutral first; then resume the old client's gentle carousel motion.
                float idleTime = Mathf.Min(age - 1, Time.unscaledTime - releasedAt - 2);
                float blend = Mathf.SmoothStep(0, 1, Mathf.Clamp01(idleTime));
                target = idleTime > 0 ? new Vector2(Mathf.Sin(idleTime * .47f) * HorizontalLimit, Mathf.Sin(idleTime * .73f) * VerticalLimit) * blend : Vector2.zero;
            }
            current = Vector2.Lerp(current, target, 1 - Mathf.Exp(-(dragging ? 18 : 12) * Time.unscaledDeltaTime));
            float pitch = Mathf.Lerp(entry.xStart, entry.xEnd, (current.y + 1) * .5f);
            float yaw = Mathf.Lerp(entry.yStart, entry.yEnd, (current.x + 1) * .5f);
            if (pivot != null)
            {
                // CardPerspectiveHandler: normalized Y = outer pitch / 1.5, X = outer yaw / 7.
                pivot.localRotation = pivotRest * Quaternion.Euler(pitch, yaw, 0);
            }
            if (sound != null) sound.volume = PlayerPrefs.GetInt("isCloseSound", 1) == 0 ? 0 : PlayerPrefs.GetInt("effectVolum", 7) / 10f;
            ApplyCut();
            if (effects != null) effects.Tick(age);
            if (presentation != null) presentation.Tick(current);
            else if (frame != null) frame.localRotation = frameRest * Quaternion.Euler(current.y * DynamicCardPresentation.MaxPitch, current.x * DynamicCardPresentation.MaxYaw, 0);
            AlignPreviewPortrait();
            if(sourceControllers!=null)sourceControllers.Tick(age,Time.unscaledDeltaTime,pitch,yaw);
            surface.color = art.color;
            // Thumbnail textures need fewer redraws; each card has its own phase to spread camera work.
            int fps = qualityProfile.FramesPerSecond(preview);
            if(fps == 0 || Time.unscaledTime>=nextRender)
            {
                if (!ApplyRenderTarget()) return;
                if (postProcess != null) postProcess.Downsample = qualityProfile.PostProcessDownsample;
                RenderCard();
                if (fps > 0)
                {
                    float step=1f/fps;float phase=(GetInstanceID()&31)/32f*step;
                    nextRender=(Mathf.Floor((Time.unscaledTime-phase)/step)+1)*step+phase;
                }
            }
        }

        private Transform beforeCutGroup,afterCutGroup;
        private Renderer[] beforeCutMeshes,afterCutMeshes;
        private bool cutKnown,cutApplied;
        private void ApplyCut()
        {
            if(entry.cutTime<0)return;
            bool after=age>=entry.cutTime;
            if(cutKnown && cutApplied==after)return;
            if(!cutKnown)
            {
                beforeCutGroup=DynamicCardPaths.Find(model.transform,entry.beforeCut);afterCutGroup=DynamicCardPaths.Find(model.transform,entry.afterCut);
                beforeCutMeshes=beforeCutGroup==null?null:beforeCutGroup.GetComponentsInChildren<Renderer>(true);
                afterCutMeshes=afterCutGroup==null?null:afterCutGroup.GetComponentsInChildren<Renderer>(true);
            }
            // The source rig-switch event controls mesh visibility, as in the Godot adaptation.
            // Particle and trail renderers retain their independent animation/event state.
            if(beforeCutGroup!=null)beforeCutGroup.gameObject.SetActive(!after);
            if(afterCutGroup!=null)afterCutGroup.gameObject.SetActive(after);
            var meshes=after?afterCutMeshes:beforeCutMeshes;
            if(meshes!=null)foreach(var renderer in meshes)
                if(renderer is SkinnedMeshRenderer || renderer is MeshRenderer)renderer.enabled=true;
            cutKnown=true;cutApplied=after;
        }

        private void NormalizeStageRoot()
        {
            // Some source clips key their parked scene root at y=-10000. Keep that staging
            // offset out of the portable camera, including after Animator has evaluated.
            foreach(Transform child in model.transform)
                if(child.localPosition.sqrMagnitude>1000000)child.localPosition=Vector3.zero;
        }

        private void RestoreSceneFacing()
        {
            // Older exported scenes can be parked facing the card back. An explicit
            // source startup transform takes precedence (some cards intentionally turn).
            if (!entry.prefab.Contains("/Legacy2017/")) return;
            string sourceId = string.IsNullOrEmpty(entry.sourceId) ? entry.id : entry.sourceId;
            var root = model.transform.Find(sourceId);
            if (root == null) return;
            foreach (var setup in entry.initialTransforms ?? new DynamicCardInitialTransform[0])
                if (setup.path == sourceId) return;
            if (Quaternion.Angle(root.localRotation, Quaternion.Euler(0, 180, 0)) < .01f)
                root.localRotation = Quaternion.identity;
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (!preview || model == null || data.button != PointerEventData.InputButton.Left || dragging) return;
            if (presentation != null) presentation.InterruptEntrance();
            dragging = true; dragOrigin = data.position; dragStart = current;
        }
        public void OnDrag(PointerEventData data)
        {
            if (!dragging) return;
            var delta = data.position - dragOrigin;
            // Screen-relative travel keeps the same feel at different window sizes.
            float referenceScale = Mathf.Max(1, Screen.height) / 900f;
            target = dragStart + new Vector2(-delta.x / (150 * referenceScale), delta.y / (120 * referenceScale));
            target = new Vector2(Mathf.Clamp(target.x, -HorizontalLimit, HorizontalLimit), Mathf.Clamp(target.y, -VerticalLimit, VerticalLimit));
        }
        public void OnEndDrag(PointerEventData data) { ReturnToCenter(); }
        internal void ReturnToCenter() { if (dragging) releasedAt = Time.unscaledTime; dragging = false; target = Vector2.zero; }

        private void Clear()
        {
            preparing = false;
            if (presentation != null) presentation.Restore();
            if (entry != null) DynamicCardLibrary.Released(entry);
            invisibleSince = -1;
            cutKnown=false;beforeCutGroup=null;afterCutGroup=null;beforeCutMeshes=null;afterCutMeshes=null;
            generation++;
            if(dragHandle!=null)dragHandle.enabled=false;
            DynamicCardLibrary.Cancel(this);
            if (surface != null) { surface.enabled = false; Destroy(surface.gameObject); }
            if (surfaceMaterial != null) { Destroy(surfaceMaterial); surfaceMaterial = null; }
            if (renderCamera != null) { renderCamera.targetTexture = null; Destroy(renderCamera.gameObject); }
            if (model != null) { model.SetActive(false); Destroy(model); }
            if (texture != null) { texture.Release(); Destroy(texture); }
            if (stage.IsValid() && stage.isLoaded) SceneManager.UnloadSceneAsync(stage);
            stage = default(Scene);
            if (cell > 0) { FreeCells.Push(cell); cell = 0; }
            if (frame != null) frame.localRotation = frameRest;
            model = null; renderCamera = null; texture = null; surface = null; sound = null; pivot = null;
            effects = null; sourceControllers = null; entry = null;
            postProcess = null; qualityProfile = null; nextRender = 0;
            target = current = Vector2.zero; dragging = false; age = 0; releasedAt = -10;
        }
    }
}
