using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCtrl : MonoBehaviour
{
	private void Start()
	{
		this._animation = base.GetComponent<Animation>();
		this.state2clip.Add(AnimationCtrl.AniState.enter, this.enterName);
		this.state2clip.Add(AnimationCtrl.AniState.quit, this.quitName);
		this.state2clip.Add(AnimationCtrl.AniState.selected, this.selectName);
		this.state2clip.Add(AnimationCtrl.AniState.idle, this.idleName);
	}

	private void SetState(AnimationCtrl.AniState newstate)
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || this.IsInAnimation)
		{
			return;
		}
		string empty = string.Empty;
		if (this.state2clip.TryGetValue(newstate, out empty) && this._animation.GetClip(empty) != null)
		{
			this._animation.CrossFade(empty);
			if (this._animation[empty].wrapMode != WrapMode.Loop)
			{
				base.StartCoroutine(this.waitingForAni(this._animation[empty].length));
			}
		}
	}

	public void PlayEnterAni()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || this.IsInAnimation)
		{
			return;
		}
		this.SetState(AnimationCtrl.AniState.enter);
	}

	public void PlayQuitAni()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || this.IsInAnimation)
		{
			return;
		}
		this.SetState(AnimationCtrl.AniState.quit);
	}

	public void PlaySelectAni()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || this.IsInAnimation)
		{
			return;
		}
		this.SetState(AnimationCtrl.AniState.selected);
	}

	public void PlayIdleAni()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || this.IsInAnimation)
		{
			return;
		}
		this.SetState(AnimationCtrl.AniState.idle);
	}

	private IEnumerator waitingForAni(float time)
	{
		this.isInAnimation = true;
		yield return new WaitForSeconds(time);
		this.isInAnimation = false;
		yield break;
	}

	public bool IsInAnimation
	{
		get
		{
			return this.isInAnimation;
		}
	}

	public Animation _animation;

	public string idleName;

	public string enterName;

	public string selectName;

	public string quitName;

	private bool isInAnimation;

	private Dictionary<AnimationCtrl.AniState, string> state2clip = new Dictionary<AnimationCtrl.AniState, string>();

	public enum AniState
	{
		none,
		enter,
		idle,
		selected,
		quit
	}
}
