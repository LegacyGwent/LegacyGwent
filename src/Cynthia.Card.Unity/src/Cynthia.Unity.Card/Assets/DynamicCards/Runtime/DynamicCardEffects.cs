using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    public static class DynamicCardPaths
    {
        public static string RelativePath(Transform node, Transform root)
        {
            if (node == root) return "";
            string path = node.name;
            for (var parent = node.parent; parent != null && parent != root; parent = parent.parent)
                path = parent.name + "/" + path;
            return path;
        }
        public static Transform Find(Transform root, string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var exact = root.Find(path);if (exact != null) return exact;
            // Unity expands optimized avatars during conversion. Exposed bones can gain parents.
            string name = path.Substring(path.LastIndexOf('/') + 1);Transform match = null;int best = -1;bool ambiguous=false;
            foreach (var candidate in root.GetComponentsInChildren<Transform>(true))
                if (candidate.name == name)
                {
                    string actual=candidate.name;
                    for(var parent=candidate.parent;parent!=null && parent!=root;parent=parent.parent)actual=parent.name+"/"+actual;
                    var expectedParts=path.Split('/');var actualParts=actual.Split('/');int score=0;
                    while(score<expectedParts.Length && score<actualParts.Length && expectedParts[score]==actualParts[score])score++;
                    int ordered=0;foreach(var part in actualParts)if(ordered<expectedParts.Length && part==expectedParts[ordered])ordered++;
                    score=ordered==expectedParts.Length?10000+ordered-actualParts.Length:score*2+((actual.IndexOf("/VFX/",System.StringComparison.OrdinalIgnoreCase)>=0)==(path.IndexOf("/VFX/",System.StringComparison.OrdinalIgnoreCase)>=0)?1:0);
                    if(score>best){best=score;match=candidate;ambiguous=false;}else if(score==best)ambiguous=true;
                }
            return ambiguous?null:match;
        }
    }
    // Replaces source-game script dependencies using exported paths and original event times.
    public sealed class DynamicCardEffects : MonoBehaviour
    {
        private sealed class UvState
        {
            public Renderer renderer;
            public DynamicCardUvMotion motion;
            public Vector4 initial;
            public MaterialPropertyBlock block = new MaterialPropertyBlock();
        }
        private DynamicCardEntry entry;
        private ParticleSystem[][] particles;
        private readonly List<UvState> uv = new List<UvState>();
        private readonly List<Transform[]> pairs = new List<Transform[]>();
        private float previous = -0.0001f;
        public void Initialize(DynamicCardEntry data)
        {
            entry = data;
            if (data.transformPairs != null)
                foreach (var pair in data.transformPairs)
                {
                    var source = DynamicCardPaths.Find(transform,pair.source);var target = DynamicCardPaths.Find(transform,pair.target);
                    if (source != null && target != null) pairs.Add(new[] { source, target });
                }
            if (data.particleEvents != null)
            {
                particles = new ParticleSystem[data.particleEvents.Length][];
                for (int i = 0; i < particles.Length; i++)
                {
                    var target = DynamicCardPaths.Find(transform,data.particleEvents[i].path);
                    particles[i] = target == null ? new ParticleSystem[0] : target.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (var particle in particles[i]) particle.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
            if (data.uvMotions == null) return;
            foreach (var motion in data.uvMotions)
            {
                var target = DynamicCardPaths.Find(transform,motion.path);
                var renderer = target == null ? null : target.GetComponent<Renderer>();
                if (renderer == null || motion.materialIndex >= renderer.sharedMaterials.Length) continue;
                var material = renderer.sharedMaterials[motion.materialIndex];
                if (material == null || !material.HasProperty(motion.property)) continue;
                var scale = material.GetTextureScale(motion.property);
                var offset = motion.forceStart ? motion.start : material.GetTextureOffset(motion.property);
                uv.Add(new UvState { renderer = renderer, motion = motion, initial = new Vector4(scale.x, scale.y, offset.x, offset.y) });
            }
        }

        public void Tick(float time)
        {
            if (entry == null) return;
            foreach (var pair in pairs) { pair[1].position = pair[0].position;pair[1].rotation = pair[0].rotation; }
            if (particles != null)
                for (int i = 0; i < particles.Length; i++)
                {
                    var group = particles[i];
                    var item = entry.particleEvents[i];
                    float trigger = item.time + (item.sourceTiming ? item.phaseStart : item.loop ? entry.introDuration : 0);
                    float period=item.sourceTiming?item.period:entry.loopDuration;
                    bool play = previous < trigger && time >= trigger;
                    if (item.loop && period > 0 && time >= trigger)
                        play |= Mathf.FloorToInt((time - trigger) / period) > Mathf.FloorToInt((previous - trigger) / period);
                    if (play)
                    {
                        var root = DynamicCardPaths.Find(transform,item.path);
                        for (var parent = root; parent != null && parent != transform; parent = parent.parent) parent.gameObject.SetActive(true);
                        foreach (var particle in group) { particle.gameObject.SetActive(true); particle.Play(false); }
                    }
                }
            foreach (var item in uv)
            {
                var value = item.initial;
                value.z += time * item.motion.speed.x; value.w += time * item.motion.speed.y;
                item.renderer.GetPropertyBlock(item.block, item.motion.materialIndex);
                item.block.SetVector(item.motion.property + "_ST", value);
                item.renderer.SetPropertyBlock(item.block, item.motion.materialIndex);
            }
            previous = time;
        }
    }
}
