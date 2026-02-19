using System;
using UnityEngine;

public class OnlyOneTheme : MonoBehaviour
{
	private void Awake()
	{
		UIScreenController.OnApplicationResumed += this.UIScreenController_OnApplicationResumed;
		this.glowGold.SetColor(Shaders.Instance.MainColor, this.glowGoldColor);
		this.glowGold.SetFloat(Shaders.Instance.Falloff, this.glowGoldFalloff);
		if (this.cameraCulling == null)
		{
			this.cameraCulling = (UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling);
		}
		this.cameraCulling.TransparentFXCullingDistance = this.glowGoldFalloff;
		RenderSettings.fogColor = this.fogColor;
		this.SetShaderStates();
		if (Camera.main != null)
		{
			Camera.main.backgroundColor = this.fogGradientBottom;
		}
		Resources.UnloadUnusedAssets();
	}

	private void SetShaderStates()
	{
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientTopColor, this.fogGradientTop);
		Shader.SetGlobalColor(Shaders.Instance.SkyGradientBottomColor, this.fogGradientBottom);
		Shader.SetGlobalColor(Shaders.Instance.FogSilhouetteColor, this.fogSilhouetteColor);
		Shader.SetGlobalFloat(Shaders.Instance.SkyGradientOffset, this.fogGradientOffset);
	}

	private void UIScreenController_OnApplicationResumed()
	{
		this.SetShaderStates();
	}

	public Color fogColor;

	public Color fogGradientTop;

	public Color fogGradientBottom;

	public Color fogSilhouetteColor;

	public float fogGradientOffset;

	public Material glowGold;

	public Color glowGoldColor;

	public float glowGoldFalloff = 200f;

	private CameraCulling cameraCulling;

	private static OnlyOneTheme _instance;
}
