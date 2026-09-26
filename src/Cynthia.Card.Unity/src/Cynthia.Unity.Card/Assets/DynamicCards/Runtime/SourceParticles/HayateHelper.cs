// Recovered from the supplied Unity 2017 card particle implementation.
// Namespace isolated; editor-time simulation is intentionally omitted.
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public static class HayateHelper
{
	public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = Vector3.Distance(p1, p2);
		float num2 = Vector3.Distance(p2, p3);
		return num * num2 / 2f;
	}

	public static ParticleSystem.Particle[] CreateParticlesDynamically(Hayate hayate, Vector3 _Position, float _Width, float _Height, float _Depth, float _TargetParticleSize, float _UiScale, HayateEnums.BuildOrder _buildOrder)
	{
		int num = Mathf.CeilToInt(_Width / _TargetParticleSize);
		int num2 = Mathf.CeilToInt(_Height / _TargetParticleSize);
		int num3 = Mathf.CeilToInt(_Depth / _TargetParticleSize);
		int num4 = num * num2 * num3;
		ParticleSystem component = hayate.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule main = component.main;
		main.startSize = _TargetParticleSize * _UiScale;
		ParticleSystem.EmissionModule emission = component.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(0f);
		hayate.GetComponent<ParticleSystem>().Emit(num4);
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[num4];
		hayate.GetComponent<ParticleSystem>().GetParticles(array);
		int num5 = 0;
		int num6 = 1;
		switch (_buildOrder)
		{
		case HayateEnums.BuildOrder.TopBottom:
		{
			for (int num36 = 0; num36 < num3; num36++)
			{
				int num37 = 1;
				for (int num38 = num2; num38 > 0; num38--)
				{
					int num39 = 1;
					for (int num40 = 0; num40 < num; num40++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num40 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num38 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num36 * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * ((float)num / 2f * (float)num37 * (float)num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num39++;
					}
					num37++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.TopRightBottomLeft:
		{
			for (int num16 = 0; num16 < num3; num16++)
			{
				int num17 = 1;
				for (int num18 = num2; num18 > 0; num18--)
				{
					int num19 = 1;
					for (int num20 = num; num20 > 0; num20--)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num20 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num18 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num16 * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num19 * num17 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num19++;
					}
					num17++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.RightLeft:
		{
			for (int num26 = 0; num26 < num3; num26++)
			{
				int num27 = 1;
				for (int num28 = num; num28 > 0; num28--)
				{
					int num29 = 1;
					for (int num30 = 0; num30 < num2; num30++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num28 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num30 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num26 * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num27 * num29 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num27++;
					}
					num29++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.BottomRightTopLeft:
		{
			for (int k = 0; k < num3; k++)
			{
				int num10 = 1;
				for (int l = 0; l < num2; l++)
				{
					int num11 = 1;
					for (int num12 = num; num12 > 0; num12--)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num12 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)l * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)k * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num11 * num10 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num11++;
					}
					num10++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.BottomTop:
		{
			for (int num31 = 0; num31 < num3; num31++)
			{
				int num32 = 1;
				for (int num33 = 0; num33 < num2; num33++)
				{
					int num34 = 1;
					for (int num35 = 0; num35 < num; num35++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num35 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num33 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num31 * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * ((float)num / 2f * (float)num32 * (float)num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num34++;
					}
					num32++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.BottomLeftTopRight:
		{
			for (int num21 = 0; num21 < num3; num21++)
			{
				int num22 = 1;
				for (int num23 = 0; num23 < num2; num23++)
				{
					int num24 = 1;
					for (int num25 = 0; num25 < num; num25++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num25 * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num23 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num21 * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num24 * num22 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num24++;
					}
					num22++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.LeftRight:
		{
			for (int m = 0; m < num3; m++)
			{
				int num13 = 1;
				for (int n = 0; n < num; n++)
				{
					int num14 = 1;
					for (int num15 = 0; num15 < num2; num15++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)n * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)num15 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)m * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num13 * num14 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num13++;
					}
					num14++;
				}
				num6++;
			}
			break;
		}
		case HayateEnums.BuildOrder.TopLeftBottomRight:
		{
			for (int i = 0; i < num3; i++)
			{
				int num7 = 1;
				for (int num8 = num2; num8 > 0; num8--)
				{
					int num9 = 1;
					for (int j = 0; j < num; j++)
					{
						array[num5].position = new Vector3(_Position.x - _Width * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)j * _TargetParticleSize * _UiScale, _Position.y - _Height * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f - _TargetParticleSize * _UiScale + (float)num8 * _TargetParticleSize * _UiScale, _Position.z - _Depth * _UiScale / 2f + _TargetParticleSize * _UiScale / 2f + (float)i * _TargetParticleSize * _UiScale);
						array[num5].remainingLifetime += array[num5].startLifetime / (float)num4 * (float)(num9 * num7 * num6);
						array[num5].startLifetime = array[num5].remainingLifetime;
						num5++;
						num9++;
					}
					num7++;
				}
				num6++;
			}
			break;
		}
		}
		return array;
	}
}

}
