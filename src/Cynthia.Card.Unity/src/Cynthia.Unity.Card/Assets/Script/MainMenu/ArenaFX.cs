using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//The arena visual effects, rebuilt from the 0.9.24 beta (prefabs UIArenaBackgroundPrefab, UIArenaIntroContent,
//OpponentMirrorEffect, IncineratorRightVFX, UIDBCardAdd). The menu canvas is Screen Space Overlay, so the beta particle
//systems and meshes are redone as RawImages: textured layers with the Arena/FX shader and simple CPU particles.
//Textures, colours, speeds, sizes and timings are the beta values (see arena-research/FINDINGS.md "Arena VFX").
public static class ArenaFX
{
    private static Shader _shader;
    private static readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

    public static Texture2D Texture(string name)
    {
        if (!_textures.TryGetValue(name, out var texture) || texture == null)
            _textures[name] = texture = Resources.Load<Texture2D>("Arena/FX/" + name);
        return texture;
    }

    //settings of one shader layer (names as the beta material properties)
    public class Look
    {
        public string Main;
        public string Noise = "TilingNoise3D_small_uncompressed";
        public Color Tint = Color.white;
        public Vector4 MainScale = new Vector4(1, 1, 0, 0);
        public Vector4 NoiseScale = new Vector4(1, 1, 0, 0);
        public float MainU, MainV, NoiseU, NoiseV, NoiseMult;
        public float ColorGain = 1, AlphaPower = 1, AlphaMult = 1, Intensity = 1;
        public bool Additive = true;
        public Vector2 Edge;
        public Vector4 Polar = new Vector4(0, 0, 0, 1);
        public float TimeOffset;
    }

    public static Material CreateMaterial(Look look)
    {
        if (_shader == null)
            _shader = Resources.Load<Shader>("Shader/ArenaFX");
        var material = new Material(_shader);
        material.SetTexture("_NoiseTex", Texture(look.Noise));
        material.SetColor("_TintColor", look.Tint);
        material.SetVector("_MainScale", look.MainScale);
        material.SetVector("_NoiseScale", look.NoiseScale);
        material.SetFloat("_MainU", look.MainU);
        material.SetFloat("_MainV", look.MainV);
        material.SetFloat("_NoiseU", look.NoiseU);
        material.SetFloat("_NoiseV", look.NoiseV);
        material.SetFloat("_NoiseMult", look.NoiseMult);
        material.SetFloat("_ColorGain", look.ColorGain);
        material.SetFloat("_AlphaPower", look.AlphaPower);
        material.SetFloat("_AlphaMult", look.AlphaMult);
        material.SetFloat("_Intensity", look.Intensity);
        material.SetVector("_Edge", look.Edge);
        material.SetVector("_Polar", look.Polar);
        material.SetFloat("_TimeOffset", look.TimeOffset);
        material.SetFloat("_Additive", look.Additive ? 1 : 0);
        material.SetFloat("_SrcBlend", look.Additive ? (float)UnityEngine.Rendering.BlendMode.One : (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", look.Additive ? (float)UnityEngine.Rendering.BlendMode.One : (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        return material;
    }

    //a layer at a position/size in the parent (centre anchored), optionally rotated
    public static RawImage Layer(string name, Transform parent, Look look, Vector2 position, Vector2 size, float rotation = 0)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localEulerAngles = new Vector3(0, 0, rotation);
        var image = go.AddComponent<RawImage>();
        image.texture = Texture(look.Main);
        image.material = CreateMaterial(look);
        image.raycastTarget = false;
        return image;
    }

    public static RectTransform Container(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rect = (RectTransform)go.transform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = Vector2.zero;
        return rect;
    }

    //linear keyframes (time, value) like the beta animation clips
    public static float Curve(float t, params float[] keys)
    {
        if (t <= keys[0]) return keys[1];
        for (var i = 2; i < keys.Length; i += 2)
            if (t <= keys[i])
                return Mathf.Lerp(keys[i - 1], keys[i + 1], (t - keys[i - 2]) / Mathf.Max(0.0001f, keys[i] - keys[i - 2]));
        return keys[keys.Length - 1];
    }

    public static void Play(string sound) =>
        AudioManager.Instance.PlayAudio("Arena/" + sound, AudioType.Effect, AudioPlayMode.PlayOneShoot);

    //================================================================ effects

    //background of the arena (UIArenaBackgroundPrefab/VFX): green smoke along the bottom, the candle flames and the
    //green glows around both candelabras. stage = a 1600x900 rect that follows the background image.
    public static void Background(RectTransform stage)
    {
        var smoke = Layer("ArenaIntro_Smoke", stage, new Look
        {
            Main = "NoiseSmokeTiling", Noise = "ScorchNoise1", Additive = false, Tint = new Color(0.16f, 0.44f, 0.17f, 0.23f),
            MainScale = new Vector4(2, 1, 0, 0), NoiseScale = new Vector4(5, 2, 0, 0), MainU = -0.033f, MainV = -0.1f, NoiseV = -0.07f,
            NoiseMult = 0.15f, AlphaMult = 2.2f, Edge = new Vector2(0.12f, 0.35f)
        }, new Vector2(0, -235), new Vector2(1700, 340));
        var candles = new GameObject("ArenaIntro_Candles", typeof(RectTransform)).GetComponent<RectTransform>();
        candles.SetParent(stage, false);
        candles.anchorMin = candles.anchorMax = new Vector2(0.5f, 0.5f);
        //the 6 flames: centre x/y on the candle tips (measured in the beta video), quad size of mesh ArenaIntroCandles
        //and texture column (4 flames)
        var flames = new[]
        {
            (-624f, 262f, 30.3f, 40.9f, 1), (-551f, 278f, 30.3f, 41.6f, 0), (-480f, 230f, 30.3f, 41.6f, 3),
            (501f, 230f, 33.1f, 45.1f, 2), (567f, 252f, 33.1f, 45.1f, 1), (638f, 214f, 33.1f, 45.1f, 0)
        };
        var flameLook = new Look
        {
            Main = "ArenaCandleFireAlpha", Tint = new Color(0.37f, 0.96f, 0.22f, 1), ColorGain = 1.5f, NoiseU = 0.05f, NoiseV = -0.6f,
            NoiseMult = 0.05f, NoiseScale = new Vector4(1, 0.3f, 0, 0)
        };
        foreach (var (x, y, w, h, column) in flames)
        {
            flameLook.MainScale = new Vector4(0.25f, 1, column * 0.25f, 0);
            flameLook.TimeOffset = Random.value * 10;
            var flame = Layer("Flame", candles, flameLook, new Vector2(x, y), new Vector2(w, h));
            //the beta mesh uses a quarter of the texture per flame; keep the noise inside that column
            flame.material.SetVector("_Edge", new Vector4(0.08f, 0.03f, 0, 0));
        }
        var glowLook = new Look { Main = "VFX_SoftGlow", Tint = new Color(0.02f, 0.18f, 0.06f, 1) };
        foreach (var position in new[] { new Vector2(-551, 276), new Vector2(543, 257) })
        {
            var glow = Particles.Create("Glow", stage, glowLook, position);
            glow.Rate = 3;
            glow.Life = new Vector2(0.6f, 0.7f);
            glow.StartSize = new Vector2(800, 1500);
            glow.RandomRotation = true;
            glow.AlphaKeys = new[] { 0f, 0, 0.53f, 1, 1, 0 };
        }
        //fade in like the intro clip (smoke 1.43 s, candles 1.17 s)
        var fade = stage.gameObject.AddComponent<Timeline>();
        fade.Duration = 1.5f;
        fade.OnTime = t =>
        {
            smoke.material.SetFloat("_Intensity", Curve(t, 0, 0, 1.433f, 1));
            foreach (var image in candles.GetComponentsInChildren<RawImage>())
                image.material.SetFloat("_Intensity", Curve(t, 0, 0, 0.567f, 0.017f, 0.767f, 0.1f, 0.833f, 0.579f, 1.167f, 1));
        };
    }

    //the welcome banner of the intro (UIArenaIntroContent Title): rolling yellow-green smoke and a glow in the green field.
    //bannerBack = the green field (masked by the caller)
    public static void IntroBanner(RectTransform bannerBack)
    {
        Layer("GlowNoise", bannerBack, new Look
        {
            Main = "GlowSoftClamped", Noise = "ScorchNoise", Tint = new Color(0.08f, 1, 0, 1), AlphaMult = 0.7f,
            MainScale = new Vector4(0.8f, 0.6f, 0.03f, 0.45f), NoiseScale = new Vector4(3, 1, 0, 0), NoiseV = -0.4f, NoiseMult = 0.3f
        }, new Vector2(0, -25), new Vector2(600, 180));
        var smoke = Flipbook.Create("Smoke_FX", bannerBack, "puffy_smoke_masked", 4, 16, new Vector2(-38, -44), new Vector2(666, 600));
        smoke.Color = new Color(0.83f, 1, 0.22f, 0.32f);
        smoke.Life = 3.66f;
        smoke.Interval = 1.066f;
        smoke.AlphaKeys = new[] { 0f, 0, 0.14f, 1, 0.82f, 1, 1, 0 };
    }

    //the swirling smoke inside the matchmaking mirror (OpponentMirrorEffect): 6 spiral smoke layers (ArenaSmoke scrolled
    //along the spiral, green) and dark smoke swirls turning in the centre
    public static void Mirror(RectTransform smokeRoot)
    {
        //dark clouds and light wisps drifting slowly through the green glass
        var layers = new[]
        {
            (new Color(0.1f, 0.3f, 0.12f, 1), 0.55f, 0.025f, 0.5f, 0f),
            (new Color(0.55f, 1, 0.47f, 1), 0.22f, -0.018f, 0.65f, 0.4f),
            (new Color(0.12f, 0.33f, 0.14f, 1), 0.35f, 0.012f, 0.8f, 0.7f)
        };
        for (var i = 0; i < layers.Length; i++)
        {
            var (tint, alpha, speed, tiling, offset) = layers[i];
            Layer("ArenaMirrorSmoke" + i, smokeRoot, new Look
            {
                Main = "ArenaSmoke", Additive = false, Tint = tint, AlphaMult = alpha, MainU = speed, NoiseU = 0.02f, NoiseV = 0.03f,
                NoiseMult = 0.12f, MainScale = new Vector4(tiling, tiling * 0.7f, offset, 0.15f), Edge = new Vector2(0.15f, 0.12f)
            }, new Vector2(0, -10), new Vector2(280, 400));
        }
    }

    //IncineratorRightVFX: the fire at the right end of the card banner when a card is picked (Incinerator_Animation,
    //1.23 s): glow strip, soft glow, flames and a spark texture flare up in 0.37 s and flicker out by 0.8 s,
    //while orange sparks fly to the left. area = the banner between its pillars: the fire burns against the right
    //pillar and is clipped there, it never covers the pillar.
    public static void Incinerator(RectTransform parent, Rect area)
    {
        var clip = Container("IncineratorClip", parent);
        clip.anchoredPosition = area.center;
        clip.sizeDelta = area.size;
        clip.gameObject.AddComponent<RectMask2D>();
        var root = Container("IncineratorRightVFX", clip);
        root.anchoredPosition = new Vector2(area.width / 2 - 25, 0);
        //additive with soft edges: an alpha blended glow reads as a flat orange block
        var glowSoft = Layer("GlowIncineratorSoft", root, new Look
        {
            Main = "Glow01_Alpha", Tint = new Color(1, 0.5f, 0.34f, 1), AlphaMult = 1.2f, Edge = new Vector2(0.35f, 0.3f)
        }, new Vector2(-2, -10), new Vector2(134, 388));
        var glow = Layer("GlowIncinerator", root, new Look
        {
            Main = "Glow02", Tint = new Color(1, 0.43f, 0.29f, 1), ColorGain = 3, AlphaPower = 0.2f, Edge = new Vector2(0, 0.25f)
        }, new Vector2(17, 2), new Vector2(40, 206));
        var flame = Layer("FlameIncinerator", root, new Look
        {
            Main = "VFX_FlameTileable02_2Sided_Alpha", Noise = "Noise3D_Fire_Tiling", NoiseScale = new Vector4(1, 0.75f, 0, 0),
            MainU = 0.78f, NoiseU = 0.23f, NoiseMult = 0.08f
        }, new Vector2(-2, -5), new Vector2(188, 63), 90);
        var sparks = Layer("SparksIncinerator", root, new Look
        {
            Main = "VFX_NoiseFireSparks", Tint = new Color(1, 0.27f, 0.13f, 1), MainScale = new Vector4(0.8f, 1.5f, -0.05f, -0.05f),
            NoiseScale = new Vector4(0.5f, 1.5f, 0, 0), MainU = 2.4f, NoiseU = 0.23f, NoiseMult = 0.1f, AlphaPower = 5, Edge = new Vector2(0.25f, 0.1f)
        }, new Vector2(-47, -6), new Vector2(147, 188));
        var particles = Particles.Create("Sparks_FX", root, new Look { Main = "VFX_Sparks02", Tint = new Color(1, 0.53f, 0.35f, 1) }, new Vector2(16, 5));
        particles.Area = new Vector2(1, 180);
        particles.Life = new Vector2(0.2f, 0.444f);
        particles.StartSize = new Vector2(2, 10);
        particles.Velocity = new Rect(-500, 0, 250, 200);
        particles.Stretch = 4;
        particles.Drag = true;
        particles.SizeKeys = new[] { 0f, 1, 1, 0.13f };
        particles.AlphaKeys = new[] { 0f, 0, 0.12f, 1, 0.9f, 1, 1, 0 };
        var timeline = root.gameObject.AddComponent<Timeline>();
        timeline.Duration = 1.3f;
        timeline.DestroyAtEnd = true;
        timeline.Target = clip.gameObject;
        timeline.OnTime = t =>
        {
            glow.color = new Color(1, 1, 1, Curve(t, 0, 0, 0.367f, 1, 0.4f, 0.259f, 0.433f, 0.996f, 0.467f, 0.356f, 0.5f, 0.992f, 0.567f, 0.525f, 0.6f, 0.653f, 0.7f, 0));
            glowSoft.color = new Color(1, 1, 1, Curve(t, 0, 0, 0.367f, 0.846f, 0.4f, 0.55f, 0.433f, 0.842f, 0.467f, 0.647f, 0.5f, 0.838f, 0.567f, 0.37f, 0.6f, 0.499f, 0.7f, 0));
            flame.material.SetFloat("_Intensity", Curve(t, 0, 0, 0.367f, 1, 0.8f, 0));
            flame.material.SetFloat("_AlphaPower", Curve(t, 0, 10, 0.367f, 1, 0.8f, 10));
            sparks.material.SetFloat("_Intensity", Curve(t, 0, 0, 0.367f, 1.5f, 0.8f, 0));
            sparks.material.SetFloat("_AlphaPower", Curve(t, 0, 5, 0.367f, 1, 0.8f, 5));
            particles.Rate = Curve(t, 0, 0, 0.267f, 60, 0.533f, 48, 0.667f, 0);
        };
    }

    //UIDBCardAdd (0.33 s): blue energy flash over the card, a blue wave sweeping across it, a glowing blue frame and a
    //burst of blue sparks. rect = the card in the parent.
    public static void CardFlash(RectTransform parent, Rect rect)
    {
        var root = Container("UIDBCardAdd", parent);
        root.anchoredPosition = rect.center;
        var size = rect.size;
        var flash = Layer("UIDBCardFlash", root, new Look
        {
            Main = "VFX_NoiseEnergy_NoAlpha", Tint = new Color(0.3f, 0.55f, 1, 1), MainScale = new Vector4(0.75f, 1, 0.8f, 0),
            MainU = 1, NoiseU = 2, NoiseMult = 0.1f, AlphaPower = 2, AlphaMult = 1.3f, Intensity = 0, Edge = new Vector2(0.05f, 0.04f)
        }, Vector2.zero, size);
        var wave = Layer("UICardWave", root, new Look
        {
            Main = "GradientUIShift01", Tint = new Color(0.22f, 0.45f, 1, 1), NoiseV = -0.5f, NoiseMult = 0.05f, AlphaPower = 2,
            Noise = "Noise3D_ForceTiling", Intensity = 0, Edge = new Vector2(0.05f, 0.05f)
        }, Vector2.zero, size, 90);
        wave.rectTransform.sizeDelta = new Vector2(size.y, size.x);
        //the glowing cyan rim of the card (the beta Highlight_CardBoard frame): the hover frame sprite, fading in and out
        var frameObject = new GameObject("Frame", typeof(RectTransform));
        frameObject.transform.SetParent(root, false);
        var frameRect = (RectTransform)frameObject.transform;
        frameRect.sizeDelta = size + new Vector2(26, 10);
        var frame = frameObject.AddComponent<Image>();
        frame.sprite = Resources.Load<Sprite>("Arena/Sprites/navigation_hovered_frame");
        frame.type = Image.Type.Sliced;
        frame.pixelsPerUnitMultiplier = 2.5f;
        frame.fillCenter = false;
        frame.raycastTarget = false;
        frame.color = new Color(1, 1, 1, 0);
        var sparks = Particles.Create("Sparks1_FX", root, new Look { Main = "Hero_Fragments_Atlas_white" }, new Vector2(-size.x / 2, 0));
        sparks.Area = new Vector2(4, size.y);
        sparks.Life = new Vector2(0.2f, 0.4f);
        sparks.StartSize = new Vector2(5, 7);
        sparks.Velocity = new Rect(-190, -60, 380, 120);
        sparks.Frames = 4;
        sparks.ColorFrom = new Color(0.27f, 0.83f, 0.98f);
        sparks.ColorTo = new Color(0.25f, 0.44f, 0.99f);
        sparks.RandomColor = true;
        sparks.SizeKeys = new[] { 0f, 1, 0.53f, 0.66f, 1, 0.16f };
        sparks.AlphaKeys = new[] { 0f, 0, 0.14f, 1, 0.71f, 1, 1, 0 };
        sparks.Burst(Random.Range(15, 21));
        var timeline = root.gameObject.AddComponent<Timeline>();
        timeline.Duration = 0.75f;
        timeline.DestroyAtEnd = true;
        timeline.OnTime = t =>
        {
            flash.material.SetFloat("_Intensity", Curve(t, 0.033f, 0, 0.133f, 2, 0.2f, 0));
            flash.material.SetVector("_MainScale", new Vector4(0.75f, 1, 0.8f + Curve(t, 0, 0, 0.333f, 0.56f), 0));
            wave.material.SetFloat("_Intensity", Curve(t, 0, 1, 0.067f, 1.5f, 0.267f, 1, 0.333f, 0));
            wave.material.SetVector("_MainScale", new Vector4(1, 1, -0.025f, Curve(t, 0, -0.766f, 0.033f, -0.653f, 0.267f, 0.011f)));
            frame.color = new Color(1, 1, 1, Mathf.Clamp01(Curve(t, 0, 0, 0.1f, 1, 0.25f, 0)));
        };
    }

    //================================================================ helpers

    //the beta loading spinner (clip LoadingSpinner_Loop, 1 s loop): the progress line fills in 0.58 s while fading out
    //over the second, the hourglass turns half a turn between 0.67 s and 1 s
    public class Spinner : MonoBehaviour
    {
        public Image Progress;
        public RectTransform Hourglass;
        private float _start;

        private void OnEnable() => _start = Time.time;

        private void Update()
        {
            var t = (Time.time - _start) % 1f;
            if (Progress != null)
            {
                Progress.fillAmount = Curve(t, 0, 0, 0.1f, 0.08f, 0.183f, 0.218f, 0.233f, 0.279f, 0.3f, 0.468f, 0.367f, 0.557f, 0.433f, 0.726f, 0.483f, 0.809f, 0.567f, 0.986f, 0.583f, 1);
                Progress.color = new Color(1, 1, 1, 1 - t);
            }
            if (Hourglass != null)
                Hourglass.localEulerAngles = new Vector3(0, 0, Curve(t, 0, 0, 0.667f, -4.091f, 0.833f, -90, 0.85f, -104.8f, 1, -180));
        }
    }

    //calls OnTime(seconds since start) every frame for Duration seconds
    public class Timeline : MonoBehaviour
    {
        public float Duration = 1;
        public bool DestroyAtEnd;
        public GameObject Target;
        public Action<float> OnTime;
        private float _start = -1;

        private void Update()
        {
            if (_start < 0) _start = Time.time;
            var t = Time.time - _start;
            OnTime?.Invoke(Mathf.Min(t, Duration));
            if (t < Duration) return;
            if (DestroyAtEnd)
                Destroy(Target != null ? Target : gameObject, 0.6f);
            enabled = false;
        }
    }

    //a texture sheet played on a few RawImages that are born every Interval and live Life seconds (beta Smoke_FX)
    public class Flipbook : MonoBehaviour
    {
        public float Life = 3, Interval = 1;
        public Color Color = Color.white;
        public float[] AlphaKeys = { 0f, 1, 1, 1 };
        private int _columns, _rows;
        private readonly List<(RawImage image, float born)> _items = new List<(RawImage, float)>();
        private float _next;

        public static Flipbook Create(string name, Transform parent, string texture, int columns, int rows, Vector2 position, Vector2 size)
        {
            var root = Container(name, parent);
            root.anchoredPosition = position;
            var book = root.gameObject.AddComponent<Flipbook>();
            book._columns = columns;
            book._rows = rows;
            var count = 4;
            for (var i = 0; i < count; i++)
            {
                var go = new GameObject("Puff", typeof(RectTransform));
                go.transform.SetParent(root, false);
                var image = go.AddComponent<RawImage>();
                image.texture = ArenaFX.Texture(texture);
                image.raycastTarget = false;
                image.rectTransform.sizeDelta = size;
                image.enabled = false;
                book._items.Add((image, -100));
            }
            return book;
        }

        private void Update()
        {
            var now = Time.time;
            if (now >= _next)
            {
                _next = now + Interval;
                for (var i = 0; i < _items.Count; i++)
                    if (now - _items[i].born >= Life)
                    {
                        _items[i] = (_items[i].image, now);
                        break;
                    }
            }
            var frames = _columns * _rows;
            foreach (var (image, born) in _items)
            {
                var age = (now - born) / Life;
                image.enabled = age < 1;
                if (age >= 1) continue;
                var frame = Mathf.Min(frames - 1, (int)(age * frames));
                var column = frame % _columns;
                var row = frame / _columns;
                image.uvRect = new Rect((float)column / _columns, 1 - (float)(row + 1) / _rows, 1f / _columns, 1f / _rows);
                var c = Color;
                c.a *= Curve(age, AlphaKeys);
                image.color = c;
            }
        }
    }

    //minimal CPU particle system on RawImages (the beta ParticleSystems): emission rate or bursts, a rectangular emitter,
    //random lifetime/size/velocity, size/alpha/colour over lifetime, spin, stretched sparks and texture-sheet frames
    public class Particles : MonoBehaviour
    {
        public float Rate;
        public Vector2 Life = new Vector2(1, 1);
        public Vector2 StartSize = new Vector2(10, 10);
        public Vector2 Area;
        public Rect Velocity;           //x/y min (position) and range (size)
        public bool Drag;               //velocity fades out over the lifetime (beta clamp velocity)
        public float Stretch;           //>0: stretched along the velocity (beta stretched billboard)
        public bool RandomRotation;
        public float Spin;              //degrees per second
        public int Frames = 1;          //texture sheet rows (1 column)
        public Color Color = Color.white;
        public Color ColorFrom = Color.white, ColorTo = Color.white;
        public bool RandomColor;
        public float[] SizeKeys = { 0f, 1, 1, 1 };
        public float[] AlphaKeys = { 0f, 1, 1, 1 };
        private Material _material;
        private Texture _texture;
        private float _pending;
        private readonly List<Particle> _live = new List<Particle>();
        private readonly Stack<RawImage> _pool = new Stack<RawImage>();

        private class Particle
        {
            public RawImage Image;
            public Vector2 Position, Velocity;
            public float Born, Life, Size, Rotation;
            public Color Tint;
        }

        public static Particles Create(string name, Transform parent, Look look, Vector2 position)
        {
            var root = Container(name, parent);
            root.anchoredPosition = position;
            var particles = root.gameObject.AddComponent<Particles>();
            particles._material = CreateMaterial(look);
            particles._texture = ArenaFX.Texture(look.Main);
            return particles;
        }

        public void Burst(int count)
        {
            for (var i = 0; i < count; i++)
                Emit();
        }

        private void Emit()
        {
            var image = _pool.Count > 0 ? _pool.Pop() : NewImage();
            image.enabled = true;
            var p = new Particle
            {
                Image = image,
                Position = new Vector2((Random.value - 0.5f) * Area.x, (Random.value - 0.5f) * Area.y),
                Velocity = new Vector2(Velocity.x + Random.value * Velocity.width, Velocity.y + Random.value * Velocity.height),
                Born = Time.time,
                Life = Random.Range(Life.x, Life.y),
                Size = Random.Range(StartSize.x, StartSize.y),
                Rotation = RandomRotation ? Random.Range(0, 360f) : 0,
                Tint = RandomColor ? Color.Lerp(ColorFrom, ColorTo, Random.value) : Color
            };
            if (Frames > 1)
            {
                var frame = Random.Range(0, Frames);
                image.uvRect = new Rect(0, (float)frame / Frames, 1, 1f / Frames);
            }
            _live.Add(p);
        }

        private RawImage NewImage()
        {
            var go = new GameObject("P", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var image = go.AddComponent<RawImage>();
            image.texture = _texture;
            image.material = _material;
            image.raycastTarget = false;
            return image;
        }

        private void Update()
        {
            _pending += Rate * Time.deltaTime;
            while (_pending >= 1)
            {
                _pending -= 1;
                Emit();
            }
            var now = Time.time;
            for (var i = _live.Count - 1; i >= 0; i--)
            {
                var p = _live[i];
                var age = (now - p.Born) / p.Life;
                if (age >= 1)
                {
                    p.Image.enabled = false;
                    _pool.Push(p.Image);
                    _live.RemoveAt(i);
                    continue;
                }
                var velocity = Drag ? p.Velocity * (1 - age) : p.Velocity;
                p.Position += velocity * Time.deltaTime;
                p.Rotation += Spin * Time.deltaTime;
                var size = p.Size * Curve(age, SizeKeys);
                var rect = p.Image.rectTransform;
                rect.anchoredPosition = p.Position;
                if (Stretch > 0 && velocity.sqrMagnitude > 1)
                {
                    rect.sizeDelta = new Vector2(size * Stretch, size);
                    rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
                }
                else
                {
                    rect.sizeDelta = new Vector2(size, size);
                    rect.localEulerAngles = new Vector3(0, 0, p.Rotation);
                }
                var tint = p.Tint;
                if (!RandomColor && (ColorFrom != Color.white || ColorTo != Color.white))
                    tint *= Color.Lerp(ColorFrom, ColorTo, age);
                tint.a = p.Tint.a * Curve(age, AlphaKeys);
                p.Image.color = tint;
            }
        }
    }
}
