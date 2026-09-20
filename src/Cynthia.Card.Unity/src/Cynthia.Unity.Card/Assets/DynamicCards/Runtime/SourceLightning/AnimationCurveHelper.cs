// Original lightning behavior, with isolated namespace and explicit source shader.
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceLightning
{
public static class AnimationCurveHelper
{
	public static Keyframe K1Zero;

	public static Keyframe K2Zero;

	public static Keyframe K1Linear;

	public static Keyframe K2Linear;

	public static Keyframe K1Opacity;

	public static Keyframe K2Opacity;

	public static Keyframe K1BuildUp;

	public static Keyframe K2BuildUp;

	public static Keyframe K1Branching;

	public static Keyframe K2Branching;

	public static Keyframe K3Branching;

	public static Keyframe K1Turbulence;

	public static Keyframe K2Turbulence;

	public static Keyframe K3Turbulence;

	static AnimationCurveHelper()
	{
		K1Zero = default(Keyframe);
		K2Zero = default(Keyframe);
		K1Linear = default(Keyframe);
		K2Linear = default(Keyframe);
		K1Opacity = default(Keyframe);
		K2Opacity = default(Keyframe);
		K1BuildUp = default(Keyframe);
		K2BuildUp = default(Keyframe);
		K1Branching = default(Keyframe);
		K2Branching = default(Keyframe);
		K3Branching = default(Keyframe);
		K1Turbulence = default(Keyframe);
		K2Turbulence = default(Keyframe);
		K3Turbulence = default(Keyframe);
		K1Zero.time = 0f;
		K1Zero.value = 0f;
		K1Zero.inTangent = 0f;
		K1Zero.outTangent = 0f;
		K2Zero.time = 1f;
		K2Zero.value = 0f;
		K2Zero.inTangent = 0f;
		K2Zero.outTangent = 0f;
		K1Linear.time = 0f;
		K1Linear.value = 1f;
		K1Linear.inTangent = 0f;
		K1Linear.outTangent = 0f;
		K2Linear.time = 1f;
		K2Linear.value = 1f;
		K2Linear.inTangent = 0f;
		K2Linear.outTangent = 0f;
		K1Opacity.time = 0f;
		K1Opacity.value = 1f;
		K1Opacity.inTangent = 0f;
		K1Opacity.outTangent = 0f;
		K2Opacity.time = 1f;
		K2Opacity.value = 0f;
		K2Opacity.inTangent = -2.6075f;
		K2Opacity.outTangent = -2.6075f;
		K1BuildUp.time = 0f;
		K1BuildUp.value = -1f;
		K1BuildUp.inTangent = 1f;
		K1BuildUp.outTangent = 1f;
		K2BuildUp.time = 1f;
		K2BuildUp.value = 0f;
		K2BuildUp.inTangent = 1f;
		K2BuildUp.outTangent = 1f;
		K1Branching.time = 0f;
		K1Branching.value = 1f;
		K1Branching.inTangent = 0f;
		K1Branching.outTangent = 0f;
		K2Branching.time = 0.5f;
		K2Branching.value = 0f;
		K2Branching.inTangent = 0f;
		K2Branching.outTangent = 0f;
		K3Branching.time = 1f;
		K3Branching.value = 1f;
		K3Branching.inTangent = 0f;
		K3Branching.outTangent = 0f;
		K1Turbulence.time = 0f;
		K1Turbulence.value = 0f;
		K1Turbulence.inTangent = 0f;
		K1Turbulence.outTangent = 0f;
		K2Turbulence.time = 0.5f;
		K2Turbulence.value = 1f;
		K2Turbulence.inTangent = 0f;
		K2Turbulence.outTangent = 0f;
		K3Turbulence.time = 1f;
		K3Turbulence.value = 0f;
		K3Turbulence.inTangent = 0f;
		K3Turbulence.outTangent = 0f;
	}

	public static AnimationCurve SetUpZeroCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1Zero);
		animationCurve.AddKey(K2Zero);
		return animationCurve;
	}

	public static AnimationCurve SetUpLinearCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1Linear);
		animationCurve.AddKey(K2Linear);
		return animationCurve;
	}

	public static AnimationCurve SetUpOpacityCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1Opacity);
		animationCurve.AddKey(K2Opacity);
		return animationCurve;
	}

	public static AnimationCurve SetUpBuildUpCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1BuildUp);
		animationCurve.AddKey(K2BuildUp);
		return animationCurve;
	}

	public static AnimationCurve SetUpBranchingCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1Branching);
		animationCurve.AddKey(K2Branching);
		animationCurve.AddKey(K3Branching);
		return animationCurve;
	}

	public static AnimationCurve SetUpTurbulenceCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.AddKey(K1Turbulence);
		animationCurve.AddKey(K2Turbulence);
		animationCurve.AddKey(K3Turbulence);
		return animationCurve;
	}
}

}
