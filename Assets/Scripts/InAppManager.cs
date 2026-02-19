using System;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private void Awake()
	{
		if (InAppManager._instance != null)
		{
			UnityEngine.Object.DestroyImmediate(this);
		}
		else
		{
			InAppManager._instance = this;
		}
	}

	public InAppManagerPopupData GetPopupData()
	{
		return this._popupData;
	}

	public static void Init()
	{
		if (InAppManager._instance == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "InAppManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<InAppManager>();
		}
	}

	public void SetupNativePopup(int cost, bool isPopup, InAppData.DataType type)
	{
		string popupTitle = string.Empty;
		bool isCoins = false;
		int num;
		if (type == InAppData.DataType.Coin)
		{
			isCoins = true;
			num = cost - PlayerInfo.Instance.amountOfCoins;
			popupTitle = Strings.Get(LanguageKey.NOT_ENOUGH_COINS);
		}
		else
		{
			num = cost - PlayerInfo.Instance.amountOfKeys;
			popupTitle = Strings.Get(LanguageKey.NOT_ENOUGH_KEYS);
		}
		string format = string.Empty;
		string popupDescription = string.Empty;
		if (type == InAppData.DataType.Coin)
		{
			if (num < 2)
			{
				format = Strings.Get(LanguageKey.PURCHASE_COINS_ONE_NO_INTERNET);
			}
			else
			{
				format = Strings.Get(LanguageKey.PURCHASE_COINS_MULTIPLE_NO_INTERNET);
			}
			popupDescription = string.Format(format, num);
		}
		else
		{
			if (num < 2)
			{
				format = Strings.Get(LanguageKey.PURCHASE_KEYS_ONE_NO_INTERNET);
			}
			else
			{
				format = Strings.Get(LanguageKey.PURCHASE_KEYS_MULTIPLE_NO_INTERNET);
			}
			popupDescription = string.Format(format, num);
		}
		if (UIScreenController.isInstanced)
		{
			this._popupData = new InAppManagerPopupData
			{
				popupTitle = popupTitle,
				popupDescription = popupDescription,
				isCoins = isCoins,
				lastIsPopup = isPopup
			};
			UIScreenController.Instance.PushPopup("NotEnoughCurencyPopup");
		}
	}

	public static InAppManager instance
	{
		get
		{
			InAppManager.Init();
			return InAppManager._instance;
		}
	}

	private static InAppManager _instance;

	private InAppManagerPopupData _popupData;
}
