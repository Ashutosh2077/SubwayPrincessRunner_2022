using System;
using UnityEngine;

public class UISlideInTaskSetHelper : UISlideIn
{
	private void Awake()
	{
		this._tasksInStoryLineSet = TasksManager.Instance.taskSetStoryCount + 1;
	}

	private int CalculateTheDisplayedMultiplierNumber(int multiplier)
	{
		if (multiplier < this._tasksInStoryLineSet)
		{
			this._queuedTaskSetSlideIns = UISliderInController.Instance.NumberOfTaskSetSlideIns();
			this._multiplierIsIncrementing = true;
		}
		else if (this._displayedMultiplier == 0)
		{
			this._queuedTaskSetSlideIns = 0;
		}
		else
		{
			this._queuedTaskSetSlideIns = this._tasksInStoryLineSet - (this._displayedMultiplier + 1);
		}
		return multiplier - this._queuedTaskSetSlideIns;
	}

	public void EnableCoinLabel(bool enable)
	{
		this.coinIcon.gameObject.SetActive(enable);
		this.coinLabel.gameObject.SetActive(enable);
	}

	public void SetupSlideInTaskSet(int multiplier)
	{
		base.gameObject.SetActive(true);
		if (this._displayedMultiplier == this._tasksInStoryLineSet)
		{
			this._multiplierIsIncrementing = false;
		}
		if (PlayerInfo.Instance.taskCompletedSum > this._tasksInStoryLineSet && !this._multiplierIsIncrementing)
		{
			this.superChest.enabled = true;
			this.superChest.transform.localPosition = new Vector3(-52f, 0f, 0f);
			this.lineReward.enabled = false;
			this.lineRewardShadow.enabled = false;
		}
		else
		{
			this.lineReward.enabled = true;
			this.lineRewardShadow.enabled = true;
			this._displayedMultiplier = this.CalculateTheDisplayedMultiplierNumber(multiplier);
			this.lineReward.text = "x" + this._displayedMultiplier;
			this.superChest.enabled = false;
		}
		this.line1.text = Strings.Get(LanguageKey.UI_TOP_TIP_TASK_SET_COMPLETE);
		this.EnableCoinLabel(false);
		this.SlideIn(null);
	}

	[SerializeField]
	private UILabel line1;

	[SerializeField]
	private UILabel lineReward;

	[SerializeField]
	private UILabel lineRewardShadow;

	[SerializeField]
	private UISprite superChest;

	[SerializeField]
	private UISprite coinIcon;

	[SerializeField]
	private UILabel coinLabel;

	private int _displayedMultiplier;

	private int _tasksInStoryLineSet;

	private bool _multiplierIsIncrementing;

	private int _queuedTaskSetSlideIns;
}
