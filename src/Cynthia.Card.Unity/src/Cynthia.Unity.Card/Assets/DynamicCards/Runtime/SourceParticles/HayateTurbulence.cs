// Recovered from the supplied Unity 2017 card particle implementation.
// Namespace isolated; editor-time simulation is intentionally omitted.
using Assets.Script.DynamicCards.SourceParticles.Simplex;
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public static class HayateTurbulence
{
	private static int positionRemapedX;

	private static int positionRemapedY;

	private static int positionRemapedZ;

	private static int indexTurb;

	private static float turbulenceValue;

	public static Vector3 GetTurbulence(Vector3 _position, Hayate hayate)
	{
		float x = 0f;
		float y = 0f;
		float z = 0f;
		HayateEnums.CalculationMethod useCalculationMethodX = hayate.UseCalculationMethodX;
		HayateEnums.CalculationMethod useCalculationMethodY = hayate.UseCalculationMethodY;
		HayateEnums.CalculationMethod useCalculationMethodY2 = hayate.UseCalculationMethodY;
		switch (useCalculationMethodX)
		{
		case HayateEnums.CalculationMethod.sine:
			x = (Mathf.Sin(_position.z / hayate.Frequency.x - hayate.Offset.x) * hayate.Amplitude.x + hayate.GlobalForce.x) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.cosine:
			x = (Mathf.Cos(_position.z / hayate.Frequency.x - hayate.Offset.x) * hayate.Amplitude.x + hayate.GlobalForce.x) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.animationCurve:
			x = (hayate.turbulenceCurveX.Evaluate(_position.y / hayate.Frequency.x - hayate.Offset.x) * hayate.Amplitude.x + hayate.GlobalForce.x) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.perlin:
			x = ((Mathf.PerlinNoise(_position.z / hayate.Frequency.z - hayate.Offset.z, _position.y / hayate.Frequency.y - hayate.Offset.y) * 2f - 1f) * hayate.Amplitude.x + hayate.GlobalForce.x) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.simplex:
			x = (Noise.Generate(_position.z / hayate.Frequency.z - hayate.Offset.z, _position.y / hayate.Frequency.y - hayate.Offset.y) * hayate.Amplitude.x + hayate.GlobalForce.x) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.precalculatedTexture:
		{
			Color32[] turbulence = hayate.turbulence;
			if (turbulence != null && turbulence.Length != 0)
			{
				x = (CalculateTextureNoiseOnXAxis(_position.z / hayate.Frequency.z * 100f - hayate.Offset.z, _position.y / hayate.Frequency.y * 100f - hayate.Offset.y, hayate) + hayate.GlobalForce.x) * hayate.deltaTime;
			}
			break;
		}
		}
		switch (useCalculationMethodY)
		{
		case HayateEnums.CalculationMethod.sine:
			y = (Mathf.Sin(_position.z / hayate.Frequency.y - hayate.Offset.y) * hayate.Amplitude.y + hayate.GlobalForce.y) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.cosine:
			y = (Mathf.Cos(_position.z / hayate.Frequency.y - hayate.Offset.y) * hayate.Amplitude.y + hayate.GlobalForce.y) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.animationCurve:
			y = (hayate.turbulenceCurveY.Evaluate(_position.x / hayate.Frequency.y - hayate.Offset.y) * hayate.Amplitude.y + hayate.GlobalForce.y) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.perlin:
			y = ((Mathf.PerlinNoise(_position.x / hayate.Frequency.x - hayate.Offset.x, _position.z / hayate.Frequency.z - hayate.Offset.z) * 2f - 1f) * hayate.Amplitude.y + hayate.GlobalForce.y) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.simplex:
			y = (Noise.Generate(_position.x / hayate.Frequency.x - hayate.Offset.x, _position.z / hayate.Frequency.z - hayate.Offset.z) * hayate.Amplitude.y + hayate.GlobalForce.y) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.precalculatedTexture:
			if (hayate.turbulence != null && hayate.turbulence.Length != 0)
			{
				y = (CalculateTextureNoiseOnYAxis(_position.x / hayate.Frequency.x * 100f - hayate.Offset.x, _position.z / hayate.Frequency.z * 100f - hayate.Offset.z, hayate) + hayate.GlobalForce.y) * hayate.deltaTime;
			}
			break;
		}
		switch (useCalculationMethodY2)
		{
		case HayateEnums.CalculationMethod.sine:
			z = (Mathf.Sin(_position.x / hayate.Frequency.z - hayate.Offset.z) * hayate.Amplitude.z + hayate.GlobalForce.z) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.cosine:
			z = (Mathf.Cos(_position.x / hayate.Frequency.z - hayate.Offset.z) * hayate.Amplitude.z + hayate.GlobalForce.z) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.animationCurve:
			z = (hayate.turbulenceCurveZ.Evaluate(_position.y / hayate.Frequency.z - hayate.Offset.z) * hayate.Amplitude.z + hayate.GlobalForce.z) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.perlin:
			z = ((Mathf.PerlinNoise(_position.y / hayate.Frequency.y - hayate.Offset.y, _position.x / hayate.Frequency.x - hayate.Offset.x) * 2f - 1f) * hayate.Amplitude.z + hayate.GlobalForce.z) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.simplex:
			z = (Noise.Generate(_position.y / hayate.Frequency.y - hayate.Offset.y, _position.x / hayate.Frequency.x - hayate.Offset.x) * hayate.Amplitude.z + hayate.GlobalForce.z) * hayate.deltaTime;
			break;
		case HayateEnums.CalculationMethod.precalculatedTexture:
			if (hayate.turbulence != null && hayate.turbulence.Length != 0)
			{
				z = (CalculateTextureNoiseOnZAxis(_position.y / hayate.Frequency.y * 100f - hayate.Offset.y, _position.x / hayate.Frequency.x * 100f - hayate.Offset.x, hayate) + hayate.GlobalForce.z) * hayate.deltaTime;
			}
			break;
		}
		return new Vector3(x, y, z);
	}

	private static float CalculateTextureNoiseOnXAxis(float _positionZ, float _positionY, Hayate hayate)
	{
		positionRemapedX = (int)(_positionZ % (float)hayate.Turbulence.width);
		positionRemapedY = (int)(_positionY % (float)hayate.Turbulence.height);
		indexTurb = positionRemapedY * hayate.Turbulence.width + positionRemapedX;
		indexTurb = Mathf.Abs(indexTurb);
		if (hayate.useAlphaMask && hayate.turbulence[indexTurb].a == 0)
		{
			hayate.removeCurrentParticle = true;
		}
		turbulenceValue = ((float)(int)hayate.turbulence[indexTurb].r / 256f * 2f - 1f) * hayate.Amplitude.x * hayate.deltaTime;
		return turbulenceValue;
	}

	private static float CalculateTextureNoiseOnYAxis(float _positionX, float _positionZ, Hayate hayate)
	{
		positionRemapedX = (int)(_positionX % (float)hayate.Turbulence.width);
		positionRemapedZ = (int)(_positionZ % (float)hayate.Turbulence.height);
		indexTurb = positionRemapedX * hayate.Turbulence.width + positionRemapedZ;
		indexTurb = Mathf.Abs(indexTurb);
		if (hayate.useAlphaMask && hayate.turbulence[indexTurb].a == 0)
		{
			hayate.removeCurrentParticle = true;
		}
		turbulenceValue = ((float)(int)hayate.turbulence[indexTurb].g / 256f * 2f - 1f) * hayate.Amplitude.y * hayate.deltaTime;
		return turbulenceValue;
	}

	private static float CalculateTextureNoiseOnZAxis(float _positionY, float _positionX, Hayate hayate)
	{
		positionRemapedY = (int)(_positionY % (float)hayate.Turbulence.width);
		positionRemapedX = (int)(_positionX % (float)hayate.Turbulence.height);
		indexTurb = positionRemapedX * hayate.Turbulence.width + positionRemapedY;
		indexTurb = Mathf.Abs(indexTurb);
		if (hayate.useAlphaMask && hayate.turbulence[indexTurb].a == 0)
		{
			hayate.removeCurrentParticle = true;
		}
		turbulenceValue = ((float)(int)hayate.turbulence[indexTurb].b / 256f * 2f - 1f) * hayate.Amplitude.z * hayate.deltaTime;
		return turbulenceValue;
	}
}

}
