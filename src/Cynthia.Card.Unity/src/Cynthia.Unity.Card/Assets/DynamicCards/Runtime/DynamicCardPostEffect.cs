using UnityEngine;

namespace Assets.Script.DynamicCards
{
    public abstract class DynamicCardPostEffect : MonoBehaviour
    {
        public Shader SourceShader;
        private Material material;
        protected abstract void Configure(Material target);

        public Material Prepare()
        {
            if (!isActiveAndEnabled || SourceShader == null) return null;
            if (material == null) material = new Material(SourceShader) { hideFlags = HideFlags.HideAndDontSave };
            Configure(material);
            return material;
        }

        private void OnDestroy()
        {
            if (material == null) return;
            if (Application.isPlaying) Destroy(material); else DestroyImmediate(material);
        }
    }
}
