// Recovered from the supplied Unity 2017 card particle implementation.
// Namespace isolated; editor-time simulation is intentionally omitted.
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public class Hayate : MonoBehaviour
{
	public bool drawTurbulenceField;

	public Vector3 fieldSize = new Vector3(5f, 0f, 5f);

	public Vector3 stepSize = new Vector3(0.1f, 0.15f, 0.15f);

	public Color debugColor = new Color(0f, 1f, 0f, 1f);

	public float rayLength = 1f;

	public bool UseTurbulence = true;

	public Texture2D Turbulence;

	public bool removeCurrentParticle;

	public bool useAlphaMask;

	public float threshold = 0.5f;

	public AnimationCurve turbulenceCurveX;

	public AnimationCurve turbulenceCurveY;

	public AnimationCurve turbulenceCurveZ;

	public Color32[] turbulence;

	public HayateEnums.AssignTo AssignTurbulenceTo;

	public HayateEnums.TurbulenceType UseRelativeOrAbsoluteValues;

	public HayateEnums.CalculationMethod UseCalculationMethodX;

	public HayateEnums.CalculationMethod UseCalculationMethodY;

	public HayateEnums.CalculationMethod UseCalculationMethodZ;

	public Vector3 Amplitude = Vector3.one;

	public AnimationCurve AmplitudeCurveX = new AnimationCurve();

	public AnimationCurve AmplitudeCurveY = new AnimationCurve();

	public AnimationCurve AmplitudeCurveZ = new AnimationCurve();

	public bool useAmplitudeCurve;

	public Vector3 Frequency = Vector3.one;

	public AnimationCurve FrequencyCurveX = new AnimationCurve();

	public AnimationCurve FrequencyCurveY = new AnimationCurve();

	public AnimationCurve FrequencyCurveZ = new AnimationCurve();

	public bool useFrequencyCurve;

	public Vector3 GlobalForce = Vector3.zero;

	public AnimationCurve GlobalForceCurveX = new AnimationCurve();

	public AnimationCurve GlobalForceCurveY = new AnimationCurve();

	public AnimationCurve GlobalForceCurveZ = new AnimationCurve();

	public bool useGlobalForceCurve;

	public Vector3 Offset;

	public AnimationCurve OffsetCurveX = new AnimationCurve();

	public AnimationCurve OffsetCurveY = new AnimationCurve();

	public AnimationCurve OffsetCurveZ = new AnimationCurve();

	public bool useOffsetCurve;

	public Vector3 OffsetSpeed;

	public AnimationCurve OffsetSpeedCurveX = new AnimationCurve();

	public AnimationCurve OffsetSpeedCurveY = new AnimationCurve();

	public AnimationCurve OffsetSpeedCurveZ = new AnimationCurve();

	public bool useOffsetSpeedCurve;

	public bool lockOffsetToEmitterPosition;

	public bool randomizeOffsetAtStart;

	public Vector2 randomOffsetRange = new Vector2(-1000f, 1000f);

	public bool burstOnCollision;

	public int burstNum;

	public bool isDeltaIndependent;

	public float targetFps = 60f;

	public bool useTransformParticle;

	public bool detachTransformParticleAfterParticleDeath;

	public float detachedObjectDestructionTimeAfter = 1f;

	public bool transformParticleLookTowardsFlightDirection;

	public GameObject transformParticle;

	private readonly List<GameObject> transformParticles = new List<GameObject>();

	public bool useAttractor;

	public Transform followTransform;

	public Vector3 followPosition;

	public float followStrength;

	public List<GameObject> attractors = new List<GameObject>();

	public List<Vector3> attractorPositions = new List<Vector3>();

	public List<float> attractorStrength = new List<float>();

	public List<float> attractorAttenuation = new List<float>();

	public bool IsWorldSpace = true;

	public bool moveToMesh;

	public bool useSkinnedMesh;

	public GameObject meshTarget;

	public GameObject skinnedMeshTarget;

	public bool useParticleSpeedCurve;

	public float particleSpeedToMesh = 0.5f;

	public AnimationCurve particleSpeedToMeshAnimation;

	public HayateEnums.MeshFollow meshFollow;

	public bool emitFromMeshTarget;

	private Vector3[] targets;

	private Mesh resetMesh;

	public float smallestTriangle = 0.1f;

	public HayateEnums.DivisionType divisionType;

	private ParticleSystem.Particle[] particles;

	private int particleCount;

	public float deltaTime;

	private readonly List<int> index = new List<int>();

	private readonly List<int> length = new List<int>();

	private int turbulenceWidth;

	private Vector3 currentPosition;

	private float lastUpdateTime;

	public bool useSfx;

	public bool CreateOnStart = true;

	public HayateEnums.BuildOrder buildOrder = HayateEnums.BuildOrder.TopLeftBottomRight;

	public float Width = 10f;

	public float Height = 10f;

	public float Depth = 10f;

	public float TargetParticleSize = 1f;

	public float UiScale = 1f;

	public bool isAlwaysOnTop;

	public bool isTimeScaleIndependent;

	private float previousUpdate;

	private int _index;

	private int _length;

	private Vector3 desiredVelocity;

	private Vector3 _desiredVelocity;

	private int safeIndex;

	private Mesh skinnedMeshBuffer;

	private Vector3[] skinnedMeshVertices;

	public int maxParticles = 1000;

	private static Vector3 TurbulenceRelativeBase = new Vector3(10f, 10f, 10f);

	private static Vector3 TurbulenceAbsoluteBase = new Vector3(0.1f, 0.1f, 0.1f);

	private ParticleSystem m_CurrentParticleSystem;

	private void Start()
	{
		if (detachTransformParticleAfterParticleDeath && !Application.isPlaying)
		{
			Debug.Log("Detaching transform particles only work when playing the scene.");
		}
		if (UseCalculationMethodX == HayateEnums.CalculationMethod.precalculatedTexture || UseCalculationMethodY == HayateEnums.CalculationMethod.precalculatedTexture || UseCalculationMethodZ == HayateEnums.CalculationMethod.precalculatedTexture)
		{
			UpdateTexture();
		}
		if (randomizeOffsetAtStart)
		{
			float x = randomOffsetRange.x;
			float y = randomOffsetRange.y;
			Offset = new Vector3(Random.Range(x, y), Random.Range(x, y), Random.Range(x, y));
		}
		if (CreateOnStart && useSfx)
		{
			CreateParticles(base.transform.InverseTransformPoint(base.transform.position), Width, Height, Depth, TargetParticleSize, UiScale, buildOrder);
			GetComponent<ParticleSystem>().Play();
		}
		previousUpdate = Time.realtimeSinceStartup;
	}

	private void InitializeParticleSystem()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (!particleSystem)
		{
			particleSystem = base.gameObject.AddComponent<ParticleSystem>();
		}
		m_CurrentParticleSystem = particleSystem;
	}

	private void OnEnable()
	{
		if (CreateOnStart && useSfx)
		{
			CreateParticles(base.transform.InverseTransformPoint(base.transform.position), Width, Height, Depth, TargetParticleSize, UiScale, buildOrder);
			InitializeParticleSystem();
			m_CurrentParticleSystem.Play();
		}
	}

	private void Update()
	{
		if (!UseTurbulence)
		{
			return;
		}
		InitializeParticleSystem();
		EvaluateCurves(m_CurrentParticleSystem);
		if (isTimeScaleIndependent)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float t = realtimeSinceStartup - previousUpdate;
			m_CurrentParticleSystem.Simulate(t, withChildren: true, restart: false);
			previousUpdate = realtimeSinceStartup;
		}
		if (particles == null || maxParticles != particles.Length)
		{
			particles = new ParticleSystem.Particle[maxParticles];
		}
		if (isDeltaIndependent)
		{
			deltaTime = 1f / targetFps;
		}
		else
		{
			deltaTime = Time.deltaTime;
		}
		particleCount = m_CurrentParticleSystem.GetParticles(particles);
		currentPosition = base.transform.position;
		for (int i = 0; i < attractors.Count; i++)
		{
			GameObject gameObject = attractors[i];
			if (gameObject != null)
			{
				attractorPositions[i] = gameObject.transform.position;
			}
			else
			{
				attractorPositions[i] = Vector3.zero;
			}
		}
		index.Add(0);
		length.Add(particleCount);
		HayateUpdate();
		m_CurrentParticleSystem.SetParticles(particles, particleCount);
	}

	private void HayateUpdate()
	{
		if (index.Count > 0)
		{
			_index = index[0];
			_length = length[0];
			index.RemoveAt(0);
			length.RemoveAt(0);
			int num = _index + _length;
			for (int i = _index; i < num; i++)
			{
				if (AssignTurbulenceTo == HayateEnums.AssignTo.position)
				{
					if (UseRelativeOrAbsoluteValues == HayateEnums.TurbulenceType.absolute)
					{
						if (particles[i].velocity == Vector3.zero)
						{
							particles[i].velocity = particles[i].position;
						}
						particles[i].position = base.transform.position + HayateTurbulence.GetTurbulence(particles[i].velocity, this);
					}
					else
					{
						particles[i].position += HayateTurbulence.GetTurbulence(particles[i].position, this);
					}
				}
				else if (AssignTurbulenceTo == HayateEnums.AssignTo.velocity)
				{
					Vector3 vector = HayateTurbulence.GetTurbulence(particles[i].position, this);
					if (UseRelativeOrAbsoluteValues == HayateEnums.TurbulenceType.absolute)
					{
						particles[i].velocity = vector;
					}
					else
					{
						particles[i].velocity += vector;
					}
				}
				float num2 = deltaTime;
				if (useAttractor)
				{
					int count = attractors.Count;
					for (int j = 0; j < count; j++)
					{
						ParticleSystem.Particle particle = particles[i];
						Vector3 position = particle.position;
						if (IsWorldSpace)
						{
							desiredVelocity = attractorPositions[j] - position;
							Vector3 vector2 = Vector3.Normalize(desiredVelocity);
							particles[i].velocity += vector2 * attractorStrength[j] * num2 * (1f - Mathf.Clamp01(Vector3.Distance(position, attractorPositions[j]) / attractorAttenuation[j]));
						}
						else
						{
							Vector3 vector3 = base.transform.InverseTransformPoint(attractorPositions[j]);
							desiredVelocity = vector3 - particles[i].position;
							Vector3 vector4 = Vector3.Normalize(desiredVelocity);
							particles[i].velocity += vector4 * attractorStrength[j] * num2 * (1f - Mathf.Clamp01(Vector3.Distance(position, vector3) / attractorAttenuation[j]));
						}
					}
				}
				if (useAlphaMask && removeCurrentParticle && particles[i].remainingLifetime >= particles[i].startLifetime - threshold)
				{
					particles[i].remainingLifetime = 0f;
					removeCurrentParticle = false;
				}
			}
			if (useTransformParticle && (bool)transformParticle && Application.isPlaying)
			{
				List<ParticleSystem.Particle> list = new List<ParticleSystem.Particle>(particleCount);
				for (int k = 0; k < particleCount; k++)
				{
					list.Add(particles[k]);
				}
				int count2 = list.Count;
				int count3 = transformParticles.Count;
				if (count3 < count2)
				{
					int num3 = count2 - count3;
					for (int l = 0; l < num3; l++)
					{
						GameObject gameObject = Object.Instantiate(transformParticle);
						gameObject.transform.parent = base.transform;
						transformParticles.Add(gameObject);
					}
				}
				for (int m = 0; m < count2; m++)
				{
					ParticleSystem.Particle particle2 = particles[m];
					Vector3 position2 = particle2.position;
					transformParticles[m].transform.position = position2;
					if (transformParticleLookTowardsFlightDirection)
					{
						transformParticles[m].transform.LookAt(position2 + particle2.velocity);
					}
					if (list[m].remainingLifetime < 1f)
					{
						list.RemoveAt(m);
						if (detachTransformParticleAfterParticleDeath)
						{
							Object.Destroy(transformParticles[m].gameObject, detachedObjectDestructionTimeAfter);
						}
						else
						{
							Object.Destroy(transformParticles[m].gameObject);
						}
						transformParticles.RemoveAt(m);
						particleCount--;
						m--;
					}
				}
				particles = list.ToArray();
			}
			else
			{
				for (int n = 0; n < transformParticles.Count; n++)
				{
					Object.DestroyImmediate(transformParticles[n]);
					transformParticles.RemoveAt(n);
				}
			}
			if (lockOffsetToEmitterPosition)
			{
				float num4 = (float)turbulenceWidth / 2f;
				if (UseCalculationMethodX == HayateEnums.CalculationMethod.precalculatedTexture || UseCalculationMethodY == HayateEnums.CalculationMethod.precalculatedTexture || UseCalculationMethodZ == HayateEnums.CalculationMethod.precalculatedTexture)
				{
					Offset = new Vector3(currentPosition.x / Frequency.x, currentPosition.y / Frequency.y, currentPosition.z / Frequency.z) * 100f + new Vector3(num4, num4, num4);
				}
				else
				{
					Offset = new Vector3(currentPosition.x / Frequency.x, currentPosition.y / Frequency.y, currentPosition.z / Frequency.z);
				}
			}
			else if ((bool)m_CurrentParticleSystem)
			{
				Offset += OffsetSpeed * deltaTime;
			}
			if (!moveToMesh)
			{
				return;
			}
			float num5 = 0f;
			if (!useSkinnedMesh && (bool)meshTarget)
			{
				MeshFilter component = meshTarget.GetComponent<MeshFilter>();
				if ((bool)component)
				{
					targets = component.sharedMesh.vertices;
					for (int num6 = _index; num6 < _index + _length; num6++)
					{
						num5 = ((!useParticleSpeedCurve) ? particleSpeedToMesh : particleSpeedToMeshAnimation.Evaluate(particles[num6].startLifetime - particles[num6].remainingLifetime));
						if (num5 < 0f)
						{
							num5 = 0f;
						}
						safeIndex = num6 % targets.Length;
						Vector3 vector5 = meshTarget.transform.TransformPoint(targets[safeIndex]);
						if (meshFollow == HayateEnums.MeshFollow.byDistance)
						{
							particles[num6].position = Vector3.Lerp(particles[num6].position, vector5, 1f / Mathf.Pow(Vector3.Distance(vector5, particles[num6].position), 2f) * num5);
							continue;
						}
						if (meshFollow == HayateEnums.MeshFollow.byTime)
						{
							particles[num6].position = Vector3.Lerp(particles[num6].position, vector5, num5 * deltaTime);
							continue;
						}
						_desiredVelocity = vector5 - particles[num6].position;
						particles[num6].velocity += Vector3.Normalize(_desiredVelocity) * num5 * deltaTime;
					}
				}
				else
				{
					Debug.LogWarning("No MeshFilter attached to this GameObject!");
					meshTarget = null;
				}
			}
			if (!useSkinnedMesh || !skinnedMeshTarget)
			{
				return;
			}
			SkinnedMeshRenderer component2 = skinnedMeshTarget.GetComponent<SkinnedMeshRenderer>();
			if ((bool)component2)
			{
				if (skinnedMeshBuffer == null)
				{
					skinnedMeshBuffer = new Mesh();
				}
				component2.BakeMesh(skinnedMeshBuffer);
				skinnedMeshVertices = skinnedMeshBuffer.vertices;
				targets = skinnedMeshBuffer.vertices;
				int max = skinnedMeshVertices.Length;
				int num7 = _index + _length;
				for (int num8 = _index; num8 < num7; num8++)
				{
					num5 = ((!useParticleSpeedCurve) ? particleSpeedToMesh : particleSpeedToMeshAnimation.Evaluate(particles[num8].remainingLifetime));
					if (num5 < 0f)
					{
						num5 = 0f;
					}
					safeIndex = num8 % targets.Length;
					if (particles[num8].startLifetime - particles[num8].remainingLifetime <= 0.05f)
					{
						particles[num8].position = skinnedMeshTarget.transform.TransformPoint(skinnedMeshVertices[Random.Range(0, max)]);
						continue;
					}
					Vector3 vector6 = skinnedMeshTarget.transform.TransformPoint(targets[safeIndex]);
					Vector3 position3 = particles[num8].position;
					if (meshFollow == HayateEnums.MeshFollow.byDistance)
					{
						particles[num8].position = Vector3.Lerp(particles[num8].position, vector6, 1f / Mathf.Pow(Vector3.Distance(vector6, position3), 2f) * num5);
						continue;
					}
					if (meshFollow == HayateEnums.MeshFollow.byTime)
					{
						particles[num8].position = Vector3.Lerp(position3, vector6, num5 * deltaTime);
						continue;
					}
					_desiredVelocity = vector6 - position3;
					particles[num8].velocity += Vector3.Normalize(_desiredVelocity) * num5 * deltaTime;
				}
			}
			else
			{
				Debug.LogWarning("No SkinnedMesh attached to this GameObject!");
				skinnedMeshTarget = null;
			}
		}
		else
		{
			Debug.LogError("Index empty! Thread can't execute.");
		}
	}

	public void OnDestroy()
	{
		if (skinnedMeshBuffer != null)
		{
			Object.Destroy(skinnedMeshBuffer);
		}
	}

	public void UpdateTexture()
	{
		if ((bool)Turbulence)
		{
			turbulenceWidth = Turbulence.width;
			turbulence = Turbulence.GetPixels32(0);
		}
		else
		{
			Debug.LogWarning("No texture assigned. Precalculated texture turbulence won't work!");
		}
	}

	public AnimationCurve EmptyAnimationCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		Keyframe key = default(Keyframe);
		Keyframe key2 = default(Keyframe);
		key.time = 0f;
		key.value = -1f;
		key2.time = m_CurrentParticleSystem.duration;
		key2.value = 1f;
		animationCurve.AddKey(key);
		animationCurve.AddKey(key2);
		return animationCurve;
	}

	public void EvaluateCurves(ParticleSystem system)
	{
		float time = system.time;
		if (useAmplitudeCurve)
		{
			Amplitude = new Vector3(AmplitudeCurveX.Evaluate(time), AmplitudeCurveY.Evaluate(time), AmplitudeCurveZ.Evaluate(time));
		}
		if (useFrequencyCurve)
		{
			Frequency = new Vector3(FrequencyCurveX.Evaluate(time), FrequencyCurveY.Evaluate(time), FrequencyCurveZ.Evaluate(time));
		}
		if (useOffsetCurve)
		{
			Offset = new Vector3(OffsetCurveX.Evaluate(time), OffsetCurveY.Evaluate(time), OffsetCurveZ.Evaluate(time));
		}
		if (useOffsetSpeedCurve)
		{
			OffsetSpeed = new Vector3(OffsetSpeedCurveX.Evaluate(time), OffsetSpeedCurveY.Evaluate(time), OffsetSpeedCurveZ.Evaluate(time));
		}
		if (useGlobalForceCurve)
		{
			GlobalForce = new Vector3(GlobalForceCurveX.Evaluate(time), GlobalForceCurveY.Evaluate(time), GlobalForceCurveZ.Evaluate(time));
		}
		if (useParticleSpeedCurve)
		{
			particleSpeedToMesh = particleSpeedToMeshAnimation.Evaluate(time);
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (burstOnCollision)
		{
			m_CurrentParticleSystem.Emit(burstNum);
			m_CurrentParticleSystem.Play();
		}
	}

	public void CreateParticles(Vector3 _Position, float _Width, float _Height, float _Depth, float TargetParticleSize, float _UiScale, HayateEnums.BuildOrder _buildOrder)
	{
		m_CurrentParticleSystem.Stop();
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[maxParticles];
		array = HayateHelper.CreateParticlesDynamically(this, _Position, _Width, _Height, _Depth, TargetParticleSize, _UiScale, _buildOrder);
		m_CurrentParticleSystem.SetParticles(array, maxParticles);
		m_CurrentParticleSystem.Play();
	}
}

}
