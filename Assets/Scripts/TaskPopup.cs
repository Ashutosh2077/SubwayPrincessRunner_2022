using System;
using UnityEngine;

public class TaskPopup : UIBaseScreen
{
	private void Awake()
	{
		this.popupMainTitleLabel.text = Strings.Get(LanguageKey.TASK_POPUP_MAIN_TITLE);
		this.rewardLabel.text = Strings.Get(LanguageKey.TASK_POPUP_REWARD);
	}

	private void RefreshContents()
	{
		this._currentTasks = TasksManager.Instance.GetTaskInfo();
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (this._currentTasks[i].complete)
			{
				num++;
			}
		}
		this.setLabel.text = Strings.Get(LanguageKey.TASK_POPUP_TASK_SET_NUMBER) + " " + PlayerInfo.Instance.taskCompletedSum;
		this.progressSlider.value = (float)num / 3f;
		this.progressLabel.text = string.Format(Strings.Get(LanguageKey.TASK_POPUP_PROGRESS_DONE), (float)num / 3f);
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			this.RewardExplanationLine1.text = Strings.Get(LanguageKey.TASK_POPUP_SUPER);
			this.RewardExplanationLine2.text = Strings.Get(LanguageKey.TASK_POPUP_MYSTERY_BOX);
		}
		else
		{
			this.RewardExplanationLine1.text = Strings.Get(LanguageKey.TASK_POPUP_SCORE);
			this.RewardExplanationLine2.text = Strings.Get(LanguageKey.TASK_POPUP_MULTIPLIER);
		}
		if (PlayerInfo.Instance.rawMultiplier == 30)
		{
			this.goalLabel.text = string.Empty;
			this.goalLabelLine2.text = string.Empty;
			this.goalSprite.enabled = true;
		}
		else
		{
			this.goalLabel.text = "x" + (PlayerInfo.Instance.rawMultiplier + 1).ToString();
			this.goalLabelLine2.text = Strings.Get(LanguageKey.TASK_POPUP_SCORE);
			this.goalSprite.enabled = false;
		}
	}

	public override void Show()
	{
		base.Show();
		PlayerInfo.Instance.CheckIfWeShouldRemoveProgressForDailyQuestInRow();
		this.RefreshContents();
		this.taskList.Show();
	}

	[SerializeField]
	private TaskList taskList;

	[SerializeField]
	private UISlider progressSlider;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UILabel setLabel;

	[SerializeField]
	private UILabel goalLabel;

	[SerializeField]
	private UILabel goalLabelLine2;

	[SerializeField]
	private UISprite goalSprite;

	[SerializeField]
	private UILabel RewardExplanationLine1;

	[SerializeField]
	private UILabel RewardExplanationLine2;

	[SerializeField]
	private UILabel popupMainTitleLabel;

	[SerializeField]
	private UILabel rewardLabel;

	private TaskInfo[] _currentTasks;
}
