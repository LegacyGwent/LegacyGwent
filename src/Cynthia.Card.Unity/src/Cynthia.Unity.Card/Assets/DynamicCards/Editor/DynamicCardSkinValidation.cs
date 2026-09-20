using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    public static class DynamicCardSkinValidation
    {
        public static void Validate(DynamicCardEntry[] cards)
        {
            foreach(var card in cards)
            {
                // Optimized source Avatars can serialize no bone references. A portable
                // prefab needs an explicit rig; particles alone can hide that omission.
                if(!File.ReadAllText(card.prefab).Contains("m_Bones: []"))continue;
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(card.prefab);
                if(prefab==null)throw new BuildFailedException("Missing dynamic card prefab: "+card.prefab);
                foreach(var skin in prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                    if(skin.sharedMesh!=null && skin.sharedMesh.bindposes.Length>0 && skin.bones.Length==0)
                        throw new BuildFailedException(card.id+": skinned actor "+skin.name+" has bind poses but no bone bindings. Restore the source Avatar before building.");
            }
        }
    }
}
