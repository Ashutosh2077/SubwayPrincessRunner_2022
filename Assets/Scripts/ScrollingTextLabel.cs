using System;
using System.Collections;
using UnityEngine;

public class ScrollingTextLabel : MonoBehaviour
{
	private void OnDisable()
	{
		this._isScrolling = false;
	}

	private void OnEnable()
	{
		this.label.enabled = false;
		this._labelTransform = this.label.transform;
		this._labelBaseColor = this.label.color;
		if (this._destroyOnDisable)
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	public void StartScrolling(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (duration <= 0f)
		{
			UnityEngine.Debug.LogError("Duration must be > 0", this);
		}
		else if (!this._isScrolling)
		{
			base.StartCoroutine(this.StartScrollingCoroutine(text, startLocalPos, endLocalPos, duration, fadeOutDuration, destroyWhenDone));
		}
	}

	private IEnumerator StartScrollingCoroutine(string text, Vector3 startLocalPos, Vector3 endLocalPos, float duration, float fadeOutDuration, bool destroyWhenDone)
	{
		if (this._isScrolling)
		{
			yield break;
		}
		this._isScrolling = true;
		this._destroyOnDisable = destroyWhenDone;
		float fadeOutAniFactorStart__0 = Mathf.Clamp01((duration - fadeOutDuration) / duration);
		this.label.enabled = true;
		this.label.text = text;
		this.label.color = this._labelBaseColor;
		startLocalPos.z = -1f;
		endLocalPos.z = -1f;
		float aniFactor__ = 0f;
		while (aniFactor__ < 1f)
		{
			aniFactor__ = Mathf.Clamp01(aniFactor__ + Time.deltaTime / duration);
			this._labelTransform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, aniFactor__);
			if (aniFactor__ >= fadeOutAniFactorStart__0)
			{
				float num = (aniFactor__ - fadeOutAniFactorStart__0) / (1f - fadeOutAniFactorStart__0);
				Color labelBaseColor = this._labelBaseColor;
				labelBaseColor.a *= 1f - num;
				this.label.color = labelBaseColor;
			}
			yield return null;
		}
		this.label.enabled = false;
		this._isScrolling = false;
		if (destroyWhenDone)
		{
			NGUITools.Destroy(base.gameObject);
		}
		yield break;
	}

	private bool _destroyOnDisable;

	private bool _isScrolling;

	private Color _labelBaseColor;

	private Transform _labelTransform;

	[SerializeField]
	private UILabel label;
}
