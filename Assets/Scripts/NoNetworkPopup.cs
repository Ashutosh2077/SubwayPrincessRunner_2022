using System;
using System.Collections;
using UnityEngine;

public class NoNetworkPopup : UIBaseScreen
{
	public override void Show()
	{
		base.Show();
		base.StartCoroutine(this.ShowPopup());
	}

	public override void Hide()
	{
		base.StartCoroutine(this.HidePopup());
	}

	public override void Init()
	{
		base.Init();
		if (this.tween == null)
		{
			this.tween = base.GetComponentInChildren<TweenAlpha>();
		}
		this.tween.GetComponent<UISprite>().alpha = this.tween.from;
	}

	private IEnumerator ShowPopup()
	{
		this.tween.PlayForward();
		float t = Time.realtimeSinceStartup;
		float start = Time.realtimeSinceStartup;
		while (t < start + this.tween.duration)
		{
			this.background.alpha = Mathf.Lerp(0.1f, 0.8f, (t - start) / this.tween.duration);
			yield return null;
			t = Time.realtimeSinceStartup;
		}
		for (t = Time.realtimeSinceStartup; t < start + this.duartion; t = Time.realtimeSinceStartup)
		{
			yield return null;
		}
		if (UIScreenController.Instance)
		{
			UIScreenController.Instance.ClosePopup(null);
		}
		yield break;
	}

	private IEnumerator HidePopup()
	{
		float t = Time.realtimeSinceStartup;
		float start = Time.realtimeSinceStartup;
		while (t < start + this.tween.duration)
		{
			this.background.alpha = Mathf.Lerp(0.8f, 0.1f, (t - start) / this.tween.duration);
			yield return null;
			t = Time.realtimeSinceStartup;
		}
		this.tween.Stop();
		base.Hide();
		yield break;
	}

	public TweenAlpha tween;

	public UISprite background;

	public float duartion = 1.5f;
}
