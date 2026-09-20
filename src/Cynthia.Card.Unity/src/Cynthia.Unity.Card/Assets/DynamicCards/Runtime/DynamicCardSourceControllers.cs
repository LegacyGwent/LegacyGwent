using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    [Serializable] public class DynamicCardCurveKey { public float time,value,inSlope,outSlope; }
    [Serializable] public class DynamicCardViewMotion { public string path,property; public int materialIndex,kind,axis; public Vector2 xRange,yRange; public float multiplier; }
    [Serializable] public class DynamicCardCameraParent { public string path; public float delay; }
    [Serializable] public class DynamicCardJiggle { public string[] bones; public float speed,damping,maxDistance,maxVelocity,delay; public DynamicCardCurveKey[] falloff; }
    [Serializable] public class DynamicCardWiggle { public string path; public float delay,length; public bool loop; public DynamicCardCurveKey[] frequency,strength,speed; }
    [Serializable] public class DynamicCardMaterialValue { public string path,property; public int materialIndex; public float value; }
    [Serializable] public class DynamicCardInitialTransform { public string path; public Vector3 position,rotation,scale; }

    // Source controller parameters live in the optional catalog, not in existing game assets.
    public sealed class DynamicCardSourceControllers : MonoBehaviour
    {
        private sealed class Motion { public Renderer renderer; public DynamicCardViewMotion data; public Vector2 scale; public float initial; public MaterialPropertyBlock block=new MaterialPropertyBlock(); }
        private sealed class Attachment { public Transform target; public float delay; public bool attached; }
        private sealed class Spring { public Transform bone; public Vector3 rest,position,velocity; public DynamicCardJiggle data; public float falloff; public bool started,applied; }
        private sealed class Wiggle { public Transform bone; public Quaternion rest; public DynamicCardWiggle data; public AnimationCurve frequency,strength,speed; public bool applied; }
        private readonly List<Motion> motions=new List<Motion>();
        private readonly List<Attachment> attachments=new List<Attachment>();
        private readonly List<Spring> springs=new List<Spring>();
        private readonly List<Wiggle> wiggles=new List<Wiggle>();
        private DynamicCardEntry entry;
        private Transform cameraAnchor;

        public void Initialize(DynamicCardEntry data,Transform camera)
        {
            entry=data;
            foreach(var item in data.initialTransforms ?? new DynamicCardInitialTransform[0])
            {
                var target=DynamicCardPaths.Find(transform,item.path);if(target==null)continue;
                target.localPosition=item.position;target.localRotation=Quaternion.Euler(item.rotation);target.localScale=item.scale;
            }
            foreach(var item in data.viewMotions ?? new DynamicCardViewMotion[0])
            {
                var target=DynamicCardPaths.Find(transform,item.path);var renderer=target==null?null:target.GetComponent<Renderer>();
                if(renderer==null || item.materialIndex<0 || item.materialIndex>=renderer.sharedMaterials.Length)continue;
                var material=renderer.sharedMaterials[item.materialIndex];if(material==null || !material.HasProperty(item.property))continue;
                motions.Add(new Motion { renderer=renderer,data=item,scale=item.kind==0?material.GetTextureScale(item.property):Vector2.one,initial=item.kind==1?material.GetFloat(item.property):0 });
            }
            foreach(var item in data.materialValues ?? new DynamicCardMaterialValue[0])
            {
                var target=DynamicCardPaths.Find(transform,item.path);var renderer=target==null?null:target.GetComponent<Renderer>();
                if(renderer==null || item.materialIndex>=renderer.sharedMaterials.Length)continue;
                var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block,item.materialIndex);block.SetFloat(item.property,item.value);renderer.SetPropertyBlock(block,item.materialIndex);
            }
            foreach(var group in data.jiggles ?? new DynamicCardJiggle[0])
            {
                var falloff=Curve(group.falloff);
                for(int i=0;i<group.bones.Length;i++)
                {
                    var bone=DynamicCardPaths.Find(transform,group.bones[i]);if(bone==null)continue;
                    springs.Add(new Spring { bone=bone,rest=bone.localPosition,data=group,falloff=falloff.Evaluate(group.bones.Length<=1?0:(float)i/(group.bones.Length-1)) });
                }
            }
            foreach(var item in data.wiggles ?? new DynamicCardWiggle[0])
            {
                var bone=DynamicCardPaths.Find(transform,item.path);if(bone==null)continue;
                wiggles.Add(new Wiggle { bone=bone,rest=bone.localRotation,data=item,frequency=Curve(item.frequency),strength=Curve(item.strength),speed=Curve(item.speed) });
            }
            foreach(var item in data.cameraParents ?? new DynamicCardCameraParent[0])
            {
                var target=DynamicCardPaths.Find(transform,item.path);if(target!=null)attachments.Add(new Attachment { target=target,delay=item.delay });
            }
            if(attachments.Count>0)
            {
                cameraAnchor=new GameObject("Camera-attached effects").transform;cameraAnchor.SetParent(transform,false);
                cameraAnchor.position=camera.position;cameraAnchor.rotation=camera.rotation;
            }
        }
        private void Update()
        {
            // Remove only last frame's procedural displacement before Animator evaluates again.
            foreach(var spring in springs)if(spring.applied){spring.bone.localPosition=spring.rest;spring.applied=false;}
            foreach(var wiggle in wiggles)if(wiggle.applied){wiggle.bone.localRotation=wiggle.rest;wiggle.applied=false;}
        }
        public void Tick(float time,float delta,float pitch,float yaw)
        {
            foreach(var attachment in attachments)
                if(!attachment.attached && time>=attachment.delay){attachment.target.SetParent(cameraAnchor,true);attachment.attached=true;}
            foreach(var motion in motions)
            {
                var d=motion.data;motion.renderer.GetPropertyBlock(motion.block,d.materialIndex);
                if(d.kind==0)
                {
                    float x=Mathf.InverseLerp(entry.yStart,entry.yEnd,yaw),y=Mathf.InverseLerp(entry.xStart,entry.xEnd,pitch);
                    motion.block.SetVector(d.property+"_ST",new Vector4(motion.scale.x,motion.scale.y,Mathf.Lerp(d.xRange.x,d.xRange.y,x),Mathf.Lerp(d.yRange.x,d.yRange.y,y)));
                }
                else motion.block.SetFloat(d.property,motion.initial+(d.axis==0?pitch:yaw)*d.multiplier);
                motion.renderer.SetPropertyBlock(motion.block,d.materialIndex);
            }
            float dt=Mathf.Min(delta,.05f);
            foreach(var spring in springs)
            {
                if(time<spring.data.delay)continue;
                Vector3 goal=spring.bone.position;
                if(!spring.started){spring.position=goal;spring.started=true;}
                spring.velocity+=(goal-spring.position)*spring.data.speed*spring.data.speed*dt;
                spring.velocity*=Mathf.Exp(-spring.data.damping*60*dt);
                spring.velocity=Vector3.ClampMagnitude(spring.velocity,spring.data.maxVelocity);
                spring.position+=spring.velocity*dt;
                spring.position=goal+Vector3.ClampMagnitude(spring.position-goal,spring.data.maxDistance);
                spring.bone.position=Vector3.Lerp(goal,spring.position,spring.falloff);spring.applied=true;
            }
            foreach(var wiggle in wiggles)
            {
                float age=time-wiggle.data.delay;if(age<0 || (!wiggle.data.loop && age>wiggle.data.length))continue;
                float phase=wiggle.data.loop?Mathf.Repeat(age,Mathf.Max(.001f,wiggle.data.length)):age;
                float speed=wiggle.speed.Evaluate(phase),frequency=wiggle.frequency.Evaluate(phase),strength=wiggle.strength.Evaluate(phase);
                var noise=new Vector3(Mathf.PerlinNoise(age*speed,frequency),Mathf.PerlinNoise(age*speed,frequency+13),Mathf.PerlinNoise(age*speed,frequency+27))*2-Vector3.one;
                wiggle.bone.localRotation*=Quaternion.Euler(noise*strength);wiggle.applied=true;
            }
        }
        private static AnimationCurve Curve(DynamicCardCurveKey[] values)
        {
            if(values==null || values.Length==0)return AnimationCurve.Linear(0,1,1,1);
            var keys=new Keyframe[values.Length];for(int i=0;i<keys.Length;i++)keys[i]=new Keyframe(values[i].time,values[i].value,values[i].inSlope,values[i].outSlope);
            return new AnimationCurve(keys);
        }
    }
}
