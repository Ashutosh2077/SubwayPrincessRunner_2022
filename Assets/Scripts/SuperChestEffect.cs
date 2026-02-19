using System;
using UnityEngine;

public class SuperChestEffect : MonoBehaviour
{
	private void Awake()
	{
		this.isRendering = false;
	}

	private void Start()
	{
		int i = 0;
		int num = this.particleSystems.Length;
		while (i < num)
		{
			var mainModule = this.particleSystems[i].main;
			mainModule.playOnAwake = false;
			i++;
		}
		this._glowMaterials = new Material[this.glowRenderers.Length];
		this._glowBaseColors = new Color[this.glowRenderers.Length];
		for (int j = 0; j < this.glowRenderers.Length; j++)
		{
			this._glowMaterials[j] = this.glowRenderers[j].material;
			this._glowBaseColors[j] = this._glowMaterials[j].GetColor(Shaders.Instance.TintColor);
		}
		this.ApplyGlowAniFactor();
	}

	private void ApplyGlowAniFactor()
	{
		float b = 0.5f + 0.5f * Mathf.Cos(3.141593f + 3.141593f * this._glowAniFactor);
		for (int i = 0; i < this._glowMaterials.Length; i++)
		{
			this._glowMaterials[i].SetColor(Shaders.Instance.TintColor, this._glowBaseColors[i] * b);
		}
	}

	public void FastForwardEffect(int time)
	{
		int i = 0;
		int num = this.particleSystems.Length;
		while (i < num)
		{
			this.particleSystems[i].Simulate((float)time);
			i++;
		}
	}

	public void SetVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public void StartEffect()
	{
		if (!this.isRendering)
		{
			this.isRendering = true;
			this._glowShouldBeOn = true;
			int i = 0;
			int num = this.particleSystems.Length;
			while (i < num)
			{
				this.particleSystems[i].Play();
				i++;
			}
		}
	}

	public void StopEffect()
	{
		this.isRendering = false;
		this._glowShouldBeOn = false;
		int i = 0;
		int num = this.particleSystems.Length;
		while (i < num)
		{
			this.particleSystems[i].Stop();
			i++;
		}
	}

	private void Update()
	{
		if (this._glowShouldBeOn && this._glowAniFactor < 1f)
		{
			this._glowAniFactor = Mathf.Clamp01(this._glowAniFactor + 0.5f * Time.deltaTime);
			this.ApplyGlowAniFactor();
		}
		else if (!this._glowShouldBeOn && this._glowAniFactor > 0f)
		{
			this._glowAniFactor = Mathf.Clamp01(this._glowAniFactor - 0.5f * Time.deltaTime);
			this.ApplyGlowAniFactor();
		}
	}

	public bool isRendering { get; set; }

	public ParticleSystem[] particleSystems = new ParticleSystem[0];

	public Renderer[] glowRenderers = new Renderer[0];

	private float _glowAniFactor;

	private Color[] _glowBaseColors;

	private Material[] _glowMaterials;

	private bool _glowShouldBeOn;
}
