using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // Original CraftPremium update/release timing; effects stay attached to existing card images.
    public sealed class PremiumCraftEffect : MonoBehaviour
    {
        public const float RevealTime = 2.966667f;
        public const float Duration = 6.6f;
        private readonly List<GameObject> effects = new List<GameObject>();
        private TaskCompletionSource<bool> completion;
        private Material glowMaterial;
        public Task Play(IEnumerable<RectTransform> targets, Action reveal)
        {
            completion = new TaskCompletionSource<bool>();
            StartCoroutine(Animate(targets, reveal));
            return completion.Task;
        }
        private IEnumerator Animate(IEnumerable<RectTransform> targets, Action reveal)
        {
            glowMaterial = new Material(Resources.Load<Shader>("PremiumCrafting/PremiumCraftGlow"));
            var layers = new List<RawImage[]>();
            var particles = new List<PremiumCraftParticles>();
            foreach (var target in targets)
            {
                if (target == null || !target.gameObject.activeInHierarchy) continue;
                var clip = PremiumCollectionPanel.Rect("PremiumCraftInPlace", target, new Vector2(.5f,.5f), Vector2.zero, Vector2.zero);
                clip.anchorMin=Vector2.zero; clip.anchorMax=Vector2.one; clip.offsetMin=clip.offsetMax=Vector2.zero;
                clip.gameObject.AddComponent<RectMask2D>();
                effects.Add(clip.gameObject);
                var root = PremiumCollectionPanel.Rect("LocalGlow", clip, new Vector2(.5f,.5f), Vector2.zero, Vector2.one * 850);
                root.localScale = Vector3.one * (target.rect.width / 500f);
                var halo = Glow(root, "Glow_green", 1050);
                var ring = PremiumCollectionPanel.Rect("Dust", root, new Vector2(.5f,.5f), Vector2.zero, Vector2.one * 850).gameObject.AddComponent<PremiumCraftParticles>();
                ring.raycastTarget = false; ring.material = glowMaterial; particles.Add(ring);
                layers.Add(new[] { halo, Glow(root,"VFX_halfringSlash1",700), Glow(root,"VFX_halfringSlash1",730),
                    Glow(root,"VFX_SphericalGlow",850), Glow(root,"Streak",1200) });
            }
            float age = 0;
            bool revealed = false;
            while (age < Duration)
            {
                age += Time.unscaledDeltaTime;
                if (!revealed && age >= RevealTime)
                {
                    revealed = true;
                    try { reveal(); }
                    catch (Exception e) { Cleanup(); completion.TrySetException(e); Destroy(this); yield break; }
                }
                float charge = Mathf.Clamp01(age / RevealTime);
                float pulse = Mathf.Exp(-Mathf.Pow((age - RevealTime) * 7, 2));
                float tail = Mathf.Clamp01(1 - Mathf.Max(0, age - RevealTime) / 2.8f);
                foreach (var ring in particles) if (ring != null) { ring.Age = age; ring.SetVerticesDirty(); }
                foreach (var layer in layers)
                {
                    if (layer[0] == null) continue;
                    layer[0].color = new Color(.5f,1,.65f,charge * .16f * tail);
                    layer[1].color = layer[2].color = new Color(.2f,1,.65f,charge * .28f * tail);
                    layer[1].rectTransform.localRotation = Quaternion.Euler(0,0,age * 125);
                    layer[2].rectTransform.localRotation = Quaternion.Euler(0,0,-age * 95 + 180);
                    layer[3].color = new Color(.45f,1,.68f,pulse);
                    layer[4].color = new Color(.4f,1,.75f,pulse * .8f);
                }
                yield return null;
            }
            Cleanup(); completion.TrySetResult(true); Destroy(this);
        }
        private RawImage Glow(Transform parent, string texture, float size)
        {
            var image = PremiumCollectionPanel.Rect(texture,parent,new Vector2(.5f,.5f),Vector2.zero,Vector2.one * size).gameObject.AddComponent<RawImage>();
            image.texture=Resources.Load<Texture2D>("PremiumCrafting/"+texture); image.material=glowMaterial; image.raycastTarget=false;
            image.color=Color.clear;
            return image;
        }
        private void Cleanup()
        {
            foreach (var effect in effects) if (effect != null) Destroy(effect);
            effects.Clear();
            if (glowMaterial != null) Destroy(glowMaterial);
        }
        private void OnDisable() { StopAllCoroutines(); Cleanup(); completion?.TrySetResult(false); }
    }

    public sealed class PremiumCraftParticles : MaskableGraphic
    {
        public float Age;
        private Texture star;
        public override Texture mainTexture => star != null ? star : (star = Resources.Load<Texture2D>("PremiumCrafting/StarGlow"));
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            float charge = Mathf.Clamp01(Age / PremiumCraftEffect.RevealTime);
            float after = Age - PremiumCraftEffect.RevealTime;
            float fade = after < 0 ? charge : Mathf.Clamp01(1 - after / 2.5f);
            for (int i = 0; i < 100; i++)
            {
                float seed = Mathf.Repeat(i * .618034f, 1), angle = i * 2.399963f + Age * (.1f + seed);
                float r = after < 0 ? Mathf.Lerp(420, 100 + seed * 140, charge) : 100 + seed * 150 + after * (80 + seed * 100);
                var p = Point(angle, r); float s = 4 + seed * 9;
                var c = new Color(.35f + seed * .5f, 1, .55f + seed * .4f, fade * (.3f + .7f * Mathf.Abs(Mathf.Sin(Age * 5 + i))));
                Quad(vh, p + new Vector2(-s, -s), p + new Vector2(-s, s), p + new Vector2(s, s), p + new Vector2(s, -s), c);
            }
        }
        private static Vector2 Point(float a, float r) => new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
        private static void Quad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
        {
            int start = vh.currentVertCount;
            vh.AddVert(a, color, new Vector2(0,0)); vh.AddVert(b, color, new Vector2(0,1)); vh.AddVert(c, color, new Vector2(1,1)); vh.AddVert(d, color, new Vector2(1,0));
            vh.AddTriangle(start, start + 1, start + 2); vh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
