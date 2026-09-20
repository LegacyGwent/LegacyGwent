// Original lightning behavior, with isolated namespace and explicit source shader.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.DynamicCards.SourceLightning
{

[Serializable]
public class LightningAnimator : MonoBehaviour
{
	public static class LightningRef
	{
		public static LightningAnimator lightningAnimator;
	}

	[Serializable]
	public class LightningAnimation
	{
		public float StartTime;

		public float AnimationLength = 0.5f;

		public AnimationCurve BuildUp = new AnimationCurve();

		public AnimationCurve Opacity = new AnimationCurve();

		public AnimationCurve Width = new AnimationCurve();

		public AnimationCurve TurbulenceSpeed = new AnimationCurve();

		public AnimationCurve TurbulenceStrength = new AnimationCurve();

		public Color TintColor = Color.white;

		public float WiggleSpeed = 10f;

		public float WiggleFrequency = 0.4f;

		public AnimationCurve WiggleStrength = new AnimationCurve();

		public bool UseSpeed;
	}

	[Serializable]
	public class MaterialAnimation
	{
		public Material WiggleMaterials;

		public string WigglePropertyNames;

		public float StartTime;

		public float AnimationLength = 0.5f;

		public float WiggleSpeed = 10f;

		public float WiggleFrequency = 0.4f;

		public AnimationCurve WiggleStrength = new AnimationCurve();

		public AnimationCurve Opacity = new AnimationCurve();
	}

	public LightningTool lightningTool;

	public int SectionNum = 1;

	public List<LightningAnimation> LightningAnimations = new List<LightningAnimation>();

	public List<MaterialAnimation> MaterialAnimations = new List<MaterialAnimation>();

	public List<bool> FoldOuts = new List<bool>();

	public List<bool> MaterialFoldOuts = new List<bool>();

	public float SequenceLength = 2f;

	public float Progress;

	private float StartTime;

	public bool UseRandomSections;

	private int CurrentSection;

	private int NextSection;

	private bool Playing;

	public bool Looping;

	public bool PlayOnAwake = true;

	private float FloatBuffer1;

	private float FloatBuffer2;

	private float FloatBuffer3;

	private Material MaterialBuffer;

	private Vector2 Vector2Buffer = default(Vector2);

	public static int SHADER_TINT_COLOR_PROPERTY = -1;

	public static int SHADER_STRENGHT_PROPERTY = -1;

	public static int SHADER_WIDTH_PROPERTY = -1;

	public static int SHADER_TURBULENCE_PROPERTY = -1;

	public static bool AreShaderPropertiesInitialized;

	private readonly List<Material> cardMaterials = new List<Material>();
    private void CreateCardMaterialInstances()
    {
        var replacements = new Dictionary<Material, Material>();
        var renderers = transform.root.GetComponentsInChildren<Renderer>(true);
        foreach (var animation in MaterialAnimations)
        {
            var original = animation.WiggleMaterials;
            if (original == null) continue;
            Material clone;
            if (!replacements.TryGetValue(original, out clone))
            {
                clone = new Material(original); replacements.Add(original, clone); cardMaterials.Add(clone);
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials; bool changed = false;
                    for (int i = 0; i < materials.Length; i++)
                        if (materials[i] == original) { materials[i] = clone; changed = true; }
                    if (changed) renderer.sharedMaterials = materials;
                }
            }
            animation.WiggleMaterials = clone;
        }
    }
    private void OnDestroy()
    {
        foreach (var material in cardMaterials)
            if (material != null) Destroy(material);
    }
    public void Awake()
	{
        if (Application.isPlaying) CreateCardMaterialInstances();
		if (!AreShaderPropertiesInitialized)
		{
			SHADER_TINT_COLOR_PROPERTY = Shader.PropertyToID("_TintColor");
			SHADER_STRENGHT_PROPERTY = Shader.PropertyToID("_Strength");
			SHADER_WIDTH_PROPERTY = Shader.PropertyToID("_Width");
			SHADER_TURBULENCE_PROPERTY = Shader.PropertyToID("_Turbulence");
			AreShaderPropertiesInitialized = true;
		}
		if (Application.isPlaying)
		{
			StartTime = Time.time;
		}
		else
		{
			StartTime = Time.realtimeSinceStartup;
		}
		if (PlayOnAwake)
		{
			Play();
		}
	}

	private void OnEnable()
	{
		if (PlayOnAwake)
		{
			Play();
		}
	}

	public void Update()
	{
		SequenceLength = Mathf.Clamp(SequenceLength, 1f, 120f);
		if (Playing)
		{
			if (Application.isPlaying)
			{
				Progress = Time.time - StartTime;
			}
			else
			{
				Progress = Time.realtimeSinceStartup - StartTime;
			}
			if (Looping)
			{
				if (UseRandomSections)
				{
					float num = SequenceLength / (float)SectionNum;
					if (Progress >= (float)CurrentSection * num + num)
					{
						if (SectionNum > 1)
						{
							do
							{
								NextSection = UnityEngine.Random.Range(0, SectionNum);
							}
							while (CurrentSection == NextSection);
							CurrentSection = NextSection;
						}
						if (Application.isPlaying)
						{
							StartTime = Time.time - (float)CurrentSection * num;
						}
						else
						{
							StartTime = Time.realtimeSinceStartup - (float)CurrentSection * num;
						}
					}
				}
				else if (Progress > SequenceLength)
				{
					if (Application.isPlaying)
					{
						StartTime = Time.time;
					}
					else
					{
						StartTime = Time.realtimeSinceStartup;
					}
				}
			}
			else if (!Looping && Progress > SequenceLength)
			{
				Pause();
				Progress = SequenceLength;
			}
		}
		if (lightningTool == null)
		{
			SetUp();
		}
		while (LightningAnimations.Count < lightningTool.Lightnings.Count)
		{
			LightningAnimation lightningAnimation = new LightningAnimation();
			lightningAnimation.BuildUp = AnimationCurveHelper.SetUpBuildUpCurve();
			lightningAnimation.Opacity = AnimationCurveHelper.SetUpLinearCurve();
			lightningAnimation.Width = AnimationCurveHelper.SetUpZeroCurve();
			lightningAnimation.TurbulenceSpeed = AnimationCurveHelper.SetUpLinearCurve();
			lightningAnimation.TurbulenceStrength = AnimationCurveHelper.SetUpLinearCurve();
			lightningAnimation.WiggleStrength = AnimationCurveHelper.SetUpZeroCurve();
			LightningAnimations.Add(lightningAnimation);
			FoldOuts.Add(item: false);
		}
		while (LightningAnimations.Count > lightningTool.Lightnings.Count)
		{
			LightningAnimations.RemoveAt(LightningAnimations.Count - 1);
			FoldOuts.RemoveAt(LightningAnimations.Count - 1);
		}
		int count = lightningTool.Lightnings.Count;
		for (int i = 0; i < count; i++)
		{
			LightningAnimation lightningAnimation2 = LightningAnimations[i];
			lightningAnimation2.WiggleFrequency = Mathf.Clamp(lightningAnimation2.WiggleFrequency, 0.1f, 100f);
			GameObject meshHolder = lightningTool.Lightnings[i].MeshHolder;
			if (meshHolder == null)
			{
				return;
			}
			FloatBuffer1 = (Progress - lightningAnimation2.StartTime) / (lightningAnimation2.AnimationLength - lightningAnimation2.StartTime);
			if (FloatBuffer1 < 0f || FloatBuffer1 > 1f)
			{
				meshHolder.SetActive(value: false);
			}
			else if (!meshHolder.activeSelf)
			{
				meshHolder.SetActive(value: true);
			}
			MaterialBuffer = meshHolder.GetComponent<MeshRenderer>().sharedMaterial;
			if (MaterialBuffer == null)
			{
				break;
			}
			FloatBuffer2 = lightningAnimation2.BuildUp.Evaluate(FloatBuffer1);
			MaterialBuffer.SetTextureOffset("_MainTex", new Vector2(0f, FloatBuffer2));
			FloatBuffer2 = lightningAnimation2.TurbulenceSpeed.Evaluate(FloatBuffer1);
			if (lightningAnimation2.UseSpeed)
			{
				Vector2Buffer = MaterialBuffer.GetTextureOffset("_Turbulence");
				if (Application.isPlaying)
				{
					float num2 = 20f * Time.deltaTime;
					Vector2Buffer.x += FloatBuffer2 * num2;
					Vector2Buffer.y += (0f - FloatBuffer2) * num2;
				}
				else
				{
					Vector2Buffer.x += FloatBuffer2;
					Vector2Buffer.y += 0f - FloatBuffer2;
				}
				MaterialBuffer.SetTextureOffset("_Turbulence", Vector2Buffer);
			}
			else
			{
				MaterialBuffer.SetTextureOffset("_Turbulence", new Vector2(FloatBuffer2, 0f - FloatBuffer2));
			}
			FloatBuffer2 = lightningAnimation2.TurbulenceStrength.Evaluate(FloatBuffer1);
			MaterialBuffer.SetFloat(SHADER_STRENGHT_PROPERTY, FloatBuffer2);
			FloatBuffer3 = lightningAnimation2.Width.Evaluate(FloatBuffer1);
			MaterialBuffer.SetFloat(SHADER_WIDTH_PROPERTY, FloatBuffer3);
			FloatBuffer2 = lightningAnimation2.Opacity.Evaluate(FloatBuffer1);
			FloatBuffer2 += Wiggle(lightningAnimation2.WiggleSpeed, lightningAnimation2.WiggleFrequency) * lightningAnimation2.WiggleStrength.Evaluate(FloatBuffer1);
			MaterialBuffer.SetColor(SHADER_TINT_COLOR_PROPERTY, new Color(FloatBuffer2, FloatBuffer2, FloatBuffer2, 1f));
			lightningTool.Lightnings[i].MeshHolder.GetComponent<MeshRenderer>().sharedMaterial = MaterialBuffer;
		}
		int count2 = MaterialAnimations.Count;
		for (int j = 0; j < count2; j++)
		{
			MaterialAnimation materialAnimation = MaterialAnimations[j];
			if (materialAnimation.WiggleMaterials == null)
			{
				break;
			}
			FloatBuffer1 = (Progress - materialAnimation.StartTime) / (materialAnimation.AnimationLength - materialAnimation.StartTime);
			FloatBuffer2 = materialAnimation.Opacity.Evaluate(FloatBuffer1);
			FloatBuffer2 += Wiggle(materialAnimation.WiggleSpeed, materialAnimation.WiggleFrequency) * materialAnimation.WiggleStrength.Evaluate(FloatBuffer1);
			materialAnimation.WiggleMaterials.SetColor(materialAnimation.WigglePropertyNames, new Color(FloatBuffer2, FloatBuffer2, FloatBuffer2, 1f));
		}
	}

	public void Play()
	{
		Playing = true;
		if (Application.isPlaying)
		{
			StartTime = Time.time;
		}
		else
		{
			StartTime = Time.realtimeSinceStartup;
		}
	}

	public void Pause()
	{
		Playing = false;
	}

	public float Wiggle(float _Speed, float _Frequency)
	{
		if (Application.isPlaying)
		{
			return Mathf.PerlinNoise(1f, Time.time / _Frequency * _Speed) * 2f - 1f;
		}
		return Mathf.PerlinNoise(1f, Time.realtimeSinceStartup / _Frequency * _Speed) * 2f - 1f;
	}

	public void SetUp()
	{
		lightningTool = GetComponent<LightningTool>();
		if (lightningTool == null)
		{
			lightningTool = base.gameObject.AddComponent<LightningTool>();
		}
	}

	public void AddWiggleMat()
	{
		MaterialAnimation materialAnimation = new MaterialAnimation();
		materialAnimation.WiggleMaterials = null;
		materialAnimation.WigglePropertyNames = "_TintColor";
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve = AnimationCurveHelper.SetUpZeroCurve();
		AnimationCurve animationCurve2 = new AnimationCurve();
		animationCurve2 = AnimationCurveHelper.SetUpLinearCurve();
		materialAnimation.Opacity = animationCurve2;
		materialAnimation.WiggleStrength = animationCurve;
		materialAnimation.WiggleSpeed = 10f;
		materialAnimation.WiggleFrequency = 0.4f;
		MaterialAnimations.Add(materialAnimation);
		MaterialFoldOuts.Add(item: false);
	}

	public void RemoveWiggleMat(int index)
	{
		MaterialAnimations.RemoveAt(index);
		MaterialFoldOuts.RemoveAt(index);
	}

	public void AddRandomSection()
	{
		SectionNum++;
	}

	public void RemoveRandomSection()
	{
		if (SectionNum > 1)
		{
			SectionNum--;
		}
	}
}

}
