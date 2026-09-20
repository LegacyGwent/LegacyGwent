// Original lightning behavior, with isolated namespace and explicit source shader.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Script.DynamicCards.SourceLightning
{

public class LightningTool : MonoBehaviour
{
	[Serializable]
	public class Lightning
	{
		public LightningTypes LightningType;

		public LightningFaceing Faceing;

		public TurbulenceStyle Style;

		public GameObject MeshHolder;

		public float ProgressAtStart;

		public GameObject StartObj;

		public GameObject EndObj;

		public Vector3 StartPoint = default(Vector3);

		public Vector3 Direction = default(Vector3);

		public Vector3 Amplitude = Vector3.one;

		public Vector3 Frequency = Vector3.one;

		public Vector3 Offset = Vector3.zero;

		public float Length = 10f;

		public int Segments = 10;

		public float Branching = 1f;

		public AnimationCurve BranchingAmount = new AnimationCurve();

		public AnimationCurve Opacity = new AnimationCurve();

		public float BranchLengthModifier = 1f;

		public float BranchAngleModifier = 1f;

		public List<Vector3> Elements = new List<Vector3>(52);

		public List<Branch> Branches = new List<Branch>(4);
	}

	public class Branch
	{
		public LightningFaceing Faceing;

		public TurbulenceStyle Style;

		public float ProgressAtStart;

		public Vector3 StartPoint = default(Vector3);

		public Vector3 Direction = default(Vector3);

		public Vector3 Amplitude = Vector3.one;

		public Vector3 Frequency = Vector3.one;

		public Vector3 Offset = Vector3.zero;

		public float Length = 10f;

		public int Segments = 10;

		public float Branching = 1f;

		public AnimationCurve BranchingAmount = new AnimationCurve();

		public AnimationCurve Opacity = new AnimationCurve();

		public float BranchLengthModifier = 1f;

		public float BranchAngleModifier = 1f;

		public List<Vector3> Elements = new List<Vector3>(52);

		public List<Branch> Branches = new List<Branch>(4);
	}

	public enum LightningTypes
	{
		Directional,
		PointToPoint
	}

	public enum LightningFaceing
	{
		Single,
		Cross,
		Camera
	}

	public enum TurbulenceStyle
	{
		Position,
		Index
	}

	public Transform ObjectToFace;
	public Shader SourceShader;

	public List<Lightning> Lightnings = new List<Lightning>();

	public Vector3 SeedOffset = Vector3.zero;

	public AnimationCurve SizeOverLength = new AnimationCurve();

	public Texture2D LightningTexture;

	public Texture2D TurbulenceTexture;

	public AnimationCurve TurbulenceStrength = new AnimationCurve();

	public int RenderQueue = 3000;

	public float LightningWidth = 0.2f;

	public float BranchDelay = 0.25f;

	private LightningAnimator lightningAnimator;

	private Quaternion dir;

	private List<Vector3> Vertices = new List<Vector3>(164);

	private List<Vector3> Normals = new List<Vector3>(164);

	private List<int> Triangles = new List<int>(512);

	private List<Vector2> UV0 = new List<Vector2>(164);

	private List<Vector2> UV1 = new List<Vector2>(164);

	public static string[] LightningNames;

	public static readonly Vector3 INITIAL_AMPLITUDE;

	public static readonly Vector3 INITIAL_FREQUENCY;

	public static readonly Vector3 INITIAL_OFFSET;

	public static readonly Vector3 INITIAL_START_POSITION;

	public static readonly Vector3 INITIAL_END_POSITION;

	private List<Material> m_CreatedMaterials = new List<Material>(4);

	private List<Mesh> m_CreatedMeshes = new List<Mesh>(4);

	static LightningTool()
	{
		LightningNames = null;
		INITIAL_AMPLITUDE = new Vector3(1f, 1f, 1f);
		INITIAL_FREQUENCY = new Vector3(0.6f, 1f, 1f);
		INITIAL_OFFSET = Vector3.zero;
		INITIAL_START_POSITION = new Vector3(0f, 10f, 0f);
		INITIAL_END_POSITION = Vector3.zero;
		if (LightningNames == null)
		{
			Initialize();
		}
	}

	public static void Initialize()
	{
		if (LightningNames == null)
		{
			LightningNames = new string[25];
			StringBuilder stringBuilder = new StringBuilder(32);
			for (int i = 0; i < 25; i++)
			{
				stringBuilder.Clear().Append("Lightning_").Append(i);
				LightningNames[i] = stringBuilder.ToString();
			}
		}
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			RecreateLightnings();
		}
	}

	private void OnDisable()
	{
	}

	public void RecreateLightnings()
	{
		CalcualteLightnings();
		CreateMeshLightning();
	}

	public void DestroyLightnings()
	{
		foreach (Lightning lightning in Lightnings)
		{
			if (lightning.MeshHolder != null)
			{
				UnityEngine.Object.Destroy(lightning.MeshHolder.gameObject);
			}
			lightning.MeshHolder = null;
		}
	}

	private Lightning CreateLightningBetweenPoints(Lightning _l, int _iteration)
	{
		if (_l.StartObj != null)
		{
			_l.StartPoint = _l.StartObj.transform.position;
		}
		Vector3 position = _l.EndObj.transform.position;
		if (_l.EndObj != null)
		{
			_l.Direction = Vector3.Normalize(_l.EndObj.transform.position - _l.StartPoint);
		}
		_l.Length = Vector3.Distance(_l.StartPoint, position);
		float num = _l.Length / (float)_l.Segments;
		Vector3 vector = _l.Direction * num;
		int segments = _l.Segments;
		for (int i = 0; i < segments + 1; i++)
		{
			_l.Elements.Add(_l.StartPoint + vector * i);
		}
		Vector3 offset = _l.Offset;
		offset += SeedOffset;
		float num2 = 0f;
		_l.Elements[0] = _l.StartPoint;
		float num3 = _l.Amplitude.x / 10f;
		float num4 = _l.Amplitude.y / 10f;
		float num5 = _l.Amplitude.z / 10f;
		for (int j = 1; j < _l.Elements.Count; j++)
		{
			num2 = (float)j / (float)_l.Segments;
			Vector3 vector2 = _l.Elements[j];
			float num6 = (float)j / _l.Frequency.x;
			float num7 = (float)j / _l.Frequency.y;
			float num8 = (float)j / _l.Frequency.z;
			float num9 = Mathf.PerlinNoise(num6 + offset.x, num6 + offset.x) * 2f - 1f;
			float num10 = Mathf.PerlinNoise(num7 + offset.y, num7 + offset.y) * 2f - 1f;
			float num11 = Mathf.PerlinNoise(num8 + offset.z, num8 + offset.z) * 2f - 1f;
			Vector3 vector3 = new Vector3(num9 * num3, num10 * num4, num11 * num5);
			if ((Mathf.Abs(num9) + Mathf.Abs(num10) + Mathf.Abs(num11)) / 3f <= _l.BranchingAmount.Evaluate(num2) * _l.Branching / 100f)
			{
				Branch emptyBranch = LightningPool.GetEmptyBranch();
				emptyBranch.StartPoint = vector2;
				Vector3 vector4 = Vector3.Normalize(new Vector3(Mathf.PerlinNoise(j, SeedOffset.x), Mathf.PerlinNoise(j, SeedOffset.y), Mathf.PerlinNoise(j, SeedOffset.z)));
				vector4.x = vector4.x * 2f - 1f;
				vector4.y = vector4.y * 2f - 1f;
				vector4.z = vector4.z * 2f - 1f;
				vector4 *= _l.BranchAngleModifier;
				emptyBranch.Direction = Vector3.Normalize(_l.Direction + vector4 * _l.BranchAngleModifier);
				emptyBranch.Style = _l.Style;
				emptyBranch.ProgressAtStart = num2;
				emptyBranch.Amplitude = _l.Amplitude;
				emptyBranch.Frequency = _l.Frequency;
				emptyBranch.Offset = _l.Offset;
				emptyBranch.BranchingAmount = _l.BranchingAmount;
				emptyBranch.StartPoint = vector2 + vector3;
				emptyBranch.Segments = Mathf.RoundToInt((float)(_l.Segments / (_iteration + 1)) * _l.BranchLengthModifier);
				emptyBranch.Length = _l.Length / (float)(_iteration + 1) * _l.BranchLengthModifier;
				emptyBranch.Branching = _l.Branching;
				_iteration++;
				emptyBranch = CreateBranch(emptyBranch, _iteration);
				_l.Branches.Add(emptyBranch);
			}
			if (j == _l.Elements.Count - 1)
			{
				vector2 = _l.EndObj.transform.position;
			}
			else
			{
				vector2 += vector3;
			}
			_l.Elements[j] = vector2;
		}
		return _l;
	}

	private Lightning CreateLightning(Lightning _l, int _iteration)
	{
		if (_l.StartObj != null)
		{
			_l.StartPoint = _l.StartObj.transform.position;
		}
		if (_l.EndObj != null)
		{
			_l.Direction = Vector3.Normalize(_l.EndObj.transform.position - _l.StartPoint);
		}
		Vector3 vector = _l.StartPoint;
		Vector3 offset = _l.Offset;
		float num = _l.Length / (float)_l.Segments;
		offset += SeedOffset;
		for (int i = 0; i < _l.Segments; i++)
		{
			if (i == 0)
			{
				_l.Elements.Add(_l.StartPoint);
				continue;
			}
			float num5;
			float num6;
			float num7;
			if (_l.Style == TurbulenceStyle.Index)
			{
				vector += _l.Direction * (_l.Length / (float)_l.Segments);
				float num2 = (float)i / _l.Frequency.x;
				float num3 = (float)i / _l.Frequency.y;
				float num4 = (float)i / _l.Frequency.z;
				num5 = Mathf.PerlinNoise(num2 + offset.x, num2 + offset.x) * 2f - 1f;
				num6 = Mathf.PerlinNoise(num3 + offset.y, num3 + offset.y) * 2f - 1f;
				num7 = Mathf.PerlinNoise(num4 + offset.z, num4 + offset.z) * 2f - 1f;
			}
			else
			{
				vector = _l.Elements[_l.Elements.Count - 1];
				num5 = Mathf.PerlinNoise(vector.y / _l.Frequency.x + offset.x, vector.z / _l.Frequency.x + offset.x) * 2f - 1f;
				num6 = Mathf.PerlinNoise(vector.x / _l.Frequency.y + offset.y, vector.z / _l.Frequency.y + offset.y) * 2f - 1f;
				num7 = Mathf.PerlinNoise(vector.x / _l.Frequency.z + offset.z, vector.y / _l.Frequency.z + offset.z) * 2f - 1f;
			}
			Vector3 vector2 = new Vector3(num5 * _l.Amplitude.x, num6 * _l.Amplitude.y, num7 * _l.Amplitude.z);
			vector += vector2 * num;
			float num8 = (float)i / (float)_l.Segments;
			if (Mathf.PerlinNoise((float)i / _l.Frequency.x + offset.x, (float)i / _l.Frequency.x + offset.x) <= _l.BranchingAmount.Evaluate(num8) * _l.Branching / 100f)
			{
				Branch emptyBranch = LightningPool.GetEmptyBranch();
				emptyBranch.StartPoint = vector;
				Vector3 vector3 = Vector3.Normalize(new Vector3(Mathf.PerlinNoise((float)i + _l.Offset.y, (float)i + _l.Offset.z), Mathf.PerlinNoise((float)i + _l.Offset.x, (float)i + _l.Offset.z), Mathf.PerlinNoise((float)i + _l.Offset.x, (float)i + _l.Offset.y)));
				vector3.x = vector3.x * 2f - 1f;
				vector3.y = vector3.y * 2f - 1f;
				vector3.z = vector3.z * 2f - 1f;
				vector3 *= _l.BranchAngleModifier;
				emptyBranch.Direction = Vector3.Normalize(_l.Direction + vector3 * _l.BranchAngleModifier);
				emptyBranch.Style = _l.Style;
				emptyBranch.ProgressAtStart = num8;
				emptyBranch.Amplitude = _l.Amplitude;
				emptyBranch.Frequency = _l.Frequency;
				emptyBranch.Offset = _l.Offset * vector3.x;
				emptyBranch.BranchingAmount = _l.BranchingAmount;
				emptyBranch.Segments = Mathf.RoundToInt((float)(_l.Segments / (_iteration + 1)) * _l.BranchLengthModifier);
				emptyBranch.Length = _l.Length / (float)(_iteration + 1) * _l.BranchLengthModifier;
				emptyBranch.Branching = _l.Branching;
				_iteration++;
				emptyBranch = CreateBranch(emptyBranch, _iteration);
				_l.Branches.Add(emptyBranch);
			}
			_l.Elements.Add(vector);
		}
		return _l;
	}

	private Branch CreateBranch(Branch _l, int _iteration)
	{
		Vector3 vector = _l.StartPoint;
		float num = _l.Length / (float)_l.Segments;
		Vector3 offset = _l.Offset;
		offset += SeedOffset;
		for (int i = 0; i < _l.Segments; i++)
		{
			if (i == 0)
			{
				_l.Elements.Add(_l.StartPoint);
				continue;
			}
			float num5;
			float num6;
			float num7;
			if (_l.Style == TurbulenceStyle.Index)
			{
				vector += _l.Direction * (_l.Length / (float)_l.Segments);
				float num2 = (float)i / _l.Frequency.x;
				float num3 = (float)i / _l.Frequency.y;
				float num4 = (float)i / _l.Frequency.z;
				num5 = Mathf.PerlinNoise(num2 + offset.x, num2 + offset.x) * 2f - 1f;
				num6 = Mathf.PerlinNoise(num3 + offset.y, num3 + offset.y) * 2f - 1f;
				num7 = Mathf.PerlinNoise(num4 + offset.z, num4 + offset.z) * 2f - 1f;
			}
			else
			{
				vector = _l.Elements[_l.Elements.Count - 1];
				num5 = Mathf.PerlinNoise(vector.y / _l.Frequency.x + offset.x, vector.z / _l.Frequency.x + offset.x) * 2f - 1f;
				num6 = Mathf.PerlinNoise(vector.x / _l.Frequency.y + offset.y, vector.z / _l.Frequency.y + offset.y) * 2f - 1f;
				num7 = Mathf.PerlinNoise(vector.x / _l.Frequency.z + offset.z, vector.y / _l.Frequency.z + offset.z) * 2f - 1f;
			}
			Vector3 vector2 = new Vector3(num5 * _l.Amplitude.x, num6 * _l.Amplitude.y, num7 * _l.Amplitude.z);
			vector += vector2 * num;
			float num8 = (float)i / (float)_l.Segments;
			if (Mathf.PerlinNoise((float)i / _l.Frequency.x + offset.x, (float)i / _l.Frequency.x + offset.x) <= _l.BranchingAmount.Evaluate(num8) * _l.Branching / 100f)
			{
				Branch emptyBranch = LightningPool.GetEmptyBranch();
				emptyBranch.StartPoint = vector;
				Vector3 vector3 = Vector3.Normalize(new Vector3(Mathf.PerlinNoise((float)i + _l.Offset.y, (float)i + _l.Offset.z), Mathf.PerlinNoise((float)i + _l.Offset.x, (float)i + _l.Offset.z), Mathf.PerlinNoise((float)i + _l.Offset.x, (float)i + _l.Offset.y)));
				vector3.x = vector3.x * 2f - 1f;
				vector3.y = vector3.y * 2f - 1f;
				vector3.z = vector3.z * 2f - 1f;
				vector3 *= _l.BranchAngleModifier;
				emptyBranch.Direction = Vector3.Normalize(_l.Direction + vector3 * _l.BranchAngleModifier);
				emptyBranch.Style = _l.Style;
				emptyBranch.ProgressAtStart = num8;
				emptyBranch.Amplitude = _l.Amplitude;
				emptyBranch.Frequency = _l.Frequency;
				emptyBranch.Offset = _l.Offset * vector3.x;
				emptyBranch.BranchingAmount = _l.BranchingAmount;
				emptyBranch.Opacity = _l.Opacity;
				emptyBranch.Segments = Mathf.RoundToInt((float)(_l.Segments / (_iteration + 1)) * _l.BranchLengthModifier);
				emptyBranch.Length = _l.Length / (float)(_iteration + 1) * _l.BranchLengthModifier;
				emptyBranch.Branching = _l.Branching - (float)(_iteration * 2);
				_iteration++;
				emptyBranch = CreateBranch(emptyBranch, _iteration);
				_l.Branches.Add(emptyBranch);
			}
			_l.Elements.Add(vector);
		}
		return _l;
	}

	public void CreateMeshLightning()
	{
		Shader shader = null;
		shader = SourceShader;
		if (shader == null)
		{
			Debug.LogWarning("[ERROR] Could not create lightning. Lightning shader was not created properly");
			return;
		}
		for (int i = 0; i < Lightnings.Count; i++)
		{
			Lightning lightning = Lightnings[i];
			MeshFilter meshFilter;
			MeshRenderer meshRenderer;
			Material material;
			if (lightning.MeshHolder == null)
			{
				lightning.MeshHolder = new GameObject();
				GameObject meshHolder = lightning.MeshHolder;
				meshHolder.layer = base.gameObject.layer;
				meshHolder.transform.SetParent(lightning.StartObj.transform);
				meshHolder.transform.position = lightning.StartPoint;
				meshHolder.name = LightningNames[i];
				meshFilter = meshHolder.AddComponent<MeshFilter>();
				meshRenderer = meshHolder.AddComponent<MeshRenderer>();
				meshRenderer.receiveShadows = false;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				material = new Material(shader);
				material.renderQueue = RenderQueue;
				material.SetTexture("_MainTex", LightningTexture);
				material.SetTexture("_Turbulence", TurbulenceTexture);
				m_CreatedMaterials.Add(material);
			}
			else
			{
				GameObject meshHolder2 = lightning.MeshHolder;
				meshHolder2.transform.position = lightning.StartPoint;
				meshHolder2.layer = base.gameObject.layer;
				meshFilter = meshHolder2.GetComponent<MeshFilter>();
				meshRenderer = meshHolder2.GetComponent<MeshRenderer>();
				material = meshRenderer.sharedMaterial;
				if (material != null)
				{
					material.renderQueue = RenderQueue;
					material.SetTexture("_MainTex", LightningTexture);
					material.SetTexture("_Turbulence", TurbulenceTexture);
				}
			}
			List<List<Vector3>> list = new List<List<Vector3>>(4);
			List<float> list2 = new List<float>(4);
			list.Add(lightning.Elements);
			list2.Add(lightning.ProgressAtStart);
			int count = lightning.Branches.Count;
			for (int j = 0; j < count; j++)
			{
				Branch branch = lightning.Branches[j];
				CycleThroughBranches(list, list2, branch);
				list.Add(branch.Elements);
				list2.Add(branch.ProgressAtStart);
			}
			var previousMesh = meshFilter.sharedMesh;
			if (previousMesh != null && m_CreatedMeshes.Remove(previousMesh)) Destroy(previousMesh);
			Transform meshHolder3 = lightning.MeshHolder.transform;
			AnimationCurve opacity = lightning.Opacity;
			switch (lightning.Faceing)
            {
                case LightningFaceing.Cross: meshFilter.mesh = BakeLightningCross(list, meshHolder3, list2, opacity); break;
                case LightningFaceing.Camera: meshFilter.mesh = BakeLightningCamFaceing(list, meshHolder3, list2, opacity); break;
                default: meshFilter.mesh = BakeLightningSingle(list, meshHolder3, list2, opacity); break;
            }
			m_CreatedMeshes.Add(meshFilter.sharedMesh);
			meshRenderer.material = material;
		}
	}

	public List<List<Vector3>> CycleThroughBranches(List<List<Vector3>> _List, List<float> _ProgressAtStart, Branch _branch)
	{
		int count = _branch.Branches.Count;
		for (int i = 0; i < count; i++)
		{
			Branch branch = _branch.Branches[i];
			_List.Add(branch.Elements);
			_ProgressAtStart.Add(branch.ProgressAtStart);
		}
		for (int j = 0; j < count; j++)
		{
			CycleThroughBranches(_List, _ProgressAtStart, _branch.Branches[j]);
		}
		return _List;
	}

	private Mesh BakeLightningSingle(List<List<Vector3>> _points, Transform _meshHolder, List<float> _Progress, AnimationCurve _Opacities)
	{
		CalcualteLightnings();
		int count = _points.Count;
		List<Vector3> vertices = Vertices;
		List<int> triangles = Triangles;
		List<Vector3> normals = Normals;
		List<Vector2> uV = UV0;
		List<Vector2> uV2 = UV1;
		List<Color> list = new List<Color>(count * 2);
		normals.Clear();
		vertices.Clear();
		triangles.Clear();
		uV.Clear();
		uV2.Clear();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			int count2 = _points[i].Count;
			for (int j = 0; j < count2; j++)
			{
				float value = (float)j / (float)count2;
				value = value.Remap(0f, 1f, _Progress[i], 1f);
				Vector3 vector = _points[i][j];
				if (j < count2 - 1)
				{
					dir = Quaternion.LookRotation(Vector3.Normalize(vector - _points[i][j + 1]), Vector3.back);
				}
				float num3 = SizeOverLength.Evaluate(value);
				vertices.Add(_meshHolder.InverseTransformPoint(vector + dir * new Vector3((0f - LightningWidth) * num3, 0f, 0f)));
				vertices.Add(_meshHolder.InverseTransformPoint(vector + dir * new Vector3(LightningWidth * num3, 0f, 0f)));
				normals.Add(dir * Vector3.left);
				normals.Add(dir * Vector3.right);
				float y = 1f - value;
				Vector2 item = new Vector2(0.5f, value);
				uV.Add(new Vector2(0f, y));
				uV.Add(new Vector2(1f, y));
				uV2.Add(item);
				uV2.Add(item);
				float r = _Opacities.Evaluate(value);
				Color item2 = ((j != 0) ? new Color(r, TurbulenceStrength.Evaluate(value), num3, 1f) : new Color(r, 0f, num3, 1f));
				list.Add(item2);
				list.Add(item2);
				num2 += 2;
			}
			int num4 = num + count2 - 1;
			for (int k = num; k < num4; k++)
			{
				int num5 = k * 2;
				triangles.Add(num5);
				triangles.Add(num5 + 1);
				triangles.Add(num5 + 2);
				triangles.Add(num5 + 2);
				triangles.Add(num5 + 1);
				triangles.Add(num5 + 3);
			}
			num += count2;
		}
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.SetNormals(normals);
		uV = AlignToUV(uV);
		mesh.SetUVs(0, uV);
		mesh.SetUVs(1, uV2);
		mesh.SetColors(list);
		m_CreatedMeshes.Add(mesh);
		return mesh;
	}

	private Mesh BakeLightningCross(List<List<Vector3>> _points, Transform _meshHolder, List<float> _Progress, AnimationCurve _Opacities)
	{
		int count = _points.Count;
		List<Vector3> vertices = Vertices;
		List<int> triangles = Triangles;
		List<Vector3> normals = Normals;
		List<Vector2> uV = UV0;
		List<Vector2> uV2 = UV1;
		List<Color> list = new List<Color>(count * 2);
		normals.Clear();
		triangles.Clear();
		vertices.Clear();
		uV.Clear();
		uV2.Clear();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			int count2 = _points[i].Count;
			for (int j = 0; j < count2; j++)
			{
				float value = (float)j / (float)count2;
				value = value.Remap(0f, 1f, _Progress[i], 1f);
				if (j < count2 - 1)
				{
					dir = Quaternion.LookRotation(Vector3.Normalize(_points[i][j] - _points[i][j + 1]), Vector3.back);
				}
				float num3 = SizeOverLength.Evaluate(value);
				vertices.Add(_meshHolder.InverseTransformPoint(_points[i][j] + dir * new Vector3((0f - LightningWidth) * num3, 0f, 0f)));
				vertices.Add(_meshHolder.InverseTransformPoint(_points[i][j] + dir * new Vector3(LightningWidth * num3, 0f, 0f)));
				normals.Add(dir * Vector3.left);
				normals.Add(dir * Vector3.right);
				float y = 1f - value;
				Vector2 item = new Vector2(0.5f, value);
				uV.Add(new Vector2(0f, y));
				uV.Add(new Vector2(1f, y));
				uV2.Add(item);
				uV2.Add(item);
				float r = _Opacities.Evaluate(value);
				Color item2 = ((j != 0) ? new Color(r, TurbulenceStrength.Evaluate(value), num3, 1f) : new Color(r, 0f, num3, 1f));
				list.Add(item2);
				list.Add(item2);
				num2 += 2;
			}
			for (int k = num; k < num + count2 - 1; k++)
			{
				int num4 = k * 2;
				triangles.Add(num4);
				triangles.Add(num4 + 1);
				triangles.Add(num4 + 3);
				triangles.Add(num4);
				triangles.Add(num4 + 3);
				triangles.Add(num4 + 2);
			}
			num += count2;
			for (int l = 0; l < count2; l++)
			{
				float value2 = (float)l / (float)count2;
				value2 = value2.Remap(0f, 1f, _Progress[i], 1f);
				if (l < count2 - 1)
				{
					dir = Quaternion.LookRotation(Vector3.Normalize(_points[i][l] - _points[i][l + 1]), Vector3.back);
				}
				float num5 = SizeOverLength.Evaluate(value2);
				vertices.Add(_meshHolder.InverseTransformPoint(_points[i][l] + dir * new Vector3(0f, (0f - LightningWidth) * num5, 0f)));
				vertices.Add(_meshHolder.InverseTransformPoint(_points[i][l] + dir * new Vector3(0f, LightningWidth * num5, 0f)));
				normals.Add(dir * Vector3.down);
				normals.Add(dir * Vector3.up);
				Vector2 item3 = new Vector2(0.5f, value2);
				float y2 = 1f - value2;
				uV.Add(new Vector2(0f, y2));
				uV.Add(new Vector2(1f, y2));
				uV2.Add(item3);
				uV2.Add(item3);
				float r2 = _Opacities.Evaluate(value2);
				Color item2 = ((l != 0) ? new Color(r2, TurbulenceStrength.Evaluate(value2), num5, 1f) : new Color(r2, 0f, num5, 1f));
				list.Add(item2);
				list.Add(item2);
				num2 += 2;
			}
			int num6 = num + count2 - 1;
			for (int m = num; m < num6; m++)
			{
				int num7 = m * 2;
				triangles.Add(num7);
				triangles.Add(num7 + 1);
				triangles.Add(num7 + 3);
				triangles.Add(num7);
				triangles.Add(num7 + 3);
				triangles.Add(num7 + 2);
			}
			num += count2;
		}
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.SetNormals(normals);
		uV = AlignToUV(uV);
		mesh.SetUVs(0, uV);
		mesh.SetUVs(1, uV2);
		mesh.SetColors(list);
		m_CreatedMeshes.Add(mesh);
		return mesh;
	}

	private Mesh BakeLightningCamFaceing(List<List<Vector3>> _points, Transform _meshHolder, List<float> _Progress, AnimationCurve _Opacities)
	{
		int count = _points.Count;
		List<Vector3> vertices = Vertices;
		List<int> triangles = Triangles;
		List<Vector3> normals = Normals;
		List<Vector2> uV = UV0;
		List<Vector2> uV2 = UV1;
		List<Color> list = new List<Color>(count * 2);
		triangles.Clear();
		vertices.Clear();
		normals.Clear();
		uV.Clear();
		uV2.Clear();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			int count2 = _points[i].Count;
			for (int j = 0; j < count2; j++)
			{
				float value = (float)j / (float)count2;
				value = value.Remap(0f, 1f, _Progress[i], 1f);
				if (j < count2 - 1)
				{
					dir = Quaternion.LookRotation(Vector3.Normalize(_points[i][j] - _points[i][j + 1]), Vector3.Normalize(ObjectToFace.position - _points[i][j]));
				}
				float num3 = SizeOverLength.Evaluate(value);
				Vector3 item = _meshHolder.InverseTransformPoint(_points[i][j] + dir * new Vector3((0f - LightningWidth) * num3, 0f, 0f));
				Vector3 item2 = _meshHolder.InverseTransformPoint(_points[i][j] + dir * new Vector3(LightningWidth * num3, 0f, 0f));
				vertices.Add(item);
				vertices.Add(item2);
				normals.Add(dir * Vector3.left);
				normals.Add(dir * Vector3.right);
				float y = 1f - value;
				Vector2 item3 = new Vector2(0.2f * (float)i, value);
				uV.Add(new Vector2(0f, y));
				uV.Add(new Vector2(1f, y));
				uV2.Add(item3);
				uV2.Add(item3);
				float r = _Opacities.Evaluate(value);
				Color item4 = ((j != 0) ? new Color(r, TurbulenceStrength.Evaluate(value), num3, 1f) : new Color(r, 0f, num3, 1f));
				list.Add(item4);
				list.Add(item4);
				num2 += 2;
			}
			int num4 = num + count2 - 1;
			for (int k = num; k < num4; k++)
			{
				int num5 = k * 2;
				triangles.Add(num5);
				triangles.Add(num5 + 1);
				triangles.Add(num5 + 3);
				triangles.Add(num5);
				triangles.Add(num5 + 3);
				triangles.Add(num5 + 2);
			}
			num += count2;
		}
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.SetNormals(normals);
		uV = AlignToUV(uV);
		mesh.SetUVs(0, uV);
		mesh.SetUVs(1, uV2);
		mesh.SetColors(list);
		m_CreatedMeshes.Add(mesh);
		return mesh;
	}

	public List<Vector2> AlignToUV(List<Vector2> _UV)
	{
		float num = 0f;
		float num2 = 0f;
		int count = _UV.Count;
		for (int i = 0; i < count; i++)
		{
			Vector2 vector = _UV[i];
			if (vector.y < num)
			{
				num = vector.y;
			}
			if (vector.y > num2)
			{
				num2 = vector.y;
			}
		}
		for (int j = 0; j < count; j++)
		{
			Vector2 value = _UV[j];
			value.y = value.y.Remap(num, num2, 0f, 1f);
			_UV[j] = value;
		}
		return _UV;
	}

	private void Update()
	{
		if (ObjectToFace == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.position = base.transform.position + Vector3.back;
			ObjectToFace = gameObject.transform;
		}
		if (Lightnings == null)
		{
			Lightnings = new List<Lightning>();
		}
	}

	public void CalcualteLightnings()
	{
		int count = Lightnings.Count;
		for (int i = 0; i < count; i++)
		{
			Lightning lightning = Lightnings[i];
			lightning.Branching = Mathf.Clamp(lightning.Branching, 0f, 50f);
			lightning.Elements.Clear();
			lightning.Branches.Clear();
			if (lightning.LightningType == LightningTypes.Directional)
			{
				Lightnings[i] = CreateLightning(lightning, 0);
			}
			else
			{
				Lightnings[i] = CreateLightningBetweenPoints(lightning, 0);
			}
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_CreatedMeshes.Count; i++)
		{
			UnityEngine.Object.Destroy(m_CreatedMeshes[i]);
		}
		for (int j = 0; j < m_CreatedMaterials.Count; j++)
		{
			UnityEngine.Object.Destroy(m_CreatedMaterials[j]);
		}
	}
}

}
