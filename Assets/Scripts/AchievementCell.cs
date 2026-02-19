using System;
using UnityEngine;

public class AchievementCell : MonoBehaviour
{
	private void Awake()
	{
		if (this.getRewardBtn != null)
		{
			this.ts = this.getRewardBtn.GetComponent<TweenScale>();
			this.rewardFillSpr = this.getRewardBtn.GetComponent<UISprite>();
		}
	}

	public void Init(int i, bool forceInit = false)
	{
		if (!forceInit && this.hasInited)
		{
			return;
		}
		this.index = i;
		this.hasInited = true;
	}

	public void RefreshUI()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(this.index + 3);
		AchievementInfo achievementInfo = TasksManager.Instance.GetAchievementInfo(this.index + 3);
		if (achievementInfo != null)
		{
			this.iconSpr.spriteName = achievementInfo.icon;
			this.rewardLbl.text = achievementInfo.rewardAmount.ToString();
			string spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			RewardType rewardType = achievementInfo.rewardType;
			if (rewardType != RewardType.coins)
			{
				if (rewardType == RewardType.keys)
				{
					spriteName = UIPosScalesAndNGUIAtlas.Instance.key;
				}
			}
			else
			{
				spriteName = UIPosScalesAndNGUIAtlas.Instance.coin;
			}
			this.rewardIconSpr.spriteName = spriteName;
		}
		this.context.text = string.Format(Strings.Get(taskInfo.template.ultraShortDescription), taskInfo.task.aim);
		this.getRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_DAILY_BUTTON_GET);
		this.progressLabel.text = taskInfo.progress.ToString() + "/" + taskInfo.task.aim;
		this.progressSlider.value = (float)taskInfo.progress / (float)taskInfo.task.aim;
		this.hasGetRewardLbl.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_COMPLETED_GOTTENLABEL);
		if (taskInfo.complete)
		{
			this.progressLabel.text = Strings.Get(LanguageKey.UI_POPUP_ACHIEVEMENT_COMPLETED);
		}
		this.ResetBtn();
	}

	private void ResetBtn()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(this.index + 3);
		if (PlayerInfo.Instance.GetCurrentAchievementAward(this.index))
		{
			this.checkMark.enabled = true;
			this.ts.enabled = false;
			this.getRewardBtn.SetActive(false);
			this.hasGetReward.SetActive(true);
			this.fillGreen.enabled = true;
			this.fillWhilte.enabled = false;
			this.rewardIconSpr.enabled = false;
			this.rewardLbl.text = string.Empty;
		}
		else
		{
			this.checkMark.enabled = false;
			this.getRewardBtn.SetActive(true);
			this.hasGetReward.SetActive(false);
			this.rewardIconSpr.enabled = true;
			if (taskInfo.complete)
			{
				this.rewardFillSpr.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.achievementCellFillSpriteName, "yellow");
				this.ts.enabled = true;
				this.ts.PlayForward();
				this.fillGreen.enabled = true;
				this.fillWhilte.enabled = false;
			}
			else
			{
				this.ts.enabled = false;
				this.ts.ResetToBeginning();
				this.rewardFillSpr.spriteName = string.Format(UIPosScalesAndNGUIAtlas.Instance.achievementCellFillSpriteName, "gray");
				this.fillWhilte.enabled = true;
				this.fillGreen.enabled = false;
			}
		}
	}

	private void OnClick()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(this.index + 3);
		AchievementInfo achievementInfo = TasksManager.Instance.GetAchievementInfo(this.index + 3);
		if (!PlayerInfo.Instance.GetCurrentAchievementAward(this.index) && taskInfo.complete)
		{
			FreeRewardManager.Instance.SetFreeRewardType(achievementInfo.rewardType, achievementInfo.rewardAmount, new Action(this.ResetBtn));
			PlayerInfo.Instance.SetCurrentAchivementReward(this.index, true);
		}
	}

	public int GetIndex()
	{
		return this.index;
	}

	public int GetState()
	{
		TaskInfo taskInfo = TasksManager.Instance.GetTaskInfo(this.index + 3);
		if (PlayerInfo.Instance.GetCurrentAchievementAward(this.index))
		{
			return 3;
		}
		if (taskInfo.complete)
		{
			return 1;
		}
		return 2;
	}

	[SerializeField]
	private UILabel getRewardLbl;

	[SerializeField]
	private UISprite fillWhilte;

	[SerializeField]
	private UISprite fillGreen;

	[SerializeField]
	private UISprite iconSpr;

	[SerializeField]
	private UILabel context;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UISprite checkMark;

	[SerializeField]
	private GameObject getRewardBtn;

	[SerializeField]
	private GameObject hasGetReward;

	[SerializeField]
	private UILabel hasGetRewardLbl;

	[SerializeField]
	private UISprite rewardIconSpr;

	[SerializeField]
	private UILabel rewardLbl;

	[SerializeField]
	private UISlider progressSlider;

	private TweenScale ts;

	private UISprite rewardFillSpr;

	private int index;

	private bool hasInited;
}
