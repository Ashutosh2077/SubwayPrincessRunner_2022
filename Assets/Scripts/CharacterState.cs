using System;
using System.Collections;
using UnityEngine;

public abstract class CharacterState : MonoBehaviour
{
	public virtual IEnumerator Begin()
	{
		yield return null;
		yield break;
	}

	public virtual void HandleCriticalHit(bool isShake = true)
	{
	}

	public virtual void HandleDoubleTap()
	{
	}

	public virtual void HandleSwipe(SwipeDir swipeDir)
	{
	}

	public virtual bool PauseActiveModifiers
	{
		get
		{
			return false;
		}
	}
}
