using System;
using UnityEngine;

public class EffectFlowLoightForMesh : MonoBehaviour
{
	private void Awake()
	{
		if (this.render != null)
		{
			this.render.material = new Material(this.mCurMaterial);
		}
		this.mUvAdd = 0f;
		this.mIsPlaying = true;
	}

	private void Update()
	{
		this.UpdateMaterial(this.render.material);
	}

	private void UpdateMaterial(Material mat)
	{
		if (this.mIsPlaying)
		{
			this.mUvAdd += this.mUvSpeed;
			mat.SetFloat(Shaders.Instance.FlowLightOffset, this.mUvStart + this.mUvAdd);
			mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 1f);
			if (this.mUvAdd >= this.mUvXMax)
			{
				this.mIsPlaying = false;
				mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 0f);
				base.StartCoroutine(DelayInvoke.start(delegate
				{
					this.PlayOnceAgain();
				}, this.mTimeInteval));
			}
		}
		else
		{
			mat.SetFloat(Shaders.Instance.IsOpenFlowLight, 0f);
		}
	}

	private void PlayOnceAgain()
	{
		this.mUvAdd = 0f;
		this.mIsPlaying = true;
	}

	public float mUvStart;

	public float mUvSpeed = 0.02f;

	public float mUvXMax = 0.9f;

	public float mTimeInteval = 3f;

	public MeshRenderer render;

	public Material mCurMaterial;

	private float mUvAdd;

	private bool mIsPlaying;
}
