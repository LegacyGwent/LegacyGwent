using System;
using UnityEngine;

namespace Assets.Script.DynamicCards
{
    [DefaultExecutionOrder(-50)]
    public sealed class DynamicCardLightning : MonoBehaviour
    {
        [Serializable] public class Section
        {
            public float Start, Length;
            public Color Tint = Color.white;
            public AnimationCurve Opacity, Width, BuildUp, Noise, Speed;
        }
        public Transform StartPoint, EndPoint;
        public Vector3[] Shape;
        public float Width = 1, SequenceLength = 5;
        public Section[] Sections;
        private LineRenderer line;
        private Vector3[] positions;
        private float age;
        private void Awake()
        { line = GetComponent<LineRenderer>();positions = new Vector3[Shape.Length];line.positionCount = Shape.Length; }
        private void LateUpdate()
        {
            if (StartPoint == null || EndPoint == null || Shape.Length < 2 || Sections.Length == 0) return;
            age += Time.unscaledDeltaTime;
            var section = Sections[Mathf.FloorToInt(age / Mathf.Max(.01f,SequenceLength)) % Sections.Length];
            float progress = Mathf.Clamp01((Mathf.Repeat(age,SequenceLength)-section.Start)/Mathf.Max(.01f,section.Length));
            var direction = EndPoint.position-StartPoint.position;
            var original = Shape[Shape.Length-1]-Shape[0];
            var rotation = Quaternion.FromToRotation(original,direction);
            float scale = direction.magnitude/Mathf.Max(.0001f,original.magnitude);
            float grow = Mathf.Clamp01(section.BuildUp.Evaluate(progress));
            for (int i = 0; i < positions.Length; i++)
            {
                float u = i/(float)(positions.Length-1);
                var offset = rotation*(Shape[i]-Shape[0])*scale;
                float wave = Mathf.Sin(age*section.Speed.Evaluate(progress)+i*2.17f)*section.Noise.Evaluate(progress)*Mathf.Sin(u*Mathf.PI)*.08f;
                positions[i] = StartPoint.position + offset*grow + rotation*Vector3.up*wave;
            }
            var tint = section.Tint;tint.a *= Mathf.Clamp01(section.Opacity.Evaluate(progress));
            line.startColor = line.endColor = tint;line.widthMultiplier = Width*section.Width.Evaluate(progress);
            line.SetPositions(positions);
        }
    }
}
