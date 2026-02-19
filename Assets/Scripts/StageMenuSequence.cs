using System;
using System.Collections;
using UnityEngine;

public class StageMenuSequence : MonoBehaviour
{
	public virtual void StartPlayIdleRummagesAnimation()
	{
		this.animationRoutine = this.Play();
		this.animationRoutine.MoveNext();
	}

	protected virtual IEnumerator Play()
	{
		yield return null;
		yield break;
	}

	public virtual void StopPlayIdleRummagesAnimation()
	{
		this.isPlay = false;
		this.animationTime = 0f;
	}

	public void OnUpdate()
	{
		if (this.animationRoutine == null)
		{
			return;
		}
		this.animationRoutine.MoveNext();
	}

	public void OnNewGameStart()
	{
		this.isPlay = false;
		this.animationTime = 0f;
	}

	protected float animationTime;

	public Animation anim;

	protected IEnumerator animationRoutine;

	protected bool isPlay;
}
