using System;
using UnityEngine;

public class GameOverOldUI : MonoBehaviour, IGameOverUI
{
	public void Init(GameOverScreen screen)
	{
		UIEventListener uieventListener = UIEventListener.Get(this.doubleViewGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnDoubleClick);
		uieventListener = UIEventListener.Get(this.tryCharacterGo);
		uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnTryClick);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void RefreshUI(bool showTryRole, int coins)
	{
		this.doubleViewGo.SetActive(!showTryRole);
		this.tryCharacterGo.SetActive(showTryRole);
		if (!showTryRole)
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				this.collider_double.enabled = true;
				this.doubleViewSpr.color = Color.white;
				this.ts_double.PlayForward();
			}
			else
			{
				this.collider_double.enabled = false;
				this.doubleViewSpr.color = Color.cyan;
			}
		}
	}

	public void AfterTryCharacter()
	{
		if (this.tryCharacterGo.activeSelf)
		{
			this.tryCharacterGo.SetActive(false);
		}
	}

	private void OnTryClick(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(10);
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

	public void AfterDoubleCoins(int coins)
	{
		if (this.doubleViewGo.activeSelf)
		{
			this.doubleViewGo.SetActive(false);
		}
		if (PlayerInfo.Instance.hasSubscribed)
		{
			if (coins > 0)
			{
				FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, null, coins * 4);
			}
		}
		else if (coins > 0)
		{
			FreeRewardManager.Instance.SetFreeRewardType(RewardType.doublecoins, null, coins * 2);
		}
	}

	private void OnDoubleClick(GameObject go)
	{
		if (UIScreenController.Instance.CheckNetwork())
		{
			if (RiseSdk.Instance.HasRewardAd())
			{
				RiseSdk.Instance.ShowRewardAd(4);
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

	public void AfterLotteryClick()
	{
	}

	public bool FootAutoShow()
	{
		return true;
	}

	[SerializeField]
	private GameObject doubleViewGo;

	[SerializeField]
	private GameObject tryCharacterGo;

	[SerializeField]
	private TweenScale ts_double;

	[SerializeField]
	private BoxCollider collider_double;

	[SerializeField]
	private UISprite doubleViewSpr;
}
