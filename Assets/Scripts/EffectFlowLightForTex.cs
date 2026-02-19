using System;
using UnityEngine;

public class EffectFlowLightForTex : MonoBehaviour
{
	private void Awake()
	{
		UITexture component = base.gameObject.GetComponent<UITexture>();
		if (component != null)
		{
			UITexture uitexture = component;
			uitexture.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(uitexture.onRender, new UIDrawCall.OnRenderCallback(this.UpdateMaterial));
			component.material = this.mCurMaterial;
		}
		else
		{
			UISprite component2 = base.gameObject.GetComponent<UISprite>();
			if (component2 != null)
			{
				UISprite uisprite = component2;
				uisprite.onRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(uisprite.onRender, new UIDrawCall.OnRenderCallback(this.UpdateMaterial));
			}
			component2.mGlitterMaterial = new Material(this.mCurMaterial);
		}
		this.mUvAdd = 0f;
		this.mIsPlaying = true;
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

	public Material mCurMaterial;

	private float mUvAdd;

	private bool mIsPlaying;
}
