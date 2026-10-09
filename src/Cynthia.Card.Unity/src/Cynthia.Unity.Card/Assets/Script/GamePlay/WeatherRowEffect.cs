using Cynthia.Card;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Animated weather layer for one board row, drawn on top of the row's tint image.
// Weathers without an animated effect keep the plain tint color.
// Each effect is drawn in two layers: one behind the board cards (ground, water, stones, ...) and one in front of
// them (mist, flames, sparks, ...), so the cards stand inside the effect.
public class WeatherRowEffect : MonoBehaviour
{
    private const float FadeOutTime = 0.4f;
    // Board cards lie about 98.5 units from the camera (the board canvas at 100); a lifted or dragged card comes
    // 3 units closer. The front layer sits in between, so it covers the cards on the board but not a picked-up card.
    private const float FrontPlaneDistance = 97f;
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");
    private static readonly int AppliedAtId = Shader.PropertyToID("_AppliedAt");
    // The effect is drawn larger than the row, like the original whose layers spill over the row's edge.
    // Fraction of the row width / height added on each side; the shaders receive it as _Pad.
    public static readonly Vector2 Pad = new Vector2(0.06f, 0.3f);

    private static Canvas _frontCanvas;

    private Image _tint;
    private Image _effect;
    private Image _front;
    private Material _material;
    private Material _frontMaterial;
    private RowStatus _current = RowStatus.None;
    private float _progress;
    private Coroutine _transition;
    private readonly Vector3[] _corners = new Vector3[4];

    public RowStatus Current => _current;

    public static WeatherRowEffect Attach(Image tint)
    {
        var effect = tint.gameObject.AddComponent<WeatherRowEffect>();
        effect.Init(tint);
        return effect;
    }

    private void Init(Image tint)
    {
        _tint = tint;
        var go = new GameObject("WeatherEffect", typeof(RectTransform));
        go.layer = tint.gameObject.layer;
        var rect = (RectTransform)go.transform;
        rect.SetParent(tint.transform, false);
        var rowSize = tint.rectTransform.rect.size;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = -Vector2.Scale(Pad, rowSize);
        rect.offsetMax = Vector2.Scale(Pad, rowSize);
        _effect = go.AddComponent<Image>();
        _effect.raycastTarget = false;
        _effect.enabled = false;

        var frontGo = new GameObject("WeatherEffectFront", typeof(RectTransform));
        frontGo.layer = tint.gameObject.layer;
        frontGo.transform.SetParent(GetFrontCanvas(tint.canvas.rootCanvas).transform, false);
        _front = frontGo.AddComponent<Image>();
        _front.raycastTarget = false;
        _front.enabled = false;
    }

    // One screen-space canvas for all front layers: a copy of the board canvas, but closer to the camera.
    private static Canvas GetFrontCanvas(Canvas board)
    {
        if (_frontCanvas != null) return _frontCanvas;
        var go = new GameObject("WeatherFrontCanvas", typeof(RectTransform));
        go.layer = board.gameObject.layer;
        go.transform.SetParent(board.transform.parent, false);
        _frontCanvas = go.AddComponent<Canvas>();
        _frontCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        _frontCanvas.worldCamera = board.worldCamera;
        _frontCanvas.planeDistance = FrontPlaneDistance;
        _frontCanvas.sortingLayerID = board.sortingLayerID;
        _frontCanvas.sortingOrder = board.sortingOrder;
        var boardScaler = board.GetComponent<CanvasScaler>();
        if (boardScaler != null)
        {
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = boardScaler.uiScaleMode;
            scaler.referenceResolution = boardScaler.referenceResolution;
            scaler.screenMatchMode = boardScaler.screenMatchMode;
            scaler.matchWidthOrHeight = boardScaler.matchWidthOrHeight;
            scaler.scaleFactor = boardScaler.scaleFactor;
            scaler.referencePixelsPerUnit = boardScaler.referencePixelsPerUnit;
        }
        return _frontCanvas;
    }

    // Keeps the front layer exactly over the back layer (they live on different canvases).
    private void LateUpdate()
    {
        if (!_front.enabled || _frontCanvas == null) return;
        var boardCamera = _tint.canvas.rootCanvas.worldCamera;
        _effect.rectTransform.GetWorldCorners(_corners);
        var canvasRect = (RectTransform)_frontCanvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(boardCamera, _corners[0]), _frontCanvas.worldCamera, out var min);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(boardCamera, _corners[2]), _frontCanvas.worldCamera, out var max);
        var rect = _front.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = (min + max) / 2;
        rect.sizeDelta = max - min;
    }

    // The front layer lives on another canvas, so it has to follow this row being shown or hidden.
    private void OnEnable()
    {
        if (_front != null) _front.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (_front != null) _front.gameObject.SetActive(false);
    }

    public void SetWeather(RowStatus weather, Color tintColor)
    {
        if (_transition != null)
        {
            StopCoroutine(_transition);
            _transition = null;
        }
        if (!isActiveAndEnabled)
        {
            SwapEffect(weather);
            SetProgress(_material != null ? 1 : 0);
            _tint.color = _material != null ? Color.clear : tintColor;
            return;
        }
        _transition = StartCoroutine(Transition(weather, tintColor));
    }

    private IEnumerator Transition(RowStatus weather, Color tintColor)
    {
        if (weather != _current && _material != null)
        {
            yield return Animate(_progress, 0, FadeOutTime, null);
        }
        if (weather != _current)
        {
            SwapEffect(weather);
        }
        var tintTarget = _material != null ? Color.clear : tintColor;
        var introTime = WeatherMaterials.IntroTime(weather);
        if (_material != null && _progress <= 0)
        {
            // Rows hit by the same card start a little apart and sweep at slightly different speeds,
            // so they never look like exact copies (each row was a separate effect instance in the original).
            yield return new WaitForSeconds(Random.Range(0f, 0.2f));
            introTime *= Random.Range(0.9f, 1.1f);
            SetFloat(AppliedAtId, Time.timeSinceLevelLoad);
        }
        yield return Animate(_progress, _material != null ? 1 : 0, introTime, tintTarget);
        _transition = null;
    }

    private IEnumerator Animate(float from, float to, float duration, Color? tintTarget)
    {
        var tintFrom = _tint.color;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            // Linear, so the sweep across the row moves at a steady pace like the original.
            var k = time / duration;
            SetProgress(Mathf.Lerp(from, to, k));
            if (tintTarget.HasValue) _tint.color = Color.Lerp(tintFrom, tintTarget.Value, k);
            yield return null;
        }
        SetProgress(to);
        if (tintTarget.HasValue) _tint.color = tintTarget.Value;
    }

    private void SwapEffect(RowStatus weather)
    {
        DestroyMaterials();
        _material = WeatherMaterials.Create(weather);
        if (_material != null)
        {
            // Same material (and so the same random seed) with the front-layer variant of the shader.
            _frontMaterial = new Material(_material);
            _frontMaterial.EnableKeyword("WEATHER_FRONT");
        }
        // Intro particles (meteors, splinters, ...) run on their own clock from this moment; _Time.y in the shader is the time since level load.
        SetFloat(AppliedAtId, Time.timeSinceLevelLoad);
        _current = weather;
        _effect.material = _material;
        _effect.enabled = _material != null;
        _front.material = _frontMaterial;
        _front.enabled = _frontMaterial != null;
        // A new material starts at the shader's default progress (fully shown); hide it until the intro runs.
        SetProgress(0);
    }

    private void SetProgress(float progress)
    {
        _progress = progress;
        SetFloat(ProgressId, progress);
    }

    private void SetFloat(int id, float value)
    {
        if (_material != null) _material.SetFloat(id, value);
        if (_frontMaterial != null) _frontMaterial.SetFloat(id, value);
    }

    private void DestroyMaterials()
    {
        if (_material != null) Destroy(_material);
        if (_frontMaterial != null) Destroy(_frontMaterial);
        _material = null;
        _frontMaterial = null;
    }

    private void OnDestroy()
    {
        DestroyMaterials();
        if (_front != null) Destroy(_front.gameObject);
    }
}

// Builds the material for each animated weather; returns null for weathers that only use a tint.
// The effects are rebuilt from the original Gwent board effects; their textures live in Resources/Sprites/WeatherFX.
public static class WeatherMaterials
{
    // Length of the original intro animations, in seconds.
    public static float IntroTime(RowStatus weather)
    {
        switch (weather)
        {
            case RowStatus.BitingFrost: return 0.93f;
            case RowStatus.ImpenetrableFog: return 1.2f;
            case RowStatus.TorrentialRain: return 1.17f;
            case RowStatus.RaghNarRoog: return 1.03f;
            case RowStatus.KorathiHeatwave: return 2f;
            case RowStatus.SkelligeStorm: return 2f;
            case RowStatus.DragonDream: return 2f;
            case RowStatus.PitTrap: return 2.08f;
            case RowStatus.BloodMoon: return 3.5f;
            case RowStatus.FullMoon: return 3.5f;
            default: return 1.5f;
        }
    }

    // Folder under Resources/Music/Effect with the original sound for this weather (one variant is picked at random).
    public static string Sound(RowStatus weather)
    {
        switch (weather)
        {
            case RowStatus.BitingFrost: return "Weather/Frost";
            case RowStatus.ImpenetrableFog: return "Weather/Fog";
            case RowStatus.TorrentialRain: return "Weather/Rain";
            case RowStatus.RaghNarRoog: return "Weather/RaghNarRoog";
            case RowStatus.KorathiHeatwave: return "Weather/Drought";
            case RowStatus.SkelligeStorm: return "Weather/SkelligeStorm";
            case RowStatus.DragonDream: return "Weather/DragonsDream";
            case RowStatus.PitTrap: return "Weather/PitTrap";
            case RowStatus.BloodMoon: return "Weather/Moonlight";
            case RowStatus.FullMoon: return "Weather/Moonlight";
            case RowStatus.GoldenFroth: return "Weather/GoldenFroth";
            default: return null;
        }
    }

    public static Material Create(RowStatus weather)
    {
        switch (weather)
        {
            case RowStatus.BitingFrost:
                return Build("WeatherFrost",
                    "_EdgeSoft", "CardRowSoftEdgeSimpleRGBMask", "_Overlay", "Frost_MainTex_Overlay1", "_Grain", "AlphaGrain1",
                    "_FrostBase", "Frost_base1", "_DetailMask", "Frost_SmallDetailMask", "_EdgeMask", "Frost_SoftCardRowEdgeMask1",
                    "_Rim", "Frost_Spikes", "_Clusters", "Frost_Cover", "_Sparkles", "Frost_SmallSparcles",
                    "_Mist", "MistSheet1Alpha", "_IceWave", "IceTexTest", "_WaveMask", "Frost_WaveBase1");
            case RowStatus.ImpenetrableFog:
                return Build("WeatherFog",
                    "_EdgeSoft", "CardRowSoftEdgeSimpleRGBMask", "_FogEdge", "Fog_SoftEdgeMask", "_Cloud", "NoiseCloud1_Darker",
                    "_Normal", "FrostNormal1", "_Wave", "Fog_FrontWaveAnimationSheet1_v05");
            case RowStatus.TorrentialRain:
                return Build("WeatherRain",
                    "_EdgeSoft", "CardRowSoftEdgeSimpleRGBMask", "_Wet", "Rain_WetCardRowBase", "_Normal", "FrostNormal1",
                    "_Drops", "WaterDrops1_Alpha", "_Ring", "WaterCircle1", "_Glow", "SolftGrad2_Alpha");
            case RowStatus.RaghNarRoog:
                return Build("WeatherRaghNarRoog",
                    "_Stones", "raghnaroog_background_png", "_Emission", "Mask_raghnarroog_emision", "_Cloud", "NoiseCloud1_Darker",
                    "_Fire", "fire_tileable", "_GroundFire", "bigFire_7x5", "_Wave", "Fog_FrontWaveAnimationSheet1_v05",
                    "_Glow", "VFX_basicSoftGlow_nonlinearGradient");
            case RowStatus.KorathiHeatwave:
                return Build("WeatherDrought",
                    "_Cracks", "DroughtBoard_1024x128", "_EdgeMask", "SoftEdgeMask2", "_Haze", "Noise05NeutralAlpha",
                    "_HazeMask", "NoiseSoftEge", "_Sand", "drummondShieldmaiden_SandDust_Atlas", "_Glow", "Glow01_Alpha");
            case RowStatus.SkelligeStorm:
                return Build("WeatherSkelligeStorm",
                    "_Sea", "SkelligeStormSeaTilingSmall", "_WaveNormal", "WaveNormal2", "_Wavey", "WaveyMask2",
                    "_BoardMask", "BoardMask", "_BoardMaskInv", "BoardMaskInv", "_Caustics", "VFX_Noise18",
                    "_Foam", "SplashesBottom", "_Clouds", "VFX_CloudsMask_1_256x256", "_Lightning", "VFX_GlowBlue_1",
                    "_Splash", "WaveBase", "_Spray", "splash", "_Churn", "Noise05NeutralAlpha", "_WaveTex", "WaveBase");
            case RowStatus.DragonDream:
                return Build("WeatherDragonsDream",
                    "_Mask", "BoardMask_DragonsDream", "_Wispy", "NoiseWispyCurvy", "_NoiseAlpha", "VFX_NoiseAlpha",
                    "_Smoke", "SmokeWispyTileable03_2Sided", "_Flame", "SandDustTiling",
                    "_Head", "RowDragonsDreamSmoke_Head", "_HeadFlow", "RowDragonsDreamSmoke_HeadFlow");
            case RowStatus.PitTrap:
                return Build("WeatherPitTrap",
                    "_Hole", "PitfallTrap_Hole", "_Spikes", "PitfallTrap_Spikes", "_Grass", "PitFallTrap_GrassTexture",
                    "_Cracks", "PitfallTrap_WoodCracks", "_SmallCracks", "PitfallTrap_SmallCracksTexture1",
                    "_Blades", "PitfallTrap_Blades", "_Wood", "PitfallTrap_WoodTexture", "_Puff", "Glow01_Alpha");
            case RowStatus.BloodMoon:
                return Moon(0.71f, 0.16f, -0.73f, new Color(0.721f, 0.048f, 0.048f), new Color(0.309f, 0f, 0f), new Color(0.059f, 0.006f, 0.006f),
                    new Color(0.449f, 0.317f, 0.325f), new Color(0.853f, 0.585f, 0.439f, 1f), new Color(0.191f, 0f, 0.036f), Color.clear);
            case RowStatus.FullMoon:
                return Moon(0.18f, -0.66f, -0.24f, new Color(0f, 0.312f, 0.574f), new Color(0.179f, 0.306f, 0.412f), new Color(0.077f, 0.059f, 0.175f),
                    new Color(0f, 0.71f, 1f), new Color(1f, 1f, 1f, 0.8f), new Color(0f, 0.128f, 0.191f), new Color(0.26f, 0.41f, 0.62f));
            case RowStatus.GoldenFroth:
                return Build("WeatherGoldenFroth",
                    "_Fill", "RainMask", "_Foam", "feast_bubbles", "_Bubble", "single_bubble", "_Burst", "bubble_explo",
                    "_Normal", "ViolentWaterNormal");
            default:
                return null;
        }
    }

    // Blood Moon and Full Moon share one shader with different colours, moon position and glow placement.
    private static Material Moon(float moonX, float glowOffset, float glowMaskOffset, Color tint, Color glow, Color cloudDark, Color cloudLit, Color moon, Color top, Color stars)
    {
        var material = Build("WeatherMoon",
            "_EdgeSoft", "CardRowSoftEdgeSimpleRGBMask", "_SoftGlow", "FX_SoftGlow", "_Blur", "blur_mask",
            "_Clouds", "VFX_CloudsMask_1_256x256", "_Moon", "Moon", "_Gradient", "GRADIENT_01", "_FogMask", "FogMask",
            "_Star", "VFX_SparkStar01");
        material.SetFloat("_MoonX", moonX);
        material.SetFloat("_GlowOffset", glowOffset);
        material.SetFloat("_GlowMaskOffset", glowMaskOffset);
        material.SetColor("_TintColor", tint);
        material.SetColor("_GlowColor", glow);
        material.SetColor("_CloudDark", cloudDark);
        material.SetColor("_CloudLit", cloudLit);
        material.SetColor("_MoonColor", moon);
        material.SetColor("_TopColor", top);
        material.SetColor("_StarColor", stars);
        return material;
    }

    // Creates the material and assigns textures given as property/texture name pairs.
    private static Material Build(string shader, params string[] textures)
    {
        var material = new Material(Resources.Load<Shader>("Shader/" + shader));
        material.SetVector("_Pad", WeatherRowEffect.Pad);
        // Every row gets its own randomness and a random mirror for static textures.
        material.SetFloat("_Seed", Random.value);
        material.SetFloat("_Flip", Random.value < 0.5f ? 0 : 1);
        for (int i = 0; i < textures.Length; i += 2)
        {
            material.SetTexture(textures[i], Resources.Load<Texture2D>("Sprites/WeatherFX/" + textures[i + 1]));
        }
        return material;
    }
}
