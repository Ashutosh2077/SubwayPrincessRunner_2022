using System;
using UnityEngine;

public class UIButtonOverlayOff : MonoBehaviour
{
	private void Awake()
	{
		if (!this._initDone)
		{
			this.Init();
		}
	}

	public void ButtonPressed(bool isPressed)
	{
		if (base.enabled)
		{
			if (this.overlay != null && this.buttonColorScheme.light != null)
			{
				Color32 value = this.buttonColorScheme.light.Value;
				TweenColor.Begin(this.overlay.gameObject, this.DURATION, isPressed ? new Color32(value.r, value.g, value.b, 0) : value);
			}
			if (isPressed)
			{
				byte r = (byte)Mathf.Clamp((int)(this.originalFillColor.r - 30), 0, 255);
				byte g = (byte)Mathf.Clamp((int)(this.originalFillColor.g - 30), 0, 255);
				byte b = (byte)Mathf.Clamp((int)(this.originalFillColor.b - 30), 0, 255);
				TweenColor.Begin(this.fillSprite.gameObject, this.DURATION, new Color32(r, g, b, this.originalFillColor.a));
			}
			else
			{
				TweenColor.Begin(this.fillSprite.gameObject, this.DURATION, this.originalFillColor);
			}
		}
	}

	public void GoToNormalState()
	{
		if (!this._buttonStateLocked)
		{
			if (base.GetComponent<BoxCollider>() != null)
			{
				base.GetComponent<BoxCollider>().enabled = true;
			}
			this.fillSprite.color = this.originalFillColor;
			if (this.buttonColorScheme.light != null)
			{
			}
			if (this.overlay != null)
			{
				this.overlay.enabled = true;
			}
		}
	}

	public void GoToNotAvailableState()
	{
		if (!this._buttonStateLocked)
		{
			if (base.GetComponent<BoxCollider>() != null)
			{
				base.GetComponent<BoxCollider>().enabled = false;
			}
			if (this.buttonColorScheme.unavailable != null)
			{
				this.fillSprite.color = this.buttonColorScheme.unavailable.Value;
			}
		}
	}

	public void GoToSelectedState()
	{
		if (!this._buttonStateLocked)
		{
			if (base.GetComponent<BoxCollider>() != null)
			{
				base.GetComponent<BoxCollider>().enabled = false;
			}
			if (this.buttonColorScheme.selected != null)
			{
				this.fillSprite.color = this.buttonColorScheme.selected.Value;
			}
			else
			{
				UnityEngine.Debug.LogWarning("No 'selected' color found for button type: " + this.buttonType.ToString(), base.gameObject);
			}
		}
	}

	protected void Init()
	{
		this._initDone = true;
		if (this.fillSprite == null)
		{
			this.fillSprite = this.fillWithPrimeColor.GetComponent<UIWidget>();
		}
		this.buttonColorScheme = GlobalColors.GetButtonColorScheme(this.buttonType, base.gameObject);
		if (this.buttonColorScheme.original != null)
		{
			this.fillSprite.color = this.buttonColorScheme.original.Value;
		}
		this.originalFillColor = this.fillSprite.color;
		if (this.overlay != null)
		{
			this.overlay.enabled = true;
			if (this.buttonColorScheme.light != null)
			{
				this.overlay.color = this.buttonColorScheme.light.Value;
			}
		}
	}

	public void LockState(bool locked)
	{
		this._buttonStateLocked = locked;
	}

	private void OnDisable()
	{
		if (this.fillSprite.color != this.originalFillColor)
		{
			this.GoToNormalState();
		}
	}

	protected virtual void OnPress(bool isPressed)
	{
		this.ButtonPressed(isPressed);
	}

	public void ResetTweens()
	{
		if (base.enabled)
		{
			if (this.overlay != null && this.buttonColorScheme.light != null)
			{
				TweenColor.Begin(this.overlay.gameObject, this.DURATION, this.buttonColorScheme.light.Value);
			}
			TweenColor.Begin(this.fillSprite.gameObject, this.DURATION, this.originalFillColor);
		}
	}

	public void SetButtonType(UIButtonOverlayOff.ButtonType type)
	{
		this.buttonType = type;
		this.Init();
		this.ButtonPressed(false);
	}

	public GameObject fillWithPrimeColor;

	public UISprite overlay;

	public UIButtonOverlayOff.ButtonType buttonType;

	private bool _buttonStateLocked;

	protected bool _initDone;

	private ButtonColorScheme buttonColorScheme;

	private float DURATION;

	private UIWidget fillSprite;

	private Color32 originalFillColor;

	public enum ButtonType
	{
		Custom,
		Primary,
		Tertiary,
		Footer_shop
	}
}
