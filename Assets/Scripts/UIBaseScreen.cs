using System;
using UnityEngine;

public class UIBaseScreen : MonoBehaviour
{
	public virtual void GainFocus()
	{
	}

	public virtual void Hide()
	{
		base.gameObject.SetActive(false);
		this._isActive = false;
	}

	public virtual void Init()
	{
		for (int i = 0; i < this.dynamicallyLoadedElements.Length; i++)
		{
			NGUITools.AddChild(base.gameObject, this.dynamicallyLoadedElements[i]);
		}
	}

	protected virtual void InitFooter()
	{
		if (this.FooterPrefab != null)
		{
			this._footerHandler = NGUITools.AddChild(base.gameObject, this.FooterPrefab).GetComponent<UIFooterHandler>();
			this._footerHandler.OnButtonClick(this.selectedFooterButton);
			if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
			{
				this._footerHandler.gameObject.GetComponent<UIAnchor>().pixelOffset.y = 102f;
			}
			else
			{
				this._footerHandler.gameObject.GetComponent<UIAnchor>().pixelOffset.y = 0f;
			}
		}
	}

	public GameObject InitializeCoinbox(bool fundsEnabled, bool coinsEnabled, bool keysEnabled, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
	{
		GameObject prefab = Resources.Load("Prefabs/DynamicLoad/CoinboxQuick") as GameObject;
		GameObject gameObject = NGUITools.AddChild(base.gameObject, prefab);
		gameObject.GetComponentInChildren<CoinBoxSizer>().Init(fundsEnabled, coinsEnabled, keysEnabled, xOffset, yOffset, zOffset);
		if (UIScreenController.Instance.curDeviceType == UIScreenController.DeviceType.iPhoneX)
		{
			gameObject.GetComponent<UIAnchor>().pixelOffset.y = -132f;
		}
		else
		{
			gameObject.GetComponent<UIAnchor>().pixelOffset.y = 0f;
		}
		return gameObject;
	}

	public static bool IsOutOfProportion()
	{
		return (float)Screen.height * 1f / ((float)Screen.width * 1f) > 1.77777779f;
	}

	public virtual void LooseFocus()
	{
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(true);
		this._isActive = true;
		if (!this._footerInited)
		{
			this.InitFooter();
			this._footerInited = true;
		}
	}

	public bool isActive
	{
		get
		{
			return this._isActive;
		}
	}

	public string parentScreen { get; set; }

	[SerializeField]
	protected GameObject[] dynamicallyLoadedElements;

	[OptionalField]
	[SerializeField]
	protected GameObject FooterPrefab;

	[SerializeField]
	protected int selectedFooterButton;

	protected UIFooterHandler _footerHandler;

	private bool _footerInited;

	private bool _isActive;
}
