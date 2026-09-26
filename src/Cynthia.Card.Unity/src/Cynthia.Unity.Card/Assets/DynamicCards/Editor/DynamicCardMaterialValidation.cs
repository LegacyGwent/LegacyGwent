using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    public static class DynamicCardMaterialValidation
    {
        [Serializable] private class Conversion
        {
            public string atlas;
            public SourceMaterial[] materials;
            public Assignment[] textureAssignments;
        }
        [Serializable] private class SourceMaterial { public string asset, originalName; }
        [Serializable] private class Assignment { public string material, materialAsset, texture; public string[] properties; }

        // A material name is not unique: source scenes often contain several instances.
        // Validate every live instance against the recorded source texture assignments.
        public static void Validate(DynamicCardEntry[] cards)
        {
            var failures = new List<string>();
            foreach (var card in cards)
            {
                if (card.nonRenderingPaths != null && card.nonRenderingPaths.Length > 0)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(card.prefab);
                    var unassigned = new HashSet<string>(prefab.GetComponentsInChildren<Renderer>(true)
                        .Where(renderer => renderer.sharedMaterials.All(material => material == null))
                        .Select(renderer => DynamicCardPaths.RelativePath(renderer.transform, prefab.transform)));
                    foreach (var path in card.nonRenderingPaths)
                    {
                        if (!unassigned.Contains(path))
                            failures.Add(card.id + ": unassigned-surface contract no longer matches " + path);
                    }
                }
                string record = Path.Combine(Path.GetDirectoryName(card.prefab), "conversion.json");
                if (!File.Exists(record)) continue;
                var source = JsonUtility.FromJson<Conversion>(File.ReadAllText(record));
                if (source == null || string.IsNullOrEmpty(source.atlas) || source.materials == null || source.textureAssignments == null) continue;
                HashSet<string> dependencies = null;
                foreach (var assignment in source.textureAssignments)
                foreach (var item in source.materials.Where(m => m.originalName == assignment.material &&
                    (string.IsNullOrEmpty(assignment.materialAsset) || assignment.materialAsset == m.asset) && File.Exists(m.asset)))
                {
                    // Most bindings can be checked without loading their large textures.
                    string yaml = File.ReadAllText(item.asset);
                    string expectedPath = string.IsNullOrEmpty(assignment.texture) ? source.atlas : assignment.texture;
                    string expectedGuid = AssetDatabase.AssetPathToGUID(expectedPath);
                    var empty = (assignment.properties ?? new string[0]).Where(property => {
                        var binding = Regex.Match(yaml, "- " + Regex.Escape(property) + @":\s*\n\s*m_Texture: (\{[^\n]+\})");
                        return string.IsNullOrEmpty(expectedGuid) || !binding.Success || !binding.Groups[1].Value.Contains("guid: " + expectedGuid + ",");
                    }).ToArray();
                    if (empty.Length == 0) continue;
                    if (dependencies == null) dependencies = new HashSet<string>(AssetDatabase.GetDependencies(card.prefab, true));
                    if (!dependencies.Contains(item.asset)) continue;
                    var material = AssetDatabase.LoadAssetAtPath<Material>(item.asset);
                    if (material == null) { failures.Add(card.id + ": missing material " + item.asset); continue; }
                    foreach (string property in empty)
                        if (material.HasProperty(property) && (material.GetTexture(property) == null || AssetDatabase.GetAssetPath(material.GetTexture(property)) != expectedPath))
                            failures.Add(card.id + ": incorrect source texture in " + item.asset + " " + property + "; expected " + expectedPath);
                }
            }
            if (failures.Count > 0)
                throw new BuildFailedException("Dynamic card source material bindings are incomplete:\n" + string.Join("\n", failures));
        }
    }
}
