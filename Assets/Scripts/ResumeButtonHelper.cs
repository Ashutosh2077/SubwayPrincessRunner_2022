using System;
using System.Collections;
using UnityEngine;

public class ResumeButtonHelper : MonoBehaviour
{
	public void DisableButton()
	{
		if (this.buttonEnabled)
		{
			this.buttonEnabled = false;
		}
	}

	public void EnableButton()
	{
		if (!this.buttonEnabled)
		{
			this.buttonEnabled = true;
		}
	}

	private IEnumerator EnableButtonWhenReady()
	{
		float startTime = Time.realtimeSinceStartup;
		float timeWaited = 0f;
		while (timeWaited < 1f)
		{
			timeWaited = Time.realtimeSinceStartup - startTime;
			yield return new WaitForEndOfFrame();
		}
		this.EnableButton();
		yield break;
	}

	private void OnApplicationPause(bool pause)
	{
		this.DisableButton();
		if (!pause)
		{
			base.StartCoroutine(this.EnableButtonWhenReady());
		}
	}

	public bool isButtonEnabled
	{
		get
		{
			return this.buttonEnabled;
		}
	}

	private UIButtonOverlayOff overlayHelper
	{
		get
		{
			if (this._cachedOverlayHelper == null)
			{
				this._cachedOverlayHelper = base.gameObject.GetComponent<UIButtonOverlayOff>();
			}
			return this._cachedOverlayHelper;
		}
	}

	private UIButtonOverlayOff _cachedOverlayHelper;

	private bool buttonEnabled = true;
}
