// Recovered original source behavior; namespace isolated.
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public class MeshParticleVelocityAlign : MonoBehaviour
{
	public float lifeTime = 5f;

	public float loopTimeOffset = 0.01f;

	public int particleEmitCount = 5;

	public bool looping;

	public Vector3 RotationCorrection = Vector3.zero;

	private ParticleSystem _particleSys;

	private ParticleSystem.Particle[] _particles;

	private float _timer;

	private Vector3 _iteratedRotation;

	private Vector3 _dirVector;

	private Vector3[] _pos;

	private Vector3[] _posLast;

	private void Start()
	{
		_particleSys = GetComponent<ParticleSystem>();
		ResetParticles();
	}

	private void Update()
	{
		if (_particles == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_timer += Time.deltaTime;
		if (_timer > lifeTime + loopTimeOffset && looping)
		{
			_timer = 0f;
			ResetParticles();
		}
		_particleSys.GetParticles(_particles);
		if (particleEmitCount == 0)
		{
			return;
		}
		for (int i = 0; i < particleEmitCount; i++)
		{
			ref Vector3 reference = ref _pos[i];
			reference = _particles[i].position;
			_dirVector = _pos[i] - _posLast[i];
			if (_dirVector != Vector3.zero)
			{
				_iteratedRotation = Quaternion.LookRotation(_dirVector.normalized, Vector3.up).eulerAngles;
				if (_iteratedRotation != Vector3.zero)
				{
					_particles[i].rotation3D = _iteratedRotation + RotationCorrection;
				}
			}
			ref Vector3 reference2 = ref _posLast[i];
			reference2 = _particles[i].position;
		}
		_particleSys.SetParticles(_particles, particleEmitCount);
	}

	private void ResetParticles()
	{
		if (_particleSys == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_particleSys.Emit(particleEmitCount);
		_particles = null;
		_particles = new ParticleSystem.Particle[particleEmitCount];
		_pos = new Vector3[particleEmitCount];
		_posLast = new Vector3[particleEmitCount];
		for (int i = 0; i < particleEmitCount; i++)
		{
			ref Vector3 reference = ref _pos[i];
			reference = new Vector3(0f, 0f, 0f);
			ref Vector3 reference2 = ref _posLast[i];
			reference2 = new Vector3(0f, 0f, 0f);
		}
	}
}

}
