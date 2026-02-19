using System;
using UnityEngine;

public class MenuSlider : MonoBehaviour
{
	private void Awake()
	{
		this._menuMask.enabled = false;
		for (int i = 0; i < this.tweens.Length; i++)
		{
			MenuSlider.MenuTween menuTween = this.tweens[i];
			menuTween.showLocalPos = menuTween.tween.localPosition;
			menuTween.hideLocalPos = Vector3.zero;
		}
		switch (this.direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			this.menuBgInitialValue = this.menuBg.height;
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			this.menuBgInitialValue = this.menuBg.width;
			break;
		}
	}

	private void Start()
	{
		if (!PlayerInfo.Instance.menuSliderShow)
		{
			this.TweenIn(1f);
		}
		else
		{
			this.TweenOut(1f);
		}
	}

	private void OnDisable()
	{
		if (!PlayerInfo.Instance.menuSliderShow)
		{
			this.TweenIn(1f);
		}
		else
		{
			this.TweenOut(1f);
		}
	}

	public void Show()
	{
		if (PlayerInfo.Instance.menuSliderShow)
		{
			this._menuMask.enabled = true;
			base.StartCoroutine(myTween.To(this.duration, new Action<float>(this.TweenIn)));
			AudioPlayer.Instance.PlaySound("chilun", true);
			PlayerInfo.Instance.menuSliderShow = false;
		}
		else
		{
			this._menuMask.enabled = true;
			base.StartCoroutine(myTween.To(this.duration, new Action<float>(this.TweenOut)));
			AudioPlayer.Instance.PlaySound("chilun", true);
			PlayerInfo.Instance.menuSliderShow = true;
		}
	}

	private void TweenIn(float t)
	{
		for (int i = 0; i < this.tweens.Length; i++)
		{
			MenuSlider.MenuTween menuTween = this.tweens[i];
			menuTween.tween.localPosition = Vector3.Lerp(menuTween.showLocalPos, menuTween.hideLocalPos, t);
		}
		switch (this.direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			this.menuBg.height = (int)Mathf.Lerp((float)this.menuBgInitialValue, this.menuBgHideValue, t);
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			this.menuBg.width = (int)Mathf.Lerp((float)this.menuBgInitialValue, this.menuBgHideValue, t);
			break;
		}
		this.menuArrow.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(180f, 0f, t));
		if (t >= 1f)
		{
			this._menuMask.enabled = false;
			if (this.menuTip != null && !this.menuTip.activeInHierarchy && PlayerInfo.Instance.GetAllAchievementAward())
			{
				this.menuTip.SetActive(true);
			}
		}
	}

	private void TweenOut(float t)
	{
		for (int i = 0; i < this.tweens.Length; i++)
		{
			MenuSlider.MenuTween menuTween = this.tweens[i];
			menuTween.tween.localPosition = Vector3.Lerp(menuTween.hideLocalPos, menuTween.showLocalPos, t);
		}
		switch (this.direction)
		{
		case ScrollDirection.Up:
		case ScrollDirection.Down:
			this.menuBg.height = (int)Mathf.Lerp(this.menuBgHideValue, (float)this.menuBgInitialValue, t);
			break;
		case ScrollDirection.Left:
		case ScrollDirection.Right:
			this.menuBg.width = (int)Mathf.Lerp(this.menuBgHideValue, (float)this.menuBgInitialValue, t);
			break;
		}
		this.menuArrow.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 180f, t));
		if (t >= 1f)
		{
			this._menuMask.enabled = false;
			if (this.menuTip != null && this.menuTip.activeInHierarchy)
			{
				this.menuTip.SetActive(false);
			}
		}
	}

	[SerializeField]
	private ScrollDirection direction = ScrollDirection.Down;

	[SerializeField]
	private UISprite menuBg;

	[SerializeField]
	private UISprite menuArrow;

	[SerializeField]
	private BoxCollider _menuMask;

	[SerializeField]
	private GameObject menuTip;

	[SerializeField]
	private MenuSlider.MenuTween[] tweens;

	[SerializeField]
	private float menuBgHideValue = 2f;

	[SerializeField]
	private float duration = 2f;

	private int menuBgInitialValue;

	[Serializable]
	public class MenuTween
	{
		public Transform tween;

		public Vector3 showLocalPos;

		public Vector3 hideLocalPos;
	}
}
