using UnityEngine;

namespace Assets.Script.DynamicCards
{
    // Original ForwardBloomSettings values and rotation response, independent of source game services.
    public sealed class DynamicCardBloom : DynamicCardPostEffect
    {
        public float blurSpread = .001f, contrast = 2, blurIntensity = 1, AngleIntensity = 1;
        public Transform ObjCtrl;
        public AnimationCurve Distribution = new AnimationCurve();
        public float XRotationStart, XRotationEnd, YRotationStart, YRotationEnd;

        protected override void Configure(Material target)
        {
            if (ObjCtrl != null)
            {
                var angle = ObjCtrl.localEulerAngles;
                for (int i = 0; i < 3; i++) if (angle[i] > 180) angle[i] -= 360;
                var high = new Vector3(YRotationStart, XRotationEnd, 0);
                var low = new Vector3(YRotationEnd, XRotationStart, 0);
                AngleIntensity = Distribution.Evaluate(Vector3.Distance(angle, high) / Vector3.Distance(low, high));
            }
            target.SetFloat("_Spread", blurSpread * AngleIntensity);
            target.SetFloat("_Intensity", blurIntensity * AngleIntensity);
            target.SetFloat("_Contrast", contrast);
        }
    }
}
