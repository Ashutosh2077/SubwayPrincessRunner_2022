using System;
using System.Collections;
using UnityEngine;

public class SlideinPowerupButton : UIButtonSender
{
	private IEnumerator AnimateColor(UISprite sprite, Color32 startValue, Color32 endValue, float speedFactor)
	{
		float Factor = 0f;
		while (Factor < 1f)
		{
			Factor += Time.deltaTime * speedFactor;
			sprite.color = Color.Lerp(startValue, endValue, Factor);
			yield return null;
		}
		yield break;
	}

	public void UpdateAmount()
	{
		this._amount.text = PlayerInfo.Instance.GetUpgradeAmount(this.propType).ToString();
	}

	public void InitSlideinButton(SlideinPowerupHelper helper, int index, PropType type, Vector3 pos)
	{
		if (this.hasInit)
		{
			return;
		}
		this.hasInit = true;
		this._helper = helper;
		this.index = index;
		this.propType = type;
		base.transform.localPosition = pos;
		this.UpdateAmount();
		base.GetComponent<Collider>().enabled = false;
	}

	private void OnDisable()
	{
		this.ResetPowerupButtonColor();
	}

	public void OnPressButton()
	{
		if (!Game.Instance.isPaused)
		{
			this.SetPowerupColor(true);
		}
	}

	public void OnReleaseButton()
	{
		if (!Game.Instance.isPaused)
		{
			this.SetPowerupColor(false);
		}
	}

	public void ResetPowerupButtonColor()
	{
		this._background.color = this.INACTIVE_BUTTON_BACKGROUND_COLOR;
	}

	protected override void Send()
	{
		Game.Instance.wasButtonClicked = true;
		if (!this.hasInit)
		{
			UnityEngine.Debug.LogError(base.name + " was used without being initialized", this);
		}
		else
		{
			this._helper.SlideinPowerupClicked(this.index);
		}
	}

	private void SetPowerupColor(bool animateIn)
	{
		if (animateIn)
		{
			this._background.color = this.ACTIVE_BUTTON_BACKGROUND_COLOR;
		}
		else
		{
			base.StartCoroutine(this.AnimateColor(this._background, this.ACTIVE_BUTTON_BACKGROUND_COLOR, this.INACTIVE_BUTTON_BACKGROUND_COLOR, 2f));
		}
	}

	public void Show(Vector3 off, Vector3 on)
	{
		base.GetComponent<Collider>().enabled = true;
		this.ResetPowerupButtonColor();
		base.transform.localPosition = off;
		SpringPosition.Begin(base.gameObject, on, 10f);
	}

	public void Hide(bool instant, Vector3 off)
	{
		base.GetComponent<Collider>().enabled = false;
		if (instant)
		{
			base.transform.localPosition = off;
		}
		else
		{
			SpringPosition.Begin(base.gameObject, off, 10f);
		}
	}

	[SerializeField]
	private GameObject _model;

	[SerializeField]
	private UISprite _background;

	[SerializeField]
	private UILabel _amount;

	private int index;

	private PropType propType;

	private SlideinPowerupHelper _helper;

	private bool hasInit;

	public Color32 INACTIVE_BUTTON_BACKGROUND_COLOR = new Color32(0, 0, 0, 128);

	public Color32 ACTIVE_BUTTON_BACKGROUND_COLOR = new Color32(253, 225, 15, 229);
}
