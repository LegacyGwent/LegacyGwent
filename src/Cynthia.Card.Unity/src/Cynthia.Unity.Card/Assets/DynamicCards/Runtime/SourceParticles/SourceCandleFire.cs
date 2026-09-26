// Original card candle mesh, atlas and turbulence behavior.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Script.DynamicCards.SourceParticles
{

public class SourceCandleFire : MonoBehaviour
{
	public List<Transform> Positions = new List<Transform>();

	public List<float> Sizes = new List<float>();

	public List<float> TurbulenceStrength = new List<float>();

	public List<int> TextureIndex = new List<int>();

	public int TextureSizeX = 4;

	public int TextureSizeY = 2;

	public MeshFilter CandleMeshFilter;

	public MeshRenderer CandleRenderer;

	public Mesh mesh;

	public Material CandleMat;

	public Texture2D CandleTexture;

	public Texture2D NoiseTexture;

	private SourceCandleTextureOffset TexOffset;
	public Shader SourceShader;

	public int CandleRenderQueue = 3000;

	public float MinTurbulence = 0.25f;

	public float MaxTurbulence = 0.75f;

	public float MinTurbulenceLimit;

	public float MaxTurbulenceLimit = 1f;

	public float MinSize = 0.25f;

	public float MaxSize = 0.75f;

	public float MinSizeLimit;

	public float MaxSizeLimit = 1f;

	public float MinIndex;

	public float MaxIndex = 8f;

	public float MinIndexLimit;

	public float MaxIndexLimit = 1f;

	public BlendMode BlendSource = BlendMode.One;

	public BlendMode BlendTarget = BlendMode.One;

	public bool UseZwriteShader;

	public float StartDelay;

	public bool AdjustToScale;

	private void Awake()
	{
		if (StartDelay <= 0f)
		{
			StartCoroutine(WaitBeforCreeation());
		}
		else
		{
			CreateMesh();
		}
	}

	private IEnumerator WaitBeforCreeation()
	{
		yield return new WaitForSeconds(StartDelay);
		CreateMesh();
	}

	public void CreateMesh()
	{
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear();
		}
		Shader shader = SourceShader;
		if (shader == null)
		{
			Debug.LogError("[ERROR] Could not create candle lights. Candle billboard shader was not created properly");
			Object.Destroy(mesh);
			return;
		}
		CandleMat = new Material(shader);
		CandleMat.renderQueue = CandleRenderQueue;
		CandleMat.SetTexture("_MainTex", CandleTexture);
		CandleMat.SetTexture("_Turbulence", NoiseTexture);
		CandleMat.SetFloat("_SrcMode", (float)BlendSource);
		CandleMat.SetFloat("_DstMode", (float)BlendTarget);
		Vector2 vector = default(Vector2);
		vector.x = 1f / (float)TextureSizeX;
		vector.y = 1f / (float)TextureSizeY;
		if (TexOffset == null)
		{
			TexOffset = base.gameObject.GetComponent<SourceCandleTextureOffset>();
			if (TexOffset == null)
			{
				TexOffset = base.gameObject.AddComponent<SourceCandleTextureOffset>();
				TexOffset.textureName = "_Turbulence";
				TexOffset.xOffsetMulti = 0.7f;
				TexOffset.yOffsetMulti = 0.2f;
			}
			TexOffset.enabled = true;
		}
		float num = 1f;
		if (AdjustToScale)
		{
			Vector3 lossyScale = base.transform.lossyScale;
			num = (lossyScale.x + lossyScale.y + lossyScale.z) / 3f;
		}
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector2> list3 = new List<Vector2>();
		List<Vector2> list4 = new List<Vector2>();
		List<Color> list5 = new List<Color>();
		List<Transform> positions = Positions;
		int count = positions.Count;
		Vector2 item2 = default(Vector2);
		for (int i = 0; i < count; i++)
		{
			Transform transform = positions[i];
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Vector3 localScale = transform.localScale;
			float num2 = Sizes[i] * num;
			float x = localScale.x;
			float y = localScale.y;
			float x2 = (0f - num2) * x / 2f;
			float x3 = num2 * x / 2f;
			float y2 = num2 * y;
			list.Add(base.transform.InverseTransformPoint(rotation * new Vector3(x2, 0f, 0f) + position));
			list.Add(base.transform.InverseTransformPoint(rotation * new Vector3(x3, 0f, 0f) + position));
			list.Add(base.transform.InverseTransformPoint(rotation * new Vector3(x3, y2, 0f) + position));
			list.Add(base.transform.InverseTransformPoint(rotation * new Vector3(x2, y2, 0f) + position));
			int num3 = i * 4;
			list2.Add(num3);
			list2.Add(num3 + 3);
			list2.Add(num3 + 2);
			list2.Add(num3);
			list2.Add(num3 + 2);
			list2.Add(num3 + 1);
			int num4 = TextureIndex[i];
			float num5 = num4 / TextureSizeX;
			Vector2 item = new Vector2((float)num4 / (float)TextureSizeX - num5, num5 / (float)TextureSizeY);
			list3.Add(item);
			list3.Add(new Vector2(item.x + vector.x, item.y));
			list3.Add(new Vector2(item.x + vector.x, item.y + vector.y));
			list3.Add(new Vector2(item.x, item.y + vector.y));
			float x4 = ((position.x + position.z + 1f) / 2f);
			float y3 = 1f;
			item2.x = x4;
			item2.y = y3;
			list4.Add(Vector2.zero);
			list4.Add(Vector2.zero);
			list4.Add(item2);
			list4.Add(item2);
			float num6 = TurbulenceStrength[i];
			Color item3 = new Color(num6, num6, num6, 0f);
			list5.Add(Color.black);
			list5.Add(Color.black);
			list5.Add(item3);
			list5.Add(item3);
		}
		mesh.SetVertices(list);
		mesh.SetTriangles(list2.ToArray(), 0);
		mesh.SetUVs(0, list3);
		mesh.SetUVs(1, list4);
		mesh.SetColors(list5);
		mesh.RecalculateBounds();
		CandleMeshFilter.mesh = mesh;
		CandleRenderer.sharedMaterial = CandleMat;
		CandleRenderer.forceRenderingOff = false;
		TexOffset.mat = CandleMat;
		if (Application.isPlaying)
		{
			CleanUp();
		}
	}

	public void OnDestroy()
	{
		if (CandleMat != null)
		{
			Object.Destroy(CandleMat);
			CandleMat = null;
		}
		if (mesh != null)
		{
			Object.Destroy(mesh);
			mesh = null;
		}
	}

	public void SetMainTex(Texture2D tex)
	{
		CandleTexture = tex;
		CandleMat.SetTexture("_MainTex", CandleTexture);
	}

	public void CleanUp()
	{
		for (int i = 0; i < Positions.Count; i++)
		{
			Object.Destroy(Positions[i].gameObject);
		}
		Positions.Clear();
	}
}

}
