using System;
using System.Collections;
using UnityEngine;

public class UIMessageHelper : MonoBehaviour
{
	private void Awake()
	{
		this._label = base.GetComponent<UILabel>();
		this._label.alpha = 0f;
		base.gameObject.SetActive(false);
	}

	public void DisableShowLabel()
	{
		this._wasPermanentlyHidden = (this._label.text != string.Empty);
		this._label.enabled = false;
	}

	private IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(2f);
		float duration = 0.2f;
		float fadeTime = 0f;
		Vector3 scaleFrom = this._label.transform.localScale;
		Vector3 scaleTo = new Vector3(scaleFrom.x, 0f, scaleFrom.z);
		while (fadeTime < 1f)
		{
			fadeTime += Time.deltaTime / duration;
			this._label.transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, fadeTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		this._label.text = string.Empty;
		this._label.transform.localScale = scaleFrom;
		base.gameObject.SetActive(false);
		UISliderInController.Instance.ReadyForNextMessage();
		yield break;
	}

	public void SetTemporaryHidden(bool hidden)
	{
		this._label.enabled = (!hidden && !this._wasPermanentlyHidden);
	}

	public void ShowMessage(string message, bool bigText = false)
	{
		this._wasPermanentlyHidden = false;
		base.gameObject.SetActive(true);
		this._label.enabled = true;
		this._label.text = message;
		this._label.color = this.shownColor;
		if (bigText)
		{
			this._label.fontSize = 50;
		}
		else
		{
			this._label.fontSize = 40;
		}
		base.StartCoroutine("FadeOut");
	}

	private UILabel _label;

	private bool _wasPermanentlyHidden;

	public Color shownColor = Color.white;
}
