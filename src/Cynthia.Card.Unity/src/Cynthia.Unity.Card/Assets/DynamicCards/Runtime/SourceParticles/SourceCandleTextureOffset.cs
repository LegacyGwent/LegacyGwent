using UnityEngine;

namespace Assets.Script.DynamicCards.SourceParticles
{
public class SourceCandleTextureOffset : MonoBehaviour
{
	[SerializeField]
	private Material m_mat;

	[SerializeField]
	private int m_matIndex;

	[SerializeField]
	private string m_textureName;

	[SerializeField]
	private float m_xOffsetMulti;

	[SerializeField]
	private float m_yOffsetMulti;

	[SerializeField]
	private bool m_ForceStartOffset;

	[SerializeField]
	private Vector2 m_ForcedOffset;

	private Vector2 offset;

	private float offsetX;

	private float offsetY;

	private float m_InitialOffsetX;

	private float m_InitialOffsetY;

	public Material mat
	{
		get
		{
			return m_mat;
		}
		set
		{
			m_mat = value;
		}
	}

	public string textureName
	{
		get
		{
			return m_textureName;
		}
		set
		{
			m_textureName = value;
		}
	}

	public float xOffsetMulti
	{
		get
		{
			return m_xOffsetMulti;
		}
		set
		{
			m_xOffsetMulti = value;
		}
	}

	public float yOffsetMulti
	{
		get
		{
			return m_yOffsetMulti;
		}
		set
		{
			m_yOffsetMulti = value;
		}
	}

	protected void Start()
	{
		mat = GetComponent<Renderer>().sharedMaterials[m_matIndex];
		if (mat.HasProperty(m_textureName))
		{
			m_InitialOffsetX = mat.GetTextureOffset(m_textureName).x;
			m_InitialOffsetY = mat.GetTextureOffset(m_textureName).y;
		}
		else
		{
			string message = $"Material {mat.name} doesn't have a texture property '_Turbulence'. Disabling component.";
			Debug.LogWarning(message);
			base.enabled = false;
		}
	}

	protected void OnEnable()
	{
		if (!m_ForceStartOffset)
		{
			offsetX = m_InitialOffsetX;
			offsetY = m_InitialOffsetY;
		}
		else
		{
			offsetX = m_ForcedOffset.x;
			offsetY = m_ForcedOffset.y;
		}
	}

	protected void Update()
	{
		if (base.isActiveAndEnabled)
		{
			offsetX += Time.deltaTime * xOffsetMulti;
			offsetY += Time.deltaTime * yOffsetMulti;
			offset.x = offsetX;
			offset.y = offsetY;
			mat.SetTextureOffset(textureName, offset);
		}
	}
}

}
