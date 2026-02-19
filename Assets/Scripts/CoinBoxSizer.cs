using System;
using UnityEngine;

public class CoinBoxSizer : MonoBehaviour
{
	private void _AdjustKeysSegmentSize()
	{
		if (this._keysParent.activeSelf)
		{
			float num = this.KEYS_FG_SCALE_OFFSET + this._cachedCoinIconOffset;
			float num2 = (float)this.keyAmountLabel.width;
			this._grayFGKeys.width = Mathf.RoundToInt(num2 + num);
		}
		this.PlaceSegments();
	}

	public void AdjustCoinsSegmentSize()
	{
		if (this._coinsParent.activeSelf)
		{
			float num = (float)this.coinAmountLabel.width;
			this._grayFGCoins.width = Mathf.RoundToInt(num + this.COINS_FG_SCALE_OFFSET);
		}
		this.PlaceSegments();
	}

	private void CacheCoinsSegmentElements()
	{
		if (this._coinsParent.activeSelf)
		{
			this._cachedCoinIconOffset = (float)this._coinIcon.width * 2f / 3f;
			this._cachedCoinIconWidth = (float)this._coinIcon.width;
			this._cachedCoinsFGWidth = (float)this._grayFGCoins.width;
		}
		else
		{
			this._cachedCoinIconOffset = (float)this._coinIcon.width * 1f / 3f;
			this._cachedCoinIconWidth = 0f;
			this._cachedCoinsFGWidth = 0f;
		}
	}

	private void CacheKeysSegmentElements()
	{
		this._cachedKeysIconWidth = (float)this._keyIcon.width;
		if (this._keysParent.activeSelf)
		{
			this._cachedKeysFGWidth = (float)this._grayFGKeys.width;
		}
		else
		{
			this._cachedKeysFGWidth = 0f;
		}
	}

	private Vector3 CalculateCoinboxPosition(UIWidget widgetAnchor, Vector3 defaultPosition, float xOffset_custom, float yOffset_custom)
	{
		float num = defaultPosition.x + xOffset_custom;
		float num2 = defaultPosition.y + yOffset_custom;
		float z = defaultPosition.z;
		Vector2 vector = Vector2.zero;
		if (widgetAnchor != null)
		{
			vector = this.CalculateDistanceToBounds(widgetAnchor);
		}
		return new Vector3(vector.x + num, vector.y + num2, z);
	}

	private Vector2 CalculateDistanceToBounds(UIWidget widget)
	{
		float x = (float)widget.width / 2f;
		return new Vector2(x, (float)widget.height / 2f);
	}

	private void EnableElements(bool fundsEnabled, bool coinsEnabled, bool keysEnabled)
	{
		this._coinsParent.SetActive(coinsEnabled);
		this._keysParent.SetActive(keysEnabled);
	}

	public void Init(bool fundsEnabled, bool coinsEnabled, bool keysEnabled, float xOffset_custom = 0f, float yOffset_custom = 0f, float zOffset_custom = 0f)
	{
		this.EnableElements(fundsEnabled, coinsEnabled, keysEnabled);
		this.parentAnchor = base.gameObject.transform.parent.GetComponent<UIAnchor>();
		this.parentAnchor.transform.localPosition = new Vector3(this.parentAnchor.transform.localPosition.x, this.parentAnchor.transform.localPosition.y, this._depthOffset_default);
		if (!this.isInitialized)
		{
			this.OnCoinsChanged();
			this.OnKeysChanged();
			this.isInitialized = true;
		}
		WatchFreeViewSystem.Instance.AddNewTime("FreeViewCoins", 180);
	}

	private void OnCoinsChanged()
	{
		if (this.updateAutomatically && this._coinsParent.activeSelf)
		{
			this.coinAmountLabel.text = PlayerInfo.Instance.amountOfCoins.ToString();
		}
	}

	private void OnDisable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Remove(instance.onCoinsChanged, new Action(this.OnCoinsChanged));
		instance.onKeysChanged = (Action)Delegate.Remove(instance.onKeysChanged, new Action(this.OnKeysChanged));
	}

	private void OnEnable()
	{
		PlayerInfo instance = PlayerInfo.Instance;
		instance.onCoinsChanged = (Action)Delegate.Combine(instance.onCoinsChanged, new Action(this.OnCoinsChanged));
		instance.onKeysChanged = (Action)Delegate.Combine(instance.onKeysChanged, new Action(this.OnKeysChanged));
		if (this.isInitialized)
		{
			this.OnCoinsChanged();
			this.OnKeysChanged();
		}
	}

	private void OnKeysChanged()
	{
		if (this._keysParent.activeSelf)
		{
			this.keyAmountLabel.text = PlayerInfo.Instance.amountOfKeys.ToString();
		}
	}

	private void PlaceSegments()
	{
		this.CacheCoinsSegmentElements();
		this.CacheKeysSegmentElements();
		this._coinIcon.transform.localPosition = new Vector3(this._grayFGCoins.transform.localPosition.x - this._cachedCoinsFGWidth - this._cachedCoinIconWidth / 2f, this._grayFGCoins.transform.localPosition.y, this._grayFGCoins.transform.localPosition.z);
		this._grayFGKeys.transform.localPosition = new Vector3(this._grayFGCoins.transform.localPosition.x - this._cachedCoinsFGWidth, this._grayFGKeys.transform.localPosition.y, this._grayFGKeys.transform.localPosition.z);
		this.keyAmountLabel.transform.localPosition = new Vector3(this._grayFGKeys.transform.localPosition.x - this._cachedCoinIconOffset, this.keyAmountLabel.transform.localPosition.y, this.keyAmountLabel.transform.localPosition.z);
		this._keyIcon.transform.localPosition = new Vector3(this._grayFGKeys.transform.localPosition.x - this._cachedKeysFGWidth - this._cachedKeysIconWidth / 2f, this._keyIcon.transform.localPosition.y, this._keyIcon.transform.localPosition.z);
	}

	private void Update()
	{
		if (!WatchFreeViewSystem.Instance.IsCoolingDownOver("FreeViewCoins"))
		{
			this.freeDelayTime.text = Strings.Get(LanguageKey.UI_SCREEN_SHOP_FREE_GEMS_CD) + WatchFreeViewSystem.Instance.GetCoolingDownTime("FreeViewCoins");
			if (this.tweenScale.enabled)
			{
				this.tweenScale.enabled = false;
			}
		}
		else
		{
			this.freeDelayTime.text = string.Empty;
			if (!this.tweenScale.enabled)
			{
				this.tweenScale.enabled = true;
			}
		}
	}

	public void FreeCoinsOnClick()
	{
		if (!WatchFreeViewSystem.Instance.IsCoolingDownOver("FreeViewCoins"))
		{
			return;
		}
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(2);
			}
			else
			{
				UISliderInController.Instance.OnNetErrorPickedUp();
			}
		}
		else
		{
			UIScreenController.Instance.PushPopup("NoNetworkPopup");
		}
	}

	public bool updateAutomatically
	{
		get
		{
			return this._updateAutomatically;
		}
		set
		{
			this._updateAutomatically = value;
			if (value)
			{
				this.OnCoinsChanged();
				this.OnKeysChanged();
			}
		}
	}

	[SerializeField]
	private UISprite _grayFGCoins;

	[SerializeField]
	private UISprite _coinIcon;

	[SerializeField]
	private UISprite _grayFGKeys;

	[SerializeField]
	private UISprite _keyIcon;

	[SerializeField]
	private GameObject _coinsParent;

	[SerializeField]
	private GameObject _keysParent;

	[SerializeField]
	private UILabel coinAmountLabel;

	[SerializeField]
	private UILabel keyAmountLabel;

	[SerializeField]
	private TweenScale tweenScale;

	[SerializeField]
	private UILabel freeDelayTime;

	private float _cachedCoinIconOffset;

	private float _cachedCoinIconWidth;

	private float _cachedCoinsFGWidth;

	private float _cachedKeysFGWidth;

	private float _cachedKeysIconWidth;

	private float _depthOffset_default = -1f;

	private bool _updateAutomatically = true;

	private float COINS_FG_SCALE_OFFSET = 25f;

	private bool isInitialized;

	private float KEYS_FG_SCALE_OFFSET = 13f;

	private UIAnchor parentAnchor;

	public const string timeKey = "FreeViewCoins";
}
