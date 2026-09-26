using System;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // Original premium effects store RGBA-half vertex positions; no foreign runtime is required.
    public sealed class DynamicCardVertexAnimation : MonoBehaviour
    {
        public TextAsset Positions;
        public Mesh SourceMesh;
        public int Samples, Vertices;
        public float FramesPerSecond = 30;
        private Mesh mesh;
        private Vector3[] frames, current;
        private bool vertexMajor;
        private float age;
        private Texture2D positionTexture;
        private Renderer targetRenderer;
        private MaterialPropertyBlock propertyBlock;

        private void Awake() { Initialize(); }
        public void Initialize()
        {
            if(mesh!=null || positionTexture!=null)return;
            if (Positions == null || SourceMesh == null || Samples <= 0 || Vertices <= 0) { enabled = false; return; }
            byte[] bytes = Positions.bytes;
            if (bytes.Length != Samples * Vertices * 8 || SourceMesh.vertexCount != Vertices) { enabled = false; return; }
            targetRenderer=GetComponent<Renderer>();
            if(targetRenderer!=null && targetRenderer.sharedMaterial!=null && targetRenderer.sharedMaterial.HasProperty("_VertexAnimation"))
            {
                // The native vertex shader addresses time horizontally and vertex index vertically.
                positionTexture=new Texture2D(Samples,Vertices,TextureFormat.RGBAHalf,false,true);
                positionTexture.name="Dynamic vertex positions";positionTexture.wrapMode=TextureWrapMode.Clamp;
                positionTexture.wrapModeU=TextureWrapMode.Repeat;positionTexture.filterMode=FilterMode.Bilinear;
                positionTexture.LoadRawTextureData(bytes);positionTexture.Apply(false,true);
                propertyBlock=new MaterialPropertyBlock();ApplyNative();return;
            }
            frames = new Vector3[Samples * Vertices];current = new Vector3[Vertices];
            for (int i = 0; i < frames.Length; i++) frames[i] = new Vector3(Half(bytes,i*8),Half(bytes,i*8+2),Half(bytes,i*8+4));
            var rest = SourceMesh.vertices;double frameError = 0, vertexError = 0;
            for (int i = 0; i < Vertices; i++)
            { frameError += (rest[i] - frames[i]).sqrMagnitude;vertexError += (rest[i] - frames[i*Samples]).sqrMagnitude; }
            vertexMajor = vertexError < frameError;
            mesh = Instantiate(SourceMesh);mesh.MarkDynamic();
            var filter = GetComponent<MeshFilter>();if (filter != null) filter.sharedMesh = mesh;
            var particleRenderer = GetComponent<ParticleSystemRenderer>();if (particleRenderer != null) particleRenderer.mesh = mesh;
        }
        private void Update()
        {
            Advance(Time.unscaledDeltaTime);
        }
        public void Advance(float delta)
        {
            age+=delta;
            if(positionTexture!=null){ApplyNative();return;}
            if (mesh == null) return;
            float position = Mathf.Repeat(age * FramesPerSecond, Samples);
            int first = Mathf.FloorToInt(position), second = (first+1)%Samples;
            for (int i = 0; i < Vertices; i++)
                current[i] = Vector3.Lerp(frames[vertexMajor?i*Samples+first:first*Vertices+i],frames[vertexMajor?i*Samples+second:second*Vertices+i],position-first);
            mesh.vertices = current;mesh.RecalculateBounds();
        }
        private void ApplyNative()
        {
            for(int i=0;i<targetRenderer.sharedMaterials.Length;i++)
            {
                var material=targetRenderer.sharedMaterials[i];if(material==null || !material.HasProperty("_VertexAnimation"))continue;
                targetRenderer.GetPropertyBlock(propertyBlock,i);propertyBlock.SetTexture("_VertexAnimation",positionTexture);
                propertyBlock.SetVector("_VertexAnimation_TexelSize",new Vector4(1f/Samples,1f/Vertices,Samples,Vertices));
                if(material.HasProperty("_AnimationTime"))propertyBlock.SetFloat("_AnimationTime",Mathf.Repeat(age*FramesPerSecond/Samples,1));
                targetRenderer.SetPropertyBlock(propertyBlock,i);
            }
        }
        private void OnDestroy()
        {
            if(Application.isPlaying){if(mesh!=null)Destroy(mesh);if(positionTexture!=null)Destroy(positionTexture);}
            else{if(mesh!=null)DestroyImmediate(mesh);if(positionTexture!=null)DestroyImmediate(positionTexture);}
        }
        private static float Half(byte[] bytes,int offset)
        {
            uint half = (uint)(bytes[offset] | bytes[offset+1]<<8);
            uint sign = (half & 0x8000) << 16, exponent = (half >> 10) & 31, mantissa = half & 1023;
            if (exponent == 0) return (half & 0x8000) == 0 ? mantissa / 16777216f : -mantissa / 16777216f;
            uint bits = sign | (exponent == 31 ? 0x7f800000 : (exponent+112)<<23) | mantissa<<13;
            return BitConverter.ToSingle(BitConverter.GetBytes(bits),0);
        }
    }
}
