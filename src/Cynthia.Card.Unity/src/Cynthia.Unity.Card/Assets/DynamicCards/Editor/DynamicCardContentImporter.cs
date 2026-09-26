using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    public static class DynamicCardContentImporter
    {
        [Serializable] private class MaterialInfo { public string asset, originalName, shader, portableShader, renderType; public bool hasState; public float srcBlend,dstBlend,zWrite,cull; public int queue; }
        [Serializable] private class Assignment { public string material, materialAsset, texture; public string[] properties; }
        [Serializable] private class VertexInfo { public string path, data; public int samples, vertices; }
        [Serializable] private class CandleInfo { public string path, texture; public float size; }
        [Serializable] private class CurveKey { public float time, value, inSlope, outSlope; }
        [Serializable] private class SectionInfo { public float start, length; public Color tint; public CurveKey[] opacity, width, buildUp, noise, speed; }
        [Serializable] private class LightningInfo { public string path, start, end, texture; public Vector3[] points; public float width, duration; public SectionInfo[] sections; }
        [Serializable] private class Conversion { public string id, atlas; public int particles; public MaterialInfo[] materials; public Assignment[] textureAssignments; public VertexInfo[] vertexAnimations; public CandleInfo[] candles; public LightningInfo[] lightning; }

        [MenuItem("Tools/Dynamic Cards/Prepare Imported Content")]
        public static void Prepare()
        {
            AssetDatabase.Refresh();
            var shader = Shader.Find("DynamicCards/PortableCard");
            if (shader == null) throw new InvalidOperationException("Portable card shader missing.");
            int cards = 0, particleCount = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
            foreach (var path in Directory.GetFiles(DynamicCardLibrary.ContentRoot, "conversion.json", SearchOption.AllDirectories))
            {
                string selectedRoot = Environment.GetEnvironmentVariable("DYNAMIC_PREPARE_ROOT");
                if (!string.IsNullOrEmpty(selectedRoot) && !path.Replace('\\','/').StartsWith(selectedRoot, StringComparison.Ordinal)) continue;
                var conversion = JsonUtility.FromJson<Conversion>(File.ReadAllText(path));
                string selected = Environment.GetEnvironmentVariable("DYNAMIC_PREPARE_IDS");
                if (!string.IsNullOrEmpty(selected) && !selected.Split(',').Contains(conversion.id)) continue;
                foreach (var info in conversion.materials)
                {
                    var material = AssetDatabase.LoadAssetAtPath<Material>(info.asset);
                    if (material == null) throw new InvalidOperationException("Missing dynamic material: " + info.asset);
                    ConvertMaterial(material, info, shader);
                    ApplyTextureAssignments(material, info, conversion);
                }
                string prefabPath = Path.GetDirectoryName(path).Replace('\\', '/') + "/Card.prefab";
                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    foreach (Transform child in root.transform)
                        if (child.localPosition.sqrMagnitude > 1000000) child.localPosition = Vector3.zero;
                    // This source scene retains an authoring half-turn that puts its entire backdrop behind the source camera.
                    if (conversion.id == "11240201") root.transform.Find(conversion.id).localRotation = Quaternion.identity;
                    foreach (var animator in root.GetComponentsInChildren<Animator>(true))
                    { animator.updateMode = AnimatorUpdateMode.UnscaledTime; animator.fireEvents = false; animator.keepAnimatorControllerStateOnDisable = true; }
                    foreach (var particle in root.GetComponentsInChildren<ParticleSystem>(true))
                    { var main = particle.main; main.useUnscaledTime = true; var renderer=particle.GetComponent<ParticleSystemRenderer>();if(renderer!=null)renderer.enableGPUInstancing=false; }
                    PrepareProceduralEffects(root, conversion, shader, Path.GetDirectoryName(prefabPath));
                    int actual = root.GetComponentsInChildren<ParticleSystem>(true).Length;
                    if (actual != conversion.particles) throw new InvalidOperationException(conversion.id + ": particle count changed in migration.");
                    particleCount += actual;
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
                cards++;
            }
            AssetDatabase.SaveAssets();
            }
            finally { AssetDatabase.SaveAssets(); AssetDatabase.StopAssetEditing(); }
            PrepareFormats();
            Debug.Log("DYNAMIC_CONTENT_READY cards=" + cards + " particles=" + particleCount);
        }

        public static void FinishImports()
        {
            AssetDatabase.Refresh();
            PrepareFormats();
            int cards = 0, particles = 0;
            foreach (var path in Directory.GetFiles(DynamicCardLibrary.ContentRoot, "Card.prefab", SearchOption.AllDirectories))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path.Replace('\\', '/'));
                if (prefab == null) throw new InvalidOperationException("Missing prefab " + path);
                cards++;particles += prefab.GetComponentsInChildren<ParticleSystem>(true).Length;
            }
            Debug.Log("DYNAMIC_CONTENT_READY cards=" + cards + " particles=" + particles);
        }

        public static void PrepareMaterials()
        {
            var shader = Shader.Find("DynamicCards/PortableCard");
            if(shader==null)throw new InvalidOperationException("Portable card shader missing");
            int count=0;
            foreach(var path in Directory.GetFiles(DynamicCardLibrary.ContentRoot,"conversion.json",SearchOption.AllDirectories))
            {
                var conversion=JsonUtility.FromJson<Conversion>(File.ReadAllText(path));
                foreach(var info in conversion.materials)
                {
                    var material=AssetDatabase.LoadAssetAtPath<Material>(info.asset);
                    if(material==null)throw new InvalidOperationException("Missing material "+info.asset);
                    ConvertMaterial(material,info,shader);
                    ApplyTextureAssignments(material, info, conversion);
                    count++;
                }
            }
            AssetDatabase.SaveAssets();Debug.Log("DYNAMIC_MATERIALS_READY "+count);
        }

        private static void ApplyTextureAssignments(Material material, MaterialInfo info, Conversion conversion)
        {
            if (conversion.textureAssignments == null) return;
            // One material can have separate atlas, light-mask and shared-texture bindings.
            foreach (var assignment in conversion.textureAssignments.Where(a => a.material == info.originalName &&
                (string.IsNullOrEmpty(a.materialAsset) || a.materialAsset == info.asset)))
            foreach (var property in assignment.properties ?? new string[0])
            {
                if (!material.HasProperty(property)) continue;
                string path = string.IsNullOrEmpty(assignment.texture) ? conversion.atlas : assignment.texture;
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (texture == null) throw new InvalidOperationException("Missing source texture for " + info.originalName + " " + property + ": " + path);
                material.SetTexture(property, texture);
            }
        }

        public static void NormalizeRootClips()
        {
            int count=0;
            // These two generic clips retain the source scene's parking offset. Their imported
            // 2022 curve cache is not exposed by AnimationUtility in 2019; edit its source curve.
            foreach(var path in Directory.GetFiles(DynamicCardLibrary.ContentRoot+"14050100","14050100__*.anim"))
            {
                string text=File.ReadAllText(path);
                string replaced=System.Text.RegularExpressions.Regex.Replace(text,@"(?s)m_PositionCurves:\s*\r?\n  - curve:.*?\r?\n    path: *\r?\n",m=>m.Value.Replace("y: -10000","y: 0"));
                if(replaced==text)continue;File.WriteAllText(path,replaced);count++;
            }
            AssetDatabase.Refresh();Debug.Log("DYNAMIC_ROOT_CLIPS_NORMALIZED "+count);
        }

        private static void PrepareFormats()
        {
            string selectedRoot = Environment.GetEnvironmentVariable("DYNAMIC_PREPARE_ROOT");
            string formatRoot = string.IsNullOrEmpty(selectedRoot) ? DynamicCardLibrary.ContentRoot : selectedRoot;
            // Queue importer changes together; rescanning the whole client per texture is costly.
            AssetDatabase.StartAssetEditing();
            try
            {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { formatRoot.TrimEnd('/') }))
            {
                var importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as TextureImporter;
                if (importer == null) continue;
                if (importer.textureCompression != TextureImporterCompression.CompressedHQ)
                { importer.textureCompression = TextureImporterCompression.CompressedHQ; importer.alphaIsTransparency = true; importer.SaveAndReimport(); }
            }
            foreach (var guid in AssetDatabase.FindAssets("t:AudioClip", new[] { formatRoot.TrimEnd('/') }))
            {
                var importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as AudioImporter;
                if (importer == null) continue;
                var settings = importer.defaultSampleSettings;
                if(settings.loadType==AudioClipLoadType.CompressedInMemory && settings.compressionFormat==AudioCompressionFormat.Vorbis && Mathf.Abs(settings.quality-.65f)<.001f && !importer.preloadAudioData)continue;
                settings.loadType = AudioClipLoadType.CompressedInMemory;
                settings.compressionFormat = AudioCompressionFormat.Vorbis; settings.quality = .65f;
                importer.defaultSampleSettings = settings; importer.preloadAudioData = false; importer.SaveAndReimport();
            }
            }
            finally { AssetDatabase.StopAssetEditing(); }
            AssetDatabase.SaveAssets();
        }

        private static void PrepareProceduralEffects(GameObject root, Conversion conversion, Shader shader, string cardDirectory)
        {
            string directory = cardDirectory + "/Generated";
            if ((conversion.vertexAnimations != null && conversion.vertexAnimations.Length > 0) || (conversion.candles != null && conversion.candles.Length > 0) || (conversion.lightning != null && conversion.lightning.Length > 0))
            { Directory.CreateDirectory(directory);AssetDatabase.Refresh(); }
            int index = 0;
            foreach (var info in conversion.vertexAnimations ?? new VertexInfo[0])
            {
                var target = DynamicCardPaths.Find(root.transform,info.path);if (target == null) throw new InvalidOperationException("Missing vertex animation object: " + info.path);
                var filter = target.GetComponent<MeshFilter>();var particle = target.GetComponent<ParticleSystemRenderer>();
                Mesh source = filter != null ? filter.sharedMesh : particle != null ? particle.mesh : null;
                if (source == null || source.vertexCount != info.vertices) throw new InvalidOperationException("Vertex animation mesh mismatch: " + info.path);
                var mesh = new Mesh { name = source.name, indexFormat = source.indexFormat };
                mesh.vertices = source.vertices;mesh.normals = source.normals;mesh.uv = source.uv;mesh.uv2 = source.uv2;mesh.colors = source.colors;mesh.tangents = source.tangents;
                mesh.subMeshCount = source.subMeshCount;
                for (int i = 0; i < source.subMeshCount; i++) mesh.SetIndices(source.GetIndices(i), source.GetTopology(i), i);
                mesh.bounds = source.bounds;
                string path = directory + "/VertexMesh" + index++ + ".asset";AssetDatabase.CreateAsset(mesh, path);
                var animation = target.GetComponent<DynamicCardVertexAnimation>();
                if (animation == null) animation = target.gameObject.AddComponent<DynamicCardVertexAnimation>();
                animation.SourceMesh = mesh;animation.Positions = AssetDatabase.LoadAssetAtPath<TextAsset>(info.data);animation.Samples = info.samples;animation.Vertices = info.vertices;
            }
            index = 0;
            var sourceCandleAnchors = new HashSet<Transform>(root.GetComponentsInChildren<SourceParticles.SourceCandleFire>(true)
                .SelectMany(c => c.Positions));
            foreach (var info in conversion.candles ?? new CandleInfo[0])
            {
                var anchor = DynamicCardPaths.Find(root.transform,info.path);if (anchor == null) throw new InvalidOperationException("Missing candle anchor: " + info.path);
                if (sourceCandleAnchors.Contains(anchor)) { index++; continue; }
                const string name = "DynamicCandle";
                var existing = anchor.Find(name);var quad = existing != null ? existing.gameObject : GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = name;quad.transform.SetParent(anchor, false);quad.transform.localPosition = Vector3.zero;quad.transform.localScale = Vector3.one * info.size;
                var collider = quad.GetComponent<Collider>();if (collider != null) UnityEngine.Object.DestroyImmediate(collider);
                var material = new Material(shader);material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(info.texture);
                material.SetFloat("_SrcBlend",5);material.SetFloat("_DstBlend",1);material.SetFloat("_Cull",0);
                material.SetVector("_UVSpeed",new Vector4(.015f,.13f,0,0));material.renderQueue = 4100;
                AssetDatabase.CreateAsset(material,directory + "/Candle" + index++ + ".mat");quad.GetComponent<MeshRenderer>().sharedMaterial = material;
            }
            index = 0;
            foreach (var info in conversion.lightning ?? new LightningInfo[0])
            {
                var anchor = DynamicCardPaths.Find(root.transform,info.path);if (anchor == null) throw new InvalidOperationException("Missing lightning anchor: " + info.path);
                if (anchor.GetComponent<SourceLightning.LightningTool>() != null) { index++; continue; }
                string name = "DynamicLightning" + index;var existing = anchor.Find(name);
                var go = existing != null ? existing.gameObject : new GameObject(name,typeof(LineRenderer));go.transform.SetParent(anchor,false);
                var line = go.GetComponent<LineRenderer>();line.useWorldSpace = true;line.widthCurve = AnimationCurve.Linear(0,1,1,0);
                var material = new Material(shader);material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(info.texture);
                material.SetFloat("_SrcBlend",5);material.SetFloat("_DstBlend",1);material.SetFloat("_Cull",0);material.renderQueue = 3255;
                AssetDatabase.CreateAsset(material,directory + "/Lightning" + index++ + ".mat");line.sharedMaterial = material;
                var lightning = go.GetComponent<DynamicCardLightning>();if (lightning == null) lightning = go.AddComponent<DynamicCardLightning>();
                lightning.StartPoint = DynamicCardPaths.Find(root.transform,info.start);lightning.EndPoint = DynamicCardPaths.Find(root.transform,info.end);lightning.Shape = info.points;
                lightning.Width = info.width;lightning.SequenceLength = info.duration;
                lightning.Sections = info.sections.Select(s => new DynamicCardLightning.Section { Start=s.start,Length=s.length,Tint=s.tint,Opacity=Curve(s.opacity),Width=Curve(s.width),BuildUp=Curve(s.buildUp),Noise=Curve(s.noise),Speed=Curve(s.speed) }).ToArray();
            }
        }

        private static AnimationCurve Curve(CurveKey[] keys)
        { return new AnimationCurve(keys.Select(k => new Keyframe(k.time,k.value,k.inSlope,k.outSlope)).ToArray()); }

        private static void RestoreQueue(Material material, MaterialInfo info)
        {
            if(info.hasState)material.renderQueue=info.queue;
            if(!string.IsNullOrEmpty(info.renderType))material.SetOverrideTag("RenderType",info.renderType);
        }

        private static void ConvertMaterial(Material material, MaterialInfo info, Shader shader)
        {
            // Source scene 10720101 has a disabled authoring ref_plane with a null shader.
            // Preserve Unity's null-shader behavior; it is not a missing gameplay shader.
            // Legacy 20154001 also carries an unreferenced Bottle material with a source null shader.
            if(info.shader=="Hidden/InternalErrorShader" && (info.originalName=="henselt_ref_plane" || info.asset.EndsWith("/Legacy2017/Shared/20154001_447704_Bottle.mat", StringComparison.Ordinal)))
            { material.shader=Shader.Find("Hidden/InternalErrorShader");RestoreQueue(material,info);EditorUtility.SetDirty(material);return; }
            if (info.asset.Contains("/Latest/"))
            {
                var latest=Shader.Find(string.IsNullOrEmpty(info.portableShader)?"DynamicCards/Latest/"+System.Text.RegularExpressions.Regex.Replace(info.shader??"","[^A-Za-z0-9_-]","_"):info.portableShader);
                if(latest==null)throw new InvalidOperationException("Missing converted latest shader: "+info.shader);
                material.shader=latest;RestoreQueue(material,info);EditorUtility.SetDirty(material);return;
            }
            if (info.asset.Contains("/Legacy2017/"))
            {
                var legacy = Shader.Find("DynamicCards/Legacy/" + System.Text.RegularExpressions.Regex.Replace(info.shader ?? "", "[^A-Za-z0-9_-]", "_"));
                if (legacy == null) throw new InvalidOperationException("Missing converted legacy shader: " + info.shader);
                material.shader = legacy; RestoreQueue(material,info);EditorUtility.SetDirty(material); return;
            }
            string nativeName="DynamicCards/Native/"+System.Text.RegularExpressions.Regex.Replace(info.shader??"","[^A-Za-z0-9_-]","_");
            var native=Shader.Find(nativeName);
            if(native!=null)
            {
                material.shader=native;
                RestoreQueue(material,info);
                EditorUtility.SetDirty(material);return;
            }
            var serialized = new SerializedObject(material);
            var textures = serialized.FindProperty("m_SavedProperties.m_TexEnvs");
            var savedTextures = new Dictionary<string, Texture>();
            var savedTransforms = new Dictionary<string, Vector4>();
            for (int i = 0; i < textures.arraySize; i++)
            {
                var pair = textures.GetArrayElementAtIndex(i);
                var name = pair.FindPropertyRelative("first").stringValue;
                var value = pair.FindPropertyRelative("second");
                savedTextures[name] = value.FindPropertyRelative("m_Texture").objectReferenceValue as Texture;
                var scale = value.FindPropertyRelative("m_Scale").vector2Value;
                var offset = value.FindPropertyRelative("m_Offset").vector2Value;
                savedTransforms[name] = new Vector4(scale.x, scale.y, offset.x, offset.y);
            }
            string family = (info.shader ?? "").ToLowerInvariant();
            var savedFloats = new Dictionary<string,float>();
            var floats = serialized.FindProperty("m_SavedProperties.m_Floats");
            for(int i=0;i<floats.arraySize;i++)
            { var pair=floats.GetArrayElementAtIndex(i);savedFloats[pair.FindPropertyRelative("first").stringValue]=pair.FindPropertyRelative("second").floatValue; }
            var savedColors = new Dictionary<string,Color>();
            var colors = serialized.FindProperty("m_SavedProperties.m_Colors");
            for(int i=0;i<colors.arraySize;i++)
            { var pair=colors.GetArrayElementAtIndex(i);savedColors[pair.FindPropertyRelative("first").stringValue]=pair.FindPropertyRelative("second").colorValue; }
            bool art = family.Contains("cardcore") || family.Contains("imagelayer") || family.Contains("matcap") || family.Contains("flowingflowmap") || family.Contains("shadowprojection");
            material.shader = shader;
            Texture main;
            savedTextures.TryGetValue("_MainTex", out main);
            string mainName = "_MainTex";
            if (main == null)
                foreach (var pair in savedTextures)
                    if (pair.Value != null && !pair.Key.ToLowerInvariant().Contains("noise") && !pair.Key.ToLowerInvariant().Contains("matcap"))
                    { main = pair.Value; mainName = pair.Key; break; }
            if (main != null)
            {
                material.SetTexture("_MainTex", main);
                var st = savedTransforms[mainName];
                material.SetTextureScale("_MainTex", new Vector2(st.x, st.y));
                material.SetTextureOffset("_MainTex", new Vector2(st.z, st.w));
            }
            // Additive glow shaders often name their image _node_9418 instead of _MainTex.
            bool additive = family.Contains("add") || family.Contains("glow") || family.Contains("shaft");
            material.SetFloat("_SrcBlend", 5); material.SetFloat("_DstBlend", additive ? 1 : 10);
            material.SetFloat("_Cull", art ? 2 : 0); material.SetFloat("_ZWrite", art ? 1 : 0);
            material.SetFloat("_Cutoff", art ? .35f : .001f);
            material.SetFloat("_Brightness", 1);
            material.SetFloat("_VertexColorStrength", art ? 0 : 1);
            // Card shaders use _Color as a lighting/specular control, not an albedo multiplier.
            if (art) { material.SetColor("_Color", Color.white); material.SetColor("_TintColor", Color.white); }
            material.renderQueue = art ? 2450 : 3000;
            if(info.hasState)
            {
                material.SetFloat("_SrcBlend",info.srcBlend);material.SetFloat("_DstBlend",info.dstBlend);
                material.SetFloat("_ZWrite",info.zWrite);material.SetFloat("_Cull",info.cull);material.renderQueue=info.queue;
                material.SetFloat("_Cutoff",info.zWrite>0 ? .35f : .001f);
            }
            float scalar;
            material.SetFloat("_MaxAlpha",savedFloats.TryGetValue("_maxParticleAlpha",out scalar)?scalar:1);
            material.SetFloat("_AlphaPower",savedFloats.TryGetValue("_AlphaExponent",out scalar)?scalar:1);
            material.SetFloat("_AlphaMultiplier",savedFloats.TryGetValue("_AlphaMultiplier",out scalar)?scalar:1);
            material.SetFloat("_AlphaFromLuma",0);material.SetFloat("_TintOnly",0);
            if(family.Contains("unicorn_nightmare_cloud"))
            {
                Color tint;if(savedColors.TryGetValue("_node_4056",out tint))material.SetColor("_TintColor",tint);
                material.SetFloat("_AlphaFromLuma",1);material.SetFloat("_TintOnly",1);
                if(savedFloats.TryGetValue("_opacity_power",out scalar))material.SetFloat("_AlphaPower",scalar);
                if(savedFloats.TryGetValue("_alpcha",out scalar))material.SetFloat("_AlphaMultiplier",scalar);
            }
            EditorUtility.SetDirty(material);
        }
    }
}
