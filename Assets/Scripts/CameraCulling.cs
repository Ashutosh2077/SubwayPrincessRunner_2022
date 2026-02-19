using System;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
	public float TransparentFXCullingDistance
	{
		set
		{
			this.distances[1] = value;
			base.GetComponent<Camera>().layerCullDistances = this.distances;
		}
	}

	private void Start()
	{
		if (this.mat == null)
		{
			this.mat = new Material(this.fadeShader);
		}
		base.enabled = false;
	}

	private void Update()
	{
		if (this.state == CameraCulling.FadeState.FadeIn && this.factor < 1f)
		{
			this.factor += Time.deltaTime / this.fadeup;
			this.mat.SetFloat(Shaders.Instance.TintValue, (this.factor <= 1f) ? (1f - this.factor) : 0f);
			if (this.factor >= 1f)
			{
				this.factor = 0f;
				this.state = CameraCulling.FadeState.White;
			}
		}
		if (this.state == CameraCulling.FadeState.White)
		{
			if (this.factor < 1f)
			{
				this.factor += Time.deltaTime / this.ing;
			}
			else
			{
				this.factor = 0f;
				this.state = CameraCulling.FadeState.FadeOut;
			}
		}
		if (this.state == CameraCulling.FadeState.FadeOut)
		{
			if (this.factor < 1f)
			{
				this.factor += Time.deltaTime / this.fadedown;
				this.factor = ((this.factor <= 1f) ? this.factor : 1f);
				this.mat.SetFloat(Shaders.Instance.TintValue, this.factor);
			}
			else
			{
				this.factor = 0f;
				this.state = CameraCulling.FadeState.None;
			}
		}
		if (this.state == CameraCulling.FadeState.None)
		{
			base.enabled = false;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, this.mat);
	}

	private void SetToAndThenBack(Color color, float fadeup, float ing, float fadedown)
	{
		base.enabled = true;
		this.fadeup = fadeup;
		this.ing = ing;
		this.fadedown = fadedown;
		this.mat.SetColor(Shaders.Instance.Color, color);
		this.mat.SetFloat(Shaders.Instance.TintValue, 1f);
		this.factor = 0f;
		this.state = CameraCulling.FadeState.FadeIn;
		this.Update();
	}

	public FadeData SetToAndThenBack()
	{
		this.SetToAndThenBack(this.defaultFadeData.color, this.defaultFadeData.fadeInDuration, this.defaultFadeData.onDuration, this.defaultFadeData.fadeOutDuration);
		return this.defaultFadeData;
	}

	public void SetToAndThenBack(FadeData fadeData)
	{
		this.SetToAndThenBack(fadeData.color, fadeData.fadeInDuration, fadeData.onDuration, fadeData.fadeOutDuration);
	}

	private float[] distances = new float[32];

	private Material mat;

	[SerializeField]
	private Shader fadeShader;

	[SerializeField]
	private FadeData defaultFadeData;

	private CameraCulling.FadeState state;

	private float fadeup = 0.3f;

	private float ing = 0.3f;

	private float fadedown = 0.3f;

	private float factor;

	private enum FadeState
	{
		FadeIn,
		FadeOut,
		White,
		None
	}
}
