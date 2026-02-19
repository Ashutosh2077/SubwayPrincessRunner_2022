using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : UIBaseScreen
{
	private void FillTable()
	{
		GameObject gameObject = NGUITools.AddChild(this._table.gameObject, this.listTitleComponent);
		ListTitleComponentHelper component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = string.Format("{0:000}", this.numberOfObjects);
		component.Setup(Strings.Get(LanguageKey.UPGRADES_TITLE), Strings.Get(LanguageKey.UPGRADES_DESCRIPTION));
		this.numberOfObjects++;
		int i = 0;
		int num = this.powerupPermanent.Length;
		while (i < num)
		{
			this.MakeBuyable(this.powerupPermanent[i], true);
			this.numberOfObjects++;
			i++;
		}
		gameObject = NGUITools.AddChild(this._table.gameObject, this.listTitleComponent);
		component = gameObject.GetComponent<ListTitleComponentHelper>();
		gameObject.name = string.Format("{0:000}", this.numberOfObjects);
		component.Setup(Strings.Get(LanguageKey.UPGRADE_SCREEN_SINGLE_USE), Strings.Get(LanguageKey.PROP_DESCRIPTION));
		this.numberOfObjects++;
		int j = 0;
		int num2 = this.powerupSingleUse.Length;
		while (j < num2)
		{
			this.MakeBuyable(this.powerupSingleUse[j], false);
			this.numberOfObjects++;
			j++;
		}
	}

	public override void Init()
	{
		base.Init();
		this.cachedUpgradeHelpers = new List<UpgradeHelper>(Upgrades.upgrades.Count);
		this.numberOfObjects = 0;
		this.FillTable();
		this.screenController = UIScreenController.Instance;
		base.InitializeCoinbox(true, true, true, 0f, 0f, 0f);
	}

	private GameObject MakeBuyable(PropType powerupType, bool permanent)
	{
		GameObject gameObject;
		if (permanent)
		{
			gameObject = NGUITools.AddChild(this._table.gameObject, this.PermanentPrefab);
			gameObject.GetComponent<UpgradeHelper>().InitPermanent(powerupType);
		}
		else
		{
			gameObject = NGUITools.AddChild(this._table.gameObject, this.ConsumablePrefab);
			gameObject.GetComponent<UpgradeHelper>().InitSingle(powerupType);
		}
		gameObject.GetComponent<UIDragScrollView>().scrollView = this._parentDragPanel;
		gameObject.name = string.Format("{0:000}", this.numberOfObjects);
		NGUITools.AddWidgetCollider(gameObject);
		this.cachedUpgradeHelpers.Add(gameObject.GetComponent<UpgradeHelper>());
		return gameObject;
	}

	public void OnScreenChanged(string screenName)
	{
		if (screenName != "UpgradesUI_shop")
		{
			if (this._isSubscribedToChangedScreen)
			{
				UIScreenController.Instance.OnPopupClosed -= this.PopupClosed;
				this._isReturningFromCelebration = false;
				this._isSubscribedToChangedScreen = false;
			}
		}
		else if (!this._isSubscribedToChangedScreen)
		{
			UIScreenController.Instance.OnPopupClosed += this.PopupClosed;
			this._isSubscribedToChangedScreen = true;
		}
	}

	private void PopupClosed(string popupName)
	{
		if (popupName == "CelebrationPopup")
		{
			this._isReturningFromCelebration = true;
		}
	}

	private IEnumerator ReadYPanelPosAfterFrames(int numberofFrames)
	{
		int frames = 0;
		while (frames < numberofFrames)
		{
			frames++;
			yield return null;
		}
		yield break;
	}

	private IEnumerator RefreshTable(bool isReturningFromCelebration)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this._table.Reposition();
		yield break;
	}

	public override void Show()
	{
		base.Show();
		if (this.screenController.GetTopScreenName() == "GameoverUI")
		{
			(this.screenController.GetScreenFromCache(this.screenController.GetTopScreenName()) as GameOverScreen).StopCoinsCountUp();
		}
		if (UpgradeScreen.forceScrollRearange && !this._isReturningFromCelebration)
		{
			UpgradeScreen.forceScrollRearange = false;
			this._parentDragPanel.ResetPosition();
		}
		if (this.screenController.GetTopScreenName() != "GameoverUI")
		{
			base.StartCoroutine(this.RefreshTable(this._isReturningFromCelebration));
		}
		this.RefreshUpgrade();
	}

	private void Start()
	{
		base.StartCoroutine(this.ReadYPanelPosAfterFrames(1));
		this.OnScreenChanged("UpgradesUI_shop");
	}

	private void OnDisable()
	{
		PurchaseHandler.Instance.RemoveOnUpgradePurchase(new Action(this.OnUpgradePurchase));
		RiseSdkListener.OnAdEvent -= this.OnFreeRewardCallback;
	}

	private void OnEnable()
	{
		PurchaseHandler.Instance.AddOnUpgradePurchase(new Action(this.OnUpgradePurchase));
		RiseSdkListener.OnAdEvent -= this.OnFreeRewardCallback;
		RiseSdkListener.OnAdEvent += this.OnFreeRewardCallback;
	}

	private void OnUpgradePurchase()
	{
		this.freeType = PropType._notset;
		this.RefreshUpgrade();
	}

	private void RefreshUpgrade()
	{
		if (this.cachedUpgradeHelpers == null)
		{
			return;
		}
		if (this.freeType == PropType._notset)
		{
			List<PropType> list = new List<PropType>();
			int i = 0;
			int num = this.powerupPermanent.Length;
			while (i < num)
			{
				Upgrade upgrade = Upgrades.upgrades[this.powerupPermanent[i]];
				if (PlayerInfo.Instance.GetCurrentTier(this.powerupPermanent[i]) < 3)
				{
					list.Add(this.powerupPermanent[i]);
				}
				i++;
			}
			if (list.Count == 0)
			{
				this.freeType = PropType._notset;
			}
			else
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				this.freeType = list[index];
			}
		}
		int j = 0;
		int count = this.cachedUpgradeHelpers.Count;
		while (j < count)
		{
			this.cachedUpgradeHelpers[j].RefreshUpgrade(this.freeType);
			j++;
		}
	}

	private void OnFreeRewardCallback(RiseSdk.AdEventType type, int id, string tag, int adType)
	{
		if (type == RiseSdk.AdEventType.RewardAdShowFinished && id == 8)
		{
			IvyApp.Instance.Statistics(string.Empty, string.Empty, "video_upgrades", 0, null);
			PlayerInfo.Instance.UseFreeUpgrade();
			PurchaseHandler.Instance.PurchaseUpgradeFree(this.freeType);
		}
	}

	public GameObject ConsumablePrefab;

	public GameObject PermanentPrefab;

	[SerializeField]
	private UITable _table;

	[SerializeField]
	private UIScrollView _parentDragPanel;

	[SerializeField]
	private GameObject listTitleComponent;

	public List<UpgradeHelper> cachedUpgradeHelpers;

	public PropType[] powerupSingleUse = new PropType[]
	{
		PropType.helmet,
		PropType.chest,
		PropType.scorebooster,
		PropType.headstart2000
	};

	public PropType[] powerupPermanent = new PropType[]
	{
		PropType.flypack,
		PropType.supershoes,
		PropType.coinmagnet,
		PropType.doubleMultiplier
	};

	private PropType freeType;

	private bool _isReturningFromCelebration;

	private bool _isSubscribedToChangedScreen;

	public static bool forceScrollRearange;

	private int numberOfObjects;

	private UIScreenController screenController;
}
