using System;
using UnityEngine;

public class UIPowerupHelper : MonoBehaviour
{
	private void Awake()
	{
		this.originalColor = this.ingameScreen.MULTIPLIER_LABEL_ORIGINAL_COLOR;
	}

	private void FadingToWhiteToGreenToDarkGreen(bool sendInverse)
	{
		if (sendInverse)
		{
			if (this.lerpTime <= 1f)
			{
				this.currentFadeTarget = UIPowerupHelper.FadeTarget.green;
				if (this.currentFadeTarget != this.oldFadeTarget)
				{
					this.storedColor = this.ingameScreen.multiplierLabel.color;
				}
				this.ingameScreen.multiplierLabel.color = Color.Lerp(this.storedColor, this.colorTargets[(int)this.currentFadeTarget], this.lerpTime);
			}
			else if (this.lerpTime <= 2f)
			{
				this.currentFadeTarget = UIPowerupHelper.FadeTarget.white;
				if (this.currentFadeTarget != this.oldFadeTarget)
				{
					this.storedColor = this.ingameScreen.multiplierLabel.color;
				}
				this.ingameScreen.multiplierLabel.color = Color.Lerp(this.storedColor, this.colorTargets[(int)this.currentFadeTarget], this.lerpTime - 1f);
			}
			else
			{
				this.currentFadeTarget = UIPowerupHelper.FadeTarget.none;
				this.inverse = false;
				this.lerpTime = 0f;
			}
		}
		else if (this.lerpTime <= 1f)
		{
			this.currentFadeTarget = UIPowerupHelper.FadeTarget.green;
			if (this.currentFadeTarget != this.oldFadeTarget)
			{
				this.storedColor = this.ingameScreen.multiplierLabel.color;
			}
			this.ingameScreen.multiplierLabel.color = Color.Lerp(this.storedColor, this.colorTargets[(int)this.currentFadeTarget], this.lerpTime);
		}
		else if (this.lerpTime <= 2f)
		{
			this.currentFadeTarget = UIPowerupHelper.FadeTarget.darkgreen;
			if (this.currentFadeTarget != this.oldFadeTarget)
			{
				this.storedColor = this.ingameScreen.multiplierLabel.color;
			}
			this.ingameScreen.multiplierLabel.color = Color.Lerp(this.storedColor, this.colorTargets[(int)this.currentFadeTarget], this.lerpTime - 1f);
		}
		else
		{
			this.currentFadeTarget = UIPowerupHelper.FadeTarget.none;
			this.inverse = true;
			this.lerpTime = 0f;
		}
		this.lerpTime += Time.deltaTime * 8.2343f;
		this.oldFadeTarget = this.currentFadeTarget;
	}

	public float getSliderHeight()
	{
		return (float)this.iconBG.height;
	}

	public int getSliderWidth()
	{
		return this.sliderBG.width + this.iconBG.width / 2;
	}

	public int getHalfIconBGWidth()
	{
		return this.iconBG.width / 2;
	}

	public void HidePowerupSlot()
	{
		if (this._powerup != null && this._powerup.type == PropType.doubleMultiplier)
		{
			this.ReturnToNormal();
		}
	}

	private void OnDisable()
	{
		this.ReturnToNormal();
	}

	private void OnEnable()
	{
		this.sliderSteps = (float)((int)this.slider.foregroundWidget.localSize.x);
	}

	private void ReturnToNormal()
	{
		this.lerpTime = 1f;
		this.inverse = false;
		if (GameStats.Instance.scoreBooster5Activated)
		{
			this.ingameScreen.multiplierLabel.color = this.ingameScreen.SCOREBOOSTER_ACTIVE_COLOR;
		}
		else if (this.ingameScreen.multiplierLabel.color != this.originalColor)
		{
			this.ingameScreen.multiplierLabel.color = this.originalColor;
		}
	}

	public void setContainerPosition(float x, float y, float z)
	{
		this.container.transform.localPosition = new Vector3(x, y, z);
	}

	public void SetPowerupSlot(ActiveProp powerup)
	{
		this._powerup = powerup;
		Upgrade upgrade = Upgrades.upgrades[powerup.type];
		this.icon.spriteName = upgrade.iconName;
		float num = powerup.timeLeft / PlayerInfo.Instance.GetPowerupDuration(powerup.type);
		if (this.sliderSteps != 0f)
		{
			this.slider.value = num * this.sliderSteps / this.sliderSteps;
		}
		else
		{
			this.sliderSteps = this.slider.foregroundWidget.localSize.x;
		}
		if (powerup.type == PropType.doubleMultiplier)
		{
			this.FadingToWhiteToGreenToDarkGreen(this.inverse);
		}
		if (powerup.timeLeft < 0f)
		{
			if (this.slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(this.slider.gameObject, false);
			}
			this.icon.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * 3.141593f * 4f));
			this.iconBG.color = Color.Lerp(Color.grey, Color.white, 0.5f + 0.5f * Mathf.Cos(powerup.timeLeft * 3.141593f * 4f));
		}
		else
		{
			if (!this.slider.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(this.slider.gameObject, true);
			}
			this.icon.color = Color.white;
			this.iconBG.color = Color.white;
		}
	}

	public UIAnchor Anchor
	{
		get
		{
			return this.anchor;
		}
	}

	private IngameScreen ingameScreen
	{
		get
		{
			if (this._ingameScreen == null)
			{
				this._ingameScreen = (UnityEngine.Object.FindObjectOfType(typeof(IngameScreen)) as IngameScreen);
			}
			return this._ingameScreen;
		}
	}

	[SerializeField]
	private UISlider slider;

	[SerializeField]
	private UISprite sliderBG;

	[SerializeField]
	private UISprite icon;

	[SerializeField]
	private UISprite iconBG;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private UIAnchor anchor;

	private IngameScreen _ingameScreen;

	private ActiveProp _powerup;

	private Color32[] colorTargets = new Color32[]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(149, 192, 34, byte.MaxValue),
		new Color32(84, 118, 64, byte.MaxValue)
	};

	private UIPowerupHelper.FadeTarget currentFadeTarget = UIPowerupHelper.FadeTarget.none;

	private bool inverse;

	private float lerpTime = 1f;

	private UIPowerupHelper.FadeTarget oldFadeTarget = UIPowerupHelper.FadeTarget.none;

	private Color originalColor;

	private float sliderSteps;

	private Color storedColor;

	private enum FadeTarget
	{
		white,
		green,
		darkgreen,
		none
	}
}
