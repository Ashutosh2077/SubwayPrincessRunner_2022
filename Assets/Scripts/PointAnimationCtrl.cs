using System;
using UnityEngine;

public class PointAnimationCtrl : MonoBehaviour
{
	public void AddClip(AnimationClip clip)
	{
		if (this.anim[clip.name] == null)
		{
			this.anim.AddClip(clip, clip.name);
		}
		if (this.follow != null && this.follow[clip.name] == null)
		{
			this.follow.AddClip(clip, clip.name);
		}
	}

	public void RemoveClip(AnimationClip clip)
	{
		if (this.anim[clip.name] != null)
		{
			this.anim.RemoveClip(clip);
		}
		if (this.follow != null && this.follow[clip.name] == null)
		{
			this.follow.RemoveClip(clip);
		}
	}

	public void CrossFade(string clip, float time)
	{
		this.anim.CrossFade(clip, time);
		if (this.follow != null)
		{
			this.follow.CrossFade(clip, time);
		}
	}

	public void CrossFadeQueued(string clip, float time)
	{
		this.anim.CrossFadeQueued(clip, time);
		if (this.follow != null)
		{
			this.follow.CrossFadeQueued(clip, time);
		}
	}

	public void Play(string clip)
	{
		this.anim.Play(clip);
		if (this.follow != null)
		{
			this.follow.Play(clip);
		}
	}

	public bool IsEnable(string clip)
	{
		return this.anim[clip].enabled;
	}

	public void SetSpeed(string clip, float speed)
	{
		this.anim[clip].speed = speed;
		if (this.follow != null)
		{
			this.follow[clip].speed = speed;
		}
	}

	public void Stop()
	{
		this.anim.Stop();
	}

	public void Sample(AnimationClip clip, int frame)
	{
		string name = clip.name;
		AnimationState animationState = this.anim[name];
		animationState.enabled = true;
		animationState.speed = 0f;
		animationState.normalizedTime = (float)frame / (clip.frameRate * clip.length);
		this.anim.Sample();
	}

	public AnimationState this[string clip]
	{
		get
		{
			return this.anim[clip];
		}
	}

	public Animation anim;

	public Animation follow;
}
