using System;
using UnityEngine;

public class UpgradeHelper : MonoBehaviour
{
	public void InitPermanent(PropType type)
	{
		this._type = type;
		Upgrade upgrade = Upgrades.upgrades[type];
		this.powerupIcon.spriteName = upgrade.iconName;
		this.titleLabel.text = Strings.Get(upgrade.name).ToUpper();
		this.freeLabel.text = Strings.Get(LanguageKey.UI_POPUP_SAVE_ME_BUTTON_FREE);
		if (this.tierHelper != null)
		{
			this.tierHelper.SetupTiers(type);
		}
		if (PlayerInfo.Instance.GetCurrentTier(type) >= upgrade.numberOfTiers - 1)
		{
			if (this.button != null)
			{
				NGUITools.Destroy(this.button.gameObject);
			}
		}
		else
		{
			this.priceLabel.text = string.Empty + upgrade.getPrice(PlayerInfo.Instance.GetCurrentTier(type) + 1);
		}
		if (this.button != null)
		{
			this.button.initBuyButton(type);
		}
		this._hasInited = true;
	}

	public void InitSingle(PropType type)
	{
		this._type = type;
		Upgrade upgrade = Upgrades.upgrades[type];
		this.powerupIcon.spriteName = upgrade.iconName;
		this.titleLabel.text = Strings.Get(upgrade.name).ToUpper();
		this.priceLabel.text = string.Empty + upgrade.getPrice(0);
		switch (type)
		{
		case PropType.skiptask1:
		{
			TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(0);
			string format = (taskInfo.task.aim == 1) ? Strings.Get(taskInfo.template.ultraShortDescriptionSingle) : Strings.Get(taskInfo.template.ultraShortDescription);
			this.amountLabel.text = string.Format(format, taskInfo.task.aim);
			break;
		}
		case PropType.skiptask2:
		{
			TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(1);
			string format = (taskInfo.task.aim == 1) ? Strings.Get(taskInfo.template.ultraShortDescriptionSingle) : Strings.Get(taskInfo.template.ultraShortDescription);
			this.amountLabel.text = string.Format(format, taskInfo.task.aim);
			break;
		}
		case PropType.skiptask3:
		{
			TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(2);
			string format = (taskInfo.task.aim == 1) ? Strings.Get(taskInfo.template.ultraShortDescriptionSingle) : Strings.Get(taskInfo.template.ultraShortDescription);
			this.amountLabel.text = string.Format(format, taskInfo.task.aim);
			break;
		}
		default:
			if (type != PropType.chest)
			{
				this.amountLabel.text = Strings.Get(LanguageKey.UPGRADES_HELPER_YOU_HAVE) + " " + PlayerInfo.Instance.GetUpgradeAmount(type);
			}
			else
			{
				this.amountLabel.text = Strings.Get(LanguageKey.UPGRADES_HELPER_MYSTERY_BOX);
			}
			break;
		}
		if (this.button != null)
		{
			this.button.initBuyButton(type);
		}
		this._hasInited = true;
	}

	public void RefreshUpgrade(PropType type)
	{
		if (this._hasInited)
		{
			switch (this._type)
			{
			case PropType.helmet:
			case PropType.headstart500:
			case PropType.headstart2000:
			case PropType.scorebooster:
				if (this.amountLabel != null)
				{
					this.amountLabel.text = Strings.Get(LanguageKey.UPGRADES_HELPER_YOU_HAVE) + " " + PlayerInfo.Instance.GetUpgradeAmount(this._type);
				}
				break;
			case PropType.flypack:
			case PropType.supershoes:
			case PropType.coinmagnet:
			case PropType.letters:
			case PropType.doubleMultiplier:
				if (this.tierHelper == null || !this.tierHelper.ResetTiers())
				{
					Upgrade upgrade = Upgrades.upgrades[this._type];
					this.priceLabel.text = string.Empty + upgrade.getPrice(PlayerInfo.Instance.GetCurrentTier(this._type) + 1);
				}
				else if (this.button != null)
				{
					NGUITools.Destroy(this.button.gameObject);
				}
				break;
			}
			if (this.button != null)
			{
				this.button.Reload(this._type != type);
			}
		}
	}

	public UISprite powerupIcon;

	public UILabel titleLabel;

	public UILabel freeLabel;

	public UILabel priceLabel;

	public UILabel amountLabel;

	public BuyButtonIngame button;

	public UITierHelper tierHelper;

	private bool _hasInited;

	private PropType _type;
}
