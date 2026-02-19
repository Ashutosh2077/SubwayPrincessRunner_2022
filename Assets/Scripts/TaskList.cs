using System;
using System.Collections;
using UnityEngine;

public class TaskList : MonoBehaviour
{
	public void Show()
	{
		this.RefreshContents();
	}

	private IEnumerator _changeTaskCheckboxes(int delayFrames)
	{
		int num = 0;
		while (num < delayFrames)
		{
			num++;
			yield return null;
		}
		this.CheckBox1.value = this._currentTasks[0].complete;
		this.CheckBox2.value = this._currentTasks[1].complete;
		this.CheckBox3.value = this._currentTasks[2].complete;
		yield break;
	}

	private void RefreshContents()
	{
		if (this.titleLabel != null)
		{
			this.titleLabel.text = Strings.Get(LanguageKey.UI_SCREEN_PAUSE_UI_TITLE);
		}
		if (this.goalLbl != null)
		{
			this.goalLbl.text = "x" + (PlayerInfo.Instance.rawMultiplier + 1).ToString();
		}
		this._currentTasks = TasksManager.Instance.GetTaskInfo();
		if (this.hasCached)
		{
			bool flag = true;
			for (int i = 0; i < 3; i++)
			{
				if (this._cachedTaskInfo[i].progress != this._currentTasks[i].progress || this._cachedTaskInfo[i].complete != this._currentTasks[i].complete)
				{
					flag = false;
				}
			}
			if (this._cachedTaskSet != TasksManager.Instance.currentTaskSet)
			{
				flag = false;
			}
			if (flag)
			{
				return;
			}
		}
		base.StartCoroutine(this._changeTaskCheckboxes(1));
		this.hasCached = true;
		this.LabelAndNumberUpdate(0, this.Label1, this.Number1);
		this.LabelAndNumberUpdate(1, this.Label2, this.Number2);
		this.LabelAndNumberUpdate(2, this.Label3, this.Number3);
		this.Label1.alpha = (this._currentTasks[0].complete ? 0.5f : 1f);
		this.Label2.alpha = (this._currentTasks[1].complete ? 0.5f : 1f);
		this.Label3.alpha = (this._currentTasks[2].complete ? 0.5f : 1f);
		if (this.background1 != null)
		{
			this.background1.enabled = this._currentTasks[0].complete;
		}
		if (this.background2 != null)
		{
			this.background2.enabled = this._currentTasks[1].complete;
		}
		if (this.background3 != null)
		{
			this.background3.enabled = this._currentTasks[2].complete;
		}
	}

	private void LabelAndNumberUpdate(int taskArrayNr, UILabel sendTaskLabel, UILabel sendTaskNumber)
	{
		if (this._currentTasks[taskArrayNr].complete)
		{
			string format = (this._currentTasks[taskArrayNr].task.aim == 1) ? Strings.Get(this._currentTasks[taskArrayNr].template.ultraShortDescriptionSingle) : Strings.Get(this._currentTasks[taskArrayNr].template.ultraShortDescription);
			sendTaskLabel.text = string.Format(format, this._currentTasks[taskArrayNr].task.aim);
			sendTaskNumber.text = string.Empty;
		}
		else
		{
			string format2 = (this._currentTasks[taskArrayNr].task.aim == 1) ? Strings.Get(this._currentTasks[taskArrayNr].template.descriptionSingle) : Strings.Get(this._currentTasks[taskArrayNr].template.description);
			if (this._currentTasks[taskArrayNr].task.type == TaskType.TimeDeath)
			{
				if (UIScreenController.Instance.GetTopScreenName() == "PauseUI")
				{
					sendTaskLabel.text = string.Format(format2, this._currentTasks[taskArrayNr].task.aim, (int)Game.Instance.GetDuration());
					this.hasCached = false;
				}
				else
				{
					sendTaskLabel.text = string.Format(format2, this._currentTasks[taskArrayNr].task.aim, 0);
				}
			}
			else if (this._currentTasks[taskArrayNr].task.type == TaskType.OneOfEachPowerup)
			{
				sendTaskLabel.text = string.Format(format2, this._currentTasks[taskArrayNr].task.aim, this.oneOfEach);
			}
			else
			{
				sendTaskLabel.text = string.Format(format2, this._currentTasks[taskArrayNr].task.aim, this._currentTasks[taskArrayNr].task.aim - this._currentTasks[taskArrayNr].progress);
			}
			sendTaskNumber.text = taskArrayNr + 1 + string.Empty;
		}
	}

	private int oneOfEach
	{
		get
		{
			int num = 4;
			if (Game.Instance.IsInGame.Value)
			{
				if (GameStats.Instance.doubleMultiplierPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.coinMagnetsPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.superShoesPickups != 0)
				{
					num--;
				}
				if (GameStats.Instance.flypackPickups != 0)
				{
					num--;
				}
			}
			return num;
		}
	}

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel goalLbl;

	[SerializeField]
	private UIToggle CheckBox1;

	[SerializeField]
	private UIToggle CheckBox2;

	[SerializeField]
	private UIToggle CheckBox3;

	[SerializeField]
	private UILabel Label1;

	[SerializeField]
	private UILabel Label2;

	[SerializeField]
	private UILabel Label3;

	[SerializeField]
	private UILabel Number1;

	[SerializeField]
	private UILabel Number2;

	[SerializeField]
	private UILabel Number3;

	[SerializeField]
	private UISprite background1;

	[SerializeField]
	private UISprite background2;

	[SerializeField]
	private UISprite background3;

	private TaskInfo[] _cachedTaskInfo = new TaskInfo[3];

	private int _cachedTaskSet;

	private TaskInfo[] _currentTasks;

	private bool hasCached;
}
