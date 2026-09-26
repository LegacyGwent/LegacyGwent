using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // The source card renderer applies each original pass twice. Keep this on the card's own camera.
    public sealed class DynamicCardPostProcessRenderer : MonoBehaviour
    {
        public DynamicCardPostEffect[] Effects;
        [System.NonSerialized] public int Downsample = 1;
        private readonly List<Material> active = new List<Material>();

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            active.Clear();
            if (Effects != null)
                foreach (var effect in Effects)
                {
                    if (effect == null) continue;
                    var material = effect.Prepare();
                    if (material != null) active.Add(material);
                }
            if (active.Count == 0) { Graphics.Blit(source, destination); return; }
            RenderTexture reducedSource = null, reducedResult = null;
            RenderTexture owned = null;
            try
            {
                if (Downsample > 1)
                {
                    int width = Mathf.Max(1, source.width / Downsample), height = Mathf.Max(1, source.height / Downsample);
                    reducedSource = RenderTexture.GetTemporary(width, height, 0, source.format);
                    reducedResult = RenderTexture.GetTemporary(width, height, 0, source.format);
                    reducedSource.filterMode = reducedResult.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(source, reducedSource);
                }
                var input = reducedSource != null ? reducedSource : source;
                var output = reducedResult != null ? reducedResult : destination;
                RenderTexture current = input;
                for (int i = 0; i < active.Count; i++)
                {
                    RenderTexture target = null, scratch = null;
                    bool ownsTarget = i != active.Count - 1;
                    try
                    {
                        target = ownsTarget ? RenderTexture.GetTemporary(input.width, input.height, 0, input.format) : output;
                        scratch = RenderTexture.GetTemporary(input.width, input.height, 0, input.format);
                        Graphics.Blit(current, scratch, active[i], 0);
                        Graphics.Blit(scratch, target, active[i], 0);
                        if (owned != null) RenderTexture.ReleaseTemporary(owned);
                        owned = ownsTarget ? target : null;
                        current = target;
                        ownsTarget = false; // Ownership transferred to the next pass / outer finally.
                    }
                    finally
                    {
                        if (scratch != null) RenderTexture.ReleaseTemporary(scratch);
                        if (ownsTarget && target != null) RenderTexture.ReleaseTemporary(target);
                    }
                }
                if (reducedResult != null) Graphics.Blit(reducedResult, destination);
            }
            finally
            {
                if (owned != null) RenderTexture.ReleaseTemporary(owned);
                if (reducedSource != null) RenderTexture.ReleaseTemporary(reducedSource);
                if (reducedResult != null) RenderTexture.ReleaseTemporary(reducedResult);
            }
        }
    }
}
