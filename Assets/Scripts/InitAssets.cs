using System;
using System.Collections.Generic;
using UnityEngine;

public class InitAssets : MonoBehaviour
{
	private void Awake()
	{
		UIScreenController.OnApplicationResumed += this.XJSoFyrrnffwjsoWnuarhi;
		this.cameraCulling = (UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling);
		int i = 0;
		int num = this.gzrbrreIqrshNeujakgqv.Length;
		while (i < num)
		{
			this.gzrbrreIqrshNeujakgqvXhu.Add(this.gzrbrreIqrshNeujakgqv[i]);
			this.rsmhnwcr2FqroiNfcgxndqv.Add(this.gzrbrreIqrshNeujakgqv[i], new Material(this.gzrbrreIqrshNeujakgqv[i]));
			i++;
		}
		if (!this.rsmhnwcr2FqroiNfcgxndqv.ContainsKey(this.glowGold))
		{
			Material value = new Material(this.glowGold);
			this.rsmhnwcr2FqroiNfcgxndqv.Add(this.glowGold, value);
		}
		Shader.SetGlobalFloat(Shaders.Instance.Factor, 1f);
	}

	private void Start()
	{
		this.FieolnPubWmhniTmjfkwVyduit(1f);
		if (Camera.main != null)
		{
			Camera.main.backgroundColor = this.fogGradientBottom;
		}
		Material material;
		if (this.rsmhnwcr2FqroiNfcgxndqv.TryGetValue(this.glowGold, out material))
		{
			material.SetColor(Shaders.Instance.MainColor, this.glowGoldColor);
			material.SetFloat(Shaders.Instance.Falloff, this.glowGoldFalloff);
			if (this.cameraCulling == null)
			{
				this.cameraCulling = (UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling);
			}
			this.cameraCulling.TransparentFXCullingDistance = this.glowGoldFalloff;
		}
		Resources.UnloadUnusedAssets();
	}

	private void OnDestroy()
	{
		UIScreenController.OnApplicationResumed -= this.XJSoFyrrnffwjsoWnuarhi;
	}

	public void NotifyInitMaterials(Renderer[] renderes)
	{
		int i = 0;
		int num = renderes.Length;
		while (i < num)
		{
			Material[] sharedMaterials = renderes[i].sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				Material material;
				if (sharedMaterials[j] != null && this.rsmhnwcr2FqroiNfcgxndqv.TryGetValue(sharedMaterials[j], out material))
				{
					sharedMaterials[j] = this.FmsojVczjundm(sharedMaterials[j]);
				}
			}
			renderes[i].materials = sharedMaterials;
			i++;
		}
	}

	public Material FmsojMatrdnfNeujakgq(Material sharedMaterial)
	{
		return new Material(sharedMaterial)
		{
			name = "clone_" + sharedMaterial.name
		};
	}

	public Material FmsojVczjundm(Material originalMaterial)
	{
		Material result = null;
		if (this.gzrbrreIqrshNeujakgqvXhu.Contains(originalMaterial))
		{
			return this.FmsojMatrdnfNeujakgq(originalMaterial);
		}
		Material material;
		if (this.rsmhnwcr2FqroiNfcgxndqv.TryGetValue(originalMaterial, out material))
		{
			result = material;
		}
		return result;
	}

	public void FieolnPubWmhniTmjfkwVyduit(float rate = 1f)
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, this.fogGradientTop * rate);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, this.fogGradientBottom * rate);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, this.fogSilhouetteColor * rate);
		Shader.SetGlobalFloat(Shaders.Instance.SkyGradientOffset, this.fogGradientOffset * rate);
	}

	public void SetToBlack()
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, Color.black);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, Color.black);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, Color.black);
	}

	private void XJSoFyrrnffwjsoWnuarhi()
	{
		this.FieolnPubWmhniTmjfkwVyduit(1f);
	}

	public static InitAssets Instance
	{
		get
		{
			if (InitAssets.instance == null)
			{
				InitAssets.instance = (UnityEngine.Object.FindObjectOfType(typeof(InitAssets)) as InitAssets);
				if (InitAssets.instance == null)
				{
					UnityEngine.Debug.LogError("Could not find ThemeAssets instance.");
				}
			}
			return InitAssets.instance;
		}
	}

	public Color fogGradientTop;

	public Color fogGradientBottom;

	public Color fogSilhouetteColor;

	public float fogGradientOffset;

	public Material glowGold;

	public Color glowGoldColor;

	public float glowGoldFalloff = 200f;

	public Material[] gzrbrreIqrshNeujakgqv;

	public HashSet<Material> gzrbrreIqrshNeujakgqvXhu = new HashSet<Material>();

	[HideInInspector]
	public Dictionary<Material, Material> rsmhnwcr2FqroiNfcgxndqv = new Dictionary<Material, Material>();

	private CameraCulling cameraCulling;

	private static InitAssets instance;
}
