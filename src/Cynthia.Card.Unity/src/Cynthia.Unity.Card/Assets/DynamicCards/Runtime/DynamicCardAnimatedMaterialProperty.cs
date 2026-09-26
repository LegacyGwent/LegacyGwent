using System.Collections.Generic;
using UnityEngine;
namespace Assets.Script.DynamicCards
{
    // Unity curves target these original field names. Renderer references are prefab-local.
    [DefaultExecutionOrder(-10)]
    public sealed class DynamicCardAnimatedMaterialProperty : MonoBehaviour
    {
        public int Type,Slot;
        public string Name;
        public Color m_Color;
        public float m_Float;
        public Vector4 m_Vector;
        public Vector2 m_Offset,m_Scale=Vector2.one;
        public Renderer[] Targets;
        private readonly Dictionary<Renderer,DynamicCardMaterialState> states=new Dictionary<Renderer,DynamicCardMaterialState>();
        private void LateUpdate()
        {
            ApplyNow();
        }

        public void ApplyNow()
        {
            if(Targets==null)return;
            foreach(var target in Targets)
            {
                if(target==null || Slot<0 || Slot>=target.sharedMaterials.Length)continue;
                DynamicCardMaterialState state;
                if(!states.TryGetValue(target,out state)){state=target.GetComponent<DynamicCardMaterialState>();if(state==null)state=target.gameObject.AddComponent<DynamicCardMaterialState>();states.Add(target,state);}
                state.Apply(this,target);
            }
        }
    }
    public sealed class DynamicCardMaterialState : MonoBehaviour
    {
        private readonly Dictionary<string,Vector4> textureValues=new Dictionary<string,Vector4>();
        private MaterialPropertyBlock block;
        public void Apply(DynamicCardAnimatedMaterialProperty value,Renderer target)
        {
            if(block==null)block=new MaterialPropertyBlock();
            target.GetPropertyBlock(block,value.Slot);
            if(value.Type==1)block.SetColor(value.Name,value.m_Color);
            else if(value.Type==2)block.SetFloat(value.Name,value.m_Float);
            else if(value.Type==5)block.SetVector(value.Name,value.m_Vector);
            else
            {
                string key=value.Slot+":"+value.Name;Vector4 st;
                if(!textureValues.TryGetValue(key,out st)){var material=target.sharedMaterials[value.Slot];if(material==null)return;var scale=material.GetTextureScale(value.Name);var offset=material.GetTextureOffset(value.Name);st=new Vector4(scale.x,scale.y,offset.x,offset.y);}
                if(value.Type==3){st.z=value.m_Offset.x;st.w=value.m_Offset.y;}
                if(value.Type==4){st.x=value.m_Scale.x;st.y=value.m_Scale.y;}
                textureValues[key]=st;block.SetVector(value.Name+"_ST",st);
            }
            target.SetPropertyBlock(block,value.Slot);
        }
    }
}
