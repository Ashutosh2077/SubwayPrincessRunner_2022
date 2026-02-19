using System;
using UnityEngine;

public class Shaders
{
	public static Shaders Instance
	{
		get
		{
			if (Shaders._instance == null)
			{
				Shaders._instance = new Shaders();
			}
			return Shaders._instance;
		}
	}

	public readonly int MainTex = Shader.PropertyToID("_MainTex");

	public readonly int ClipTex = Shader.PropertyToID("_ClipTex");

	public readonly int MainColor = Shader.PropertyToID("_MainColor");

	public readonly int Color = Shader.PropertyToID("_Color");

	public readonly int TintColor = Shader.PropertyToID("_TintColor");

	public readonly int Falloff = Shader.PropertyToID("_Falloff");

	public readonly int TintValue = Shader.PropertyToID("_TintValue");

	public readonly int Factor = Shader.PropertyToID("_Factor");

	public readonly int SkyGradientTopColor = Shader.PropertyToID("_SkyGradientTopColor");

	public readonly int SkyGradientBottomColor = Shader.PropertyToID("_SkyGradientBottomColor");

	public readonly int FogSilhouetteColor = Shader.PropertyToID("_FogSilhouetteColor");

	public readonly int SkyGradientOffset = Shader.PropertyToID("_SkyGradientOffset");

	public readonly int Fade = Shader.PropertyToID("_Fade");

	public readonly int ShiftColor = Shader.PropertyToID("_ShiftColor");

	public readonly int FlowLightOffset = Shader.PropertyToID("_FlowLightOffset");

	public readonly int IsOpenFlowLight = Shader.PropertyToID("_IsOpenFlowLight");

	public readonly int WidthRate = Shader.PropertyToID("_WidthRate");

	public readonly int HeightRate = Shader.PropertyToID("_HeightRate");

	public readonly int OffsetXRate = Shader.PropertyToID("_OffsetXRate");

	public readonly int OffsetYRate = Shader.PropertyToID("_OffsetYRate");

	public readonly int ClipSharpness = Shader.PropertyToID("_ClipSharpness");

	private static Shaders _instance;
}
