using System;
using UnityEngine;

public class LotteryButtonHelp : MonoBehaviour
{
	private void Awake()
	{
		this.usecoinAmound.text = this.amount.ToString();
	}

	private void OnEnable()
	{
		RiseSdkListener.OnAdEvent -= this.RewardAdSuc;
		RiseSdkListener.OnAdEvent += this.RewardAdSuc;
		this.nextLbl.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_NEXT_FREE_SPIN_TIME);
	}

	private void OnDisable()
	{
		RiseSdkListener.OnAdEvent -= this.RewardAdSuc;
	}

	public void RewardAdSuc(RiseSdk.AdEventType type, int id, string tag, int eventType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 7)
		{
			if (this.force)
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_lottery_endless", 0, null);
				this.freeVideoLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE_AGAIN);
			}
			else
			{
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_lottery", 0, null);
				PlayerInfo.Instance.UseLotteryWatchView();
			}
			if (this.OnButtonClick != null)
			{
				this.OnButtonClick();
			}
		}
	}

	public void OnClick()
	{
		if (this.type == 0)
		{
			if (PlayerInfo.Instance.amountOfCoins >= this.amount)
			{
				PlayerInfo.Instance.amountOfCoins -= this.amount;
				if (this.OnButtonClick != null)
				{
					this.OnButtonClick();
				}
			}
			else
			{
				PurchaseHandler.Instance.PurchaseCoinsIfNeeded(this.amount);
			}
		}
		else if (this.type == 1)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_all_success", 0, null);
			RiseSdk.Instance.TrackEvent("click_video_all_success", "default,default");
			if (this.force)
			{
				RiseSdk.Instance.TrackEvent("click_video_lottery_endless", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_lottery_endless", 0, null);
			}
			else
			{
				RiseSdk.Instance.TrackEvent("click_video_lottery", "default,default");
				IvyApp.Instance.Statistics(string.Empty, string.Empty, "click_video_lottery", 0, null);
			}
			if (UIScreenController.Instance.CheckNetwork())
			{
				if (this.force)
				{
					if (RiseSdk.Instance.HasRewardAd())
					{
						RiseSdk.Instance.ShowRewardAd(7);
					}
					else
					{
						UISliderInController.Instance.OnNetErrorPickedUp();
					}
				}
				else if (RiseSdk.Instance.HasRewardAd())
				{
					RiseSdk.Instance.ShowRewardAd(7);
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
		else if (this.type == 2)
		{
			if (this.OnButtonClick != null)
			{
				this.OnButtonClick();
			}
			PlayerInfo.Instance.UseLotteryWatchView();
			NotificationsObserver.Instance.NotifyNotificationDataChange(NotificationType.Lucky);
		}
	}

	public void Reload(bool force)
	{
		this.force = force;
		if (force)
		{
			this.type = 0;
			if (GameStats.Instance.gameOverPlayLotteryCount < 2)
			{
				if (GameStats.Instance.gameOverPlayLotteryCount == 1)
				{
					this.freeVideoLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE_AGAIN);
				}
				if (RiseSdk.Instance.HasRewardAd())
				{
					this.buttonTS.enabled = true;
					this.buttonCollider.enabled = true;
					this.type = 1;
					this.watchVideo.SetActive(true);
					this.free.SetActive(false);
					this.usePay.SetActive(false);
					this.fill.enabled = true;
					this.fill.spriteName = this.freeFillSpriteName;
					this.fill.color = new Color(255f, 255f, 255f, 255f);
					for (int i = 0; i < this.adSprits.Length; i++)
					{
						this.adSprits[i].color = new Color(255f, 255f, 255f, 255f);
					}
					this.freeVideoLabel.color = this.freeVideoLabelColor;
				}
				else
				{
					this.buttonTS.enabled = false;
					this.buttonCollider.enabled = false;
					this.type = 1;
					this.watchVideo.SetActive(true);
					this.free.SetActive(false);
					this.usePay.SetActive(false);
					this.fill.enabled = true;
					this.fill.spriteName = this.freeFillSpriteName;
					this.fill.color = new Color(0f, 255f, 255f, 255f);
					for (int j = 0; j < this.adSprits.Length; j++)
					{
						this.adSprits[j].color = new Color(0f, 255f, 255f, 255f);
					}
					this.freeVideoLabel.color = Color.gray;
				}
			}
			else
			{
				this.watchVideo.SetActive(false);
				this.free.SetActive(false);
				this.usePay.SetActive(false);
				this.fill.enabled = false;
				this.type = -1;
			}
			if (this.nextFreeTimeGo.activeInHierarchy)
			{
				this.nextFreeTimeGo.SetActive(false);
			}
			this.isActive = false;
			return;
		}
		this.isActive = true;
		if (PlayerInfo.Instance.CheckIfLotteryCanWatchFreeView())
		{
			if (PlayerInfo.Instance.lotteryWatchViewRemainCount == 3)
			{
				this.free.SetActive(true);
				this.watchVideo.SetActive(false);
				this.usePay.SetActive(false);
				this.fill.enabled = true;
				this.fill.spriteName = this.freeFillSpriteName;
				this.fill.color = new Color(255f, 255f, 255f, 255f);
				this.freeLabel.text = Strings.Get(LanguageKey.UI_POPUP_LOTTERY_BUTTON_SPIN_FREE);
				this.type = 2;
			}
			else if (RiseSdk.Instance.HasRewardAd())
			{
				this.watchVideo.SetActive(true);
				this.free.SetActive(false);
				this.usePay.SetActive(false);
				this.fill.enabled = true;
				this.fill.spriteName = this.freeFillSpriteName;
				this.fill.color = new Color(255f, 255f, 255f, 255f);
				this.type = 1;
			}
			else
			{
				this.watchVideo.SetActive(false);
				this.free.SetActive(false);
				this.usePay.SetActive(true);
				this.fill.enabled = true;
				this.fill.color = new Color(255f, 255f, 255f, 255f);
				this.fill.spriteName = this.payFillSpriteName;
				this.type = 0;
			}
		}
		else
		{
			this.watchVideo.SetActive(false);
			this.free.SetActive(false);
			this.usePay.SetActive(true);
			this.fill.enabled = true;
			this.fill.color = new Color(255f, 255f, 255f, 255f);
			this.fill.spriteName = this.payFillSpriteName;
			this.type = 0;
		}
	}

	private void Update()
	{
		if (!this.isActive)
		{
			return;
		}
		if (PlayerInfo.Instance.lotteryWatchViewRemainCount == 3)
		{
			if (this.nextFreeTimeGo.activeInHierarchy)
			{
				this.nextFreeTimeGo.SetActive(false);
			}
		}
		else
		{
			if (!this.nextFreeTimeGo.activeInHierarchy)
			{
				this.nextFreeTimeGo.SetActive(true);
			}
			this.timeLbl.text = PlayerInfo.Instance.NextFreeLotteryTime();
		}
	}

	[SerializeField]
	private UISprite fill;

	[SerializeField]
	private UILabel nextLbl;

	[SerializeField]
	private UILabel timeLbl;

	[SerializeField]
	private string freeFillSpriteName;

	[SerializeField]
	private string payFillSpriteName;

	[SerializeField]
	private GameObject nextFreeTimeGo;

	[SerializeField]
	private GameObject watchVideo;

	[SerializeField]
	private GameObject usePay;

	[SerializeField]
	private GameObject free;

	[SerializeField]
	private UILabel usecoinAmound;

	[SerializeField]
	private int amount;

	[SerializeField]
	private UILabel freeLabel;

	[SerializeField]
	private UILabel freeVideoLabel;

	[SerializeField]
	private Color freeVideoLabelColor;

	[SerializeField]
	private UISprite[] adSprits;

	[SerializeField]
	private BoxCollider buttonCollider;

	[SerializeField]
	private TweenScale buttonTS;

	public Action OnButtonClick;

	private bool force;

	private int type;

	private bool isActive;
}
